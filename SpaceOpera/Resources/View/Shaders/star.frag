#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_internal_position;

uniform float cycle_time;

void main()
{
    const float pi = 3.141592f;
    const float attenuation = 256f;
    const float flicker = 0.5;

    float cycle = mod(vert_color.a + cycle_time, 1);
    float f = flicker * (cos(cycle * 20 * pi) * sin(cycle * 10 * pi) + 1) + (1 - flicker);
    float a = attenuation * dot(vert_internal_position, vert_internal_position);

    out_color = f * vec4(vert_color.rgb, 1) / a;
}