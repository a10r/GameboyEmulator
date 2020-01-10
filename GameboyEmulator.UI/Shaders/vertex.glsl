#version 150
in vec2 pos;
in vec2 texcoord;

out vec2 frag_texcoord;

void main()
{
    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
	frag_texcoord = texcoord;
}