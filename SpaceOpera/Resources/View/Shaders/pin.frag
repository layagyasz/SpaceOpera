#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec3 vert_position;

uniform float dash_length;

void main()
{
	if (mod(vert_position.y, 2 * dash_length) < dash_length)
	{
		discard;
	}
	out_color = vert_color;
}