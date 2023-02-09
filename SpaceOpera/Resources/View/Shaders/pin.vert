#version 430 core

layout(location = 0) in vec3 in_position;  
layout(location = 1) in vec4 in_color;
layout(location = 2) in vec2 in_tex_coords;

uniform mat4 model;

out VS_OUT {
    vec4 color;
} vs_out;

void main(void)
{
	gl_Position = vec4(in_position, 1.0) * model;
	vs_out.color = in_color;
}