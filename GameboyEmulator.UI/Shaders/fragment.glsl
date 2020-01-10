#version 150
in vec2 frag_texcoord;

uniform sampler2D tex;

out vec4 outColor;

void main()
{
	outColor = texture(tex, frag_texcoord) * vec4(1.0, 1.0, 1.0, 1.0);
    //outColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}