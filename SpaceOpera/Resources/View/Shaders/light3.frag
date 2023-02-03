#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;
in vec3 vert_normal;
in vec2 vert_normal_tex_coord;
in vec2 vert_lighting_tex_coord;
in vec3 vert_position;

layout(binding = 0) uniform sampler2D diffuse_texture;
layout(binding = 1) uniform sampler2D normal_texture;
layout(binding = 2) uniform sampler2D lighting_texture;

uniform vec3 light_position;
uniform vec4 light_color;
uniform float ambient;
uniform vec3 eye_position;

vec2 as_spherical(vec3 v)
{
    return vec2(atan(length(v.xz), v.y), atan(v.z, v.x));
}

vec3 as_cartesian(vec2 v)
{
    return vec3(sin(v.x) * cos(v.y), cos(v.x), sin(v.x) * sin(v.y));
}

vec3 combine_normals_tan(vec3 surface_normal, vec3 texture_normal)
{
    const float pi_over_2 = 1.57079632f;
    return as_cartesian(as_spherical(surface_normal) + as_spherical(texture_normal) - vec2(pi_over_2, pi_over_2));
}

void main()
{
    vec4 lighting = texture(lighting_texture, vert_lighting_tex_coord);
    vec2 specular_params = lighting.xy;
    float luminance = lighting.z;
    float roughness = lighting.w;

    vec3 texture_normal =  2 * texture(normal_texture, vert_normal_tex_coord).rgb - 1;
    texture_normal = normalize(vec3(texture_normal.x * roughness, texture_normal.y * roughness, texture_normal.z));
    vec3 normal = combine_normals_tan(normalize(vert_normal), texture_normal);

    vec3 light_normal = normalize(vert_position - light_position);
    vec3 eye_normal = normalize(eye_position - vert_position);
    float diffuse = max(0, dot(normal, light_normal));
    float specular = max(0, specular_params.x
        * pow(dot(normal, normalize(light_normal + eye_normal)), specular_params.y));

    vec4 diffuse_color = vert_color * texture(diffuse_texture, vert_tex_coord);
    vec4 c = diffuse_color * (ambient + luminance) + diffuse_color * light_color * (diffuse + specular);
    out_color = vec4(c.rgb, 1);
}