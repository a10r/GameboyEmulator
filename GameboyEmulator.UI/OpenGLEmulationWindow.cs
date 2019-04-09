using System;
using GameboyEmulator.Core.Video;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameboyEmulator.UI
{
    public class OpenGLEmulationWindow : GameWindow
    {
        private Bitmap _currentFrame;

        private int _textureId;

        public OpenGLEmulationWindow() : base(160, 144, new GraphicsMode(32, 24, 0, 4), "GameboyEmulator")
        {
            VSync = VSyncMode.On;
            WindowBorder = WindowBorder.Fixed;
            // For some reason this needs to be here as well, parameters in base constructor are not enough.
            Width = 160;
            Height = 144;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
        }
        
        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Viewport(0, 0, 160, 144);
            GL.Ortho(0, 160, 144, 0, 0, 0);

            using (var pinned = _currentFrame.Pinned(BitmapOrientation.FlipY))
            {
                GL.DrawPixels(_currentFrame.Width, _currentFrame.Height, PixelFormat.Rgb, PixelType.UnsignedByte, pinned.Pointer);
            }

            SwapBuffers();
        }

        public void SetNextFrame(Bitmap newFrame)
        {
            _currentFrame = newFrame.Clone();
        }
    }
}
