using System;
using System.Reflection;
using System.Linq;
using GameboyEmulator.Core.Video;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Bitmap = GameboyEmulator.Core.Video.Bitmap;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.IO;
using System.Runtime.InteropServices;

namespace GameboyEmulator.UI
{
    public class OpenGLEmulationWindow : GameWindow
    {
        private const int DefaultWidth = 160;
        private const int DefaultHeight = 144;

        private int _windowScaleFactor;
        public int WindowScaleFactor
        {
            get => _windowScaleFactor;

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Invalid scaling factor.");
                }
                _windowScaleFactor = value;

                // Set actual window size;
                Width = value * DefaultWidth;
                Height = value * DefaultHeight;

                GL.Viewport(0, 0, Width, Height);
            }
        }

        private Bitmap _currentFrame;

        // OpenGL handles.
        private int _textureId;
        private int _shaderProgram;
        private int _vertexArray;

        public OpenGLEmulationWindow() : base(DefaultWidth, DefaultHeight, new GraphicsMode(32, 24, 0, 4), "GameboyEmulator")
        {
            VSync = VSyncMode.On;
            WindowBorder = WindowBorder.Fixed;
            WindowScaleFactor = 1;
        }

        private string LoadShaderFromResources(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(res => res.EndsWith(name));
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private int SetUpVertexBuffers()
        {
            // Quad vertices
            var pos = new Vector2[] {
                new Vector2(-1f, -1f), // Bottom Left
                new Vector2(1f, -1f), // Bottom Right
                new Vector2(-1f, 1f), // Top Left
                new Vector2(1f, 1f), // Top Right
            };

            // Texture edge vertices
            var tex = new Vector2[] {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            };

            // Interleaved vertex position and texture position data.
            var vertices = new Vector2[] {
                pos[0], tex[0],
                pos[1], tex[1],
                pos[2], tex[2],
                pos[3], tex[3]
            };

            GL.GenBuffers(1, out int vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf(typeof(Vector2)), 
                vertices, BufferUsageHint.StaticDraw);

            return vertexBuffer;
        }

        private int SetUpShaderProgram()
        {
            var vertexShaderText = LoadShaderFromResources("vertex.glsl");
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderText);
            GL.CompileShader(vertexShader);

            string fragmentShaderText = LoadShaderFromResources("fragment.glsl");
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderText);
            GL.CompileShader(fragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.BindFragDataLocation(shaderProgram, 0, "outColor");

            GL.LinkProgram(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));

            GL.GetProgramInfoLog(shaderProgram, out string info);
            Console.WriteLine(info);

            return shaderProgram;
        }

        private int LinkShaderAttributes(int vertexBuffer, int shaderProgram)
        {
            GL.GenVertexArrays(1, out int vertexArrayObject);
            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

            var vectorSize = Marshal.SizeOf(typeof(Vector2));
            var stride = 2 * vectorSize;

            int posAttrib = GL.GetAttribLocation(shaderProgram, "pos");
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(posAttrib);

            int texAttrib = GL.GetAttribLocation(shaderProgram, "texcoord");
            GL.VertexAttribPointer(texAttrib, 2, VertexAttribPointerType.Float, false, stride, vectorSize);
            GL.EnableVertexAttribArray(texAttrib);

            if (posAttrib == -1 || texAttrib == -1)
            {
                Console.WriteLine("Shader attributes not set correctly.");
            }

            return vertexArrayObject;
        }

        private int SetUpAndConfigureFrameTexture()
        {
            var texId = GL.GenTexture();

            // Set up nearest neighbor filtering.
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return texId;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);

            var vertexBuffer = SetUpVertexBuffers();
            _shaderProgram = SetUpShaderProgram();
            _vertexArray = LinkShaderAttributes(vertexBuffer, _shaderProgram);
            _textureId = SetUpAndConfigureFrameTexture();
        }

        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            UpdateTexture(_currentFrame);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vertexArray);
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine(error);
            }

            SwapBuffers();
        }

        public void UpdateTexture(Bitmap frame)
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            using (var pinned = frame.Pinned(BitmapOrientation.FlipY))
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, frame.Width, frame.Height, 0,
                    PixelFormat.Rgb, PixelType.UnsignedByte, pinned.Pointer);
            }
        }
        
        public void SetNextFrame(Bitmap newFrame)
        {
            _currentFrame = newFrame.Clone();
        }
    }
}

