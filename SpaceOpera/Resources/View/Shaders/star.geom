#version 430 core

layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

uniform mat4 projection;

in VS_OUT {
    vec4 color;
    vec2 scale;
} gs_in[]; 

out vec4 vert_color;
out vec2 vert_internal_position;
  
void main() {
    vert_color = gs_in[0].color;
    vec2 scale = gs_in[0].scale;

    vec4 top_left = (gl_in[0].gl_Position + vec4(-scale.x, -scale.y, 0, 0)) * projection;
    vec4 top_right = (gl_in[0].gl_Position + vec4(scale.x, -scale.y, 0, 0)) * projection;
    vec4 bottom_left = (gl_in[0].gl_Position + vec4(-scale.x, scale.y, 0, 0)) * projection;
    vec4 bottom_right = (gl_in[0].gl_Position + vec4(scale.x, scale.y, 0, 0)) * projection;

    gl_Position = top_left;
    vert_internal_position = vec2(-1, -1);
    EmitVertex();
    gl_Position = top_right;
    vert_internal_position = vec2(1, -1);
    EmitVertex();
    gl_Position = bottom_left;
    vert_internal_position = vec2(-1, 1);
    EmitVertex();
    gl_Position = bottom_right;
    vert_internal_position = vec2(1, 1);
    EmitVertex();
    EndPrimitive();
}