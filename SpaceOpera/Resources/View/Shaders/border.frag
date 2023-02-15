#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

void main()
{
    const float attenuation = 1f;
    out_color = vec4(vert_color.rgb, vert_color.a * (1f - attenuation * vert_tex_coord.x));
}