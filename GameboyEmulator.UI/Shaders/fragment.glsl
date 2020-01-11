#version 150
in vec2 frag_texcoord;

uniform sampler2D tex;

out vec4 outColor;

void main()
{
	outColor = texture(tex, frag_texcoord);
}