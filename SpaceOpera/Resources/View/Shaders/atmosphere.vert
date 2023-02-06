#version 430 core

layout(location = 0) in vec3 in_position;  
layout(location = 1) in vec4 in_color;
layout(location = 2) in vec2 in_tex_coord;
layout(location = 3) in vec3 in_normal;
layout(location = 4) in vec2 in_normal_tex_coord;
layout(location = 5) in vec2 in_lighting_tex_coord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec4 vert_color;
out vec3 vert_position;
out vec3 vert_normal;

void main(void)
{
	gl_Position = vec4(in_position, 1.0) * model * view * projection;
	vert_color = in_color;
	vert_position = (vec4(in_position, 1.0) * model).xyz;
	vert_normal = (vec4(in_normal, 0) * model).xyz;
}