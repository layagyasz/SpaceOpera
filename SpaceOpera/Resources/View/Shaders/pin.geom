#version 430 core

layout (lines) in;
layout (triangle_strip, max_vertices = 4) out;

uniform float width;
uniform mat4 view;
uniform mat4 projection;

in VS_OUT {
    vec4 color;
} gs_in[]; 

out vec4 vert_color;
out vec3 vert_position;
  
void main() {
    

    vec4 offset = vec4(width, 0, 0, 0);
    vec4 top = gl_in[0].gl_Position;
    vec4 bottom = gl_in[1].gl_Position;

    vert_color = gs_in[0].color;
    gl_Position = (top * view - offset) * projection;
    vert_position = (top - offset).xyz;
    EmitVertex();
    gl_Position = (top * view + offset) * projection;
    vert_position = (top + offset).xyz;
    EmitVertex();

    vert_color = gs_in[1].color;
    gl_Position = (bottom * view - offset) * projection;
    vert_position = (bottom - offset).xyz;
    EmitVertex();
    gl_Position = (bottom * view + offset) * projection;
    vert_position = (bottom + offset).xyz;
    EmitVertex();
    EndPrimitive();
}