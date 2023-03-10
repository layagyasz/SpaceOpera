#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

void main()
{
    const float attenuation = 64f;
    out_color = vec4(vert_color.rgb, vert_color.a / (attenuation * pow(2 * vert_tex_coord.x - 1f, 2f)));
}