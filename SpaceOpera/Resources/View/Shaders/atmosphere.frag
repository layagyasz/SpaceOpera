#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec3 vert_position;
in vec3 vert_normal;

uniform vec3 center_position;
uniform float outer_radius;
uniform float inner_radius;
uniform vec3 eye_position;
uniform vec3 light_position;
uniform vec4 light_color;
uniform float light_luminance;
uniform float light_attenuation;
uniform float atmosphere_density;
uniform float atmosphere_density_falloff;
uniform int atmosphere_precision;

vec2 quadratic(float a, float b, float c)
{
    const float max_value = 1000000;
    const float min_value = -1000000;
    float det = b * b - 4 * a * c;
    if (det < 0)
    {
        return vec2(max_value, min_value);
    }
    return vec2((-b - sqrt(det)) / (2 * a), (-b + sqrt(det)) / (2 * a));
}

vec2 intersect_sphere(vec3 sphere_center, float sphere_radius, vec3 ray_origin, vec3 ray_direction)
{
    vec3 q = ray_origin - sphere_center;
    return 
        quadratic(
            dot(ray_direction, ray_direction), 2 * dot(ray_direction, q), dot(q, q) - sphere_radius * sphere_radius);
}

float density(vec3 p)
{
    float h = (distance(p, center_position) - inner_radius) / (outer_radius - inner_radius);
    return atmosphere_density * exp(-h * atmosphere_density_falloff) * (1 - h);
}

float optical_depth(vec3 ray_origin, vec3 ray_direction, float ray_length)
{
    vec3 p = ray_origin;
    float step_size = ray_length / atmosphere_precision;
    float result = 0;
    for (int i=0; i<atmosphere_precision + 1; ++i)
    {
        result += step_size * density(p);
        p += step_size * ray_direction;
    }
    return result;
}

vec4 atmosphere_light(vec3 ray_origin, vec3 ray_direction, float ray_length)
{
    vec3 scatter_point = ray_origin;
    float step_size = ray_length / atmosphere_precision;
    vec3 scattered = vec3(0, 0, 0);
    float view_ray_optical_depth = 0;
    vec3 light_direction = normalize(light_position - vert_position);
    for (int i=0; i<atmosphere_precision + 1; ++i)
    {
        float sun_ray_length = intersect_sphere(center_position, outer_radius, scatter_point, light_direction).y;
        float sun_optical_depth = optical_depth(scatter_point, light_direction, sun_ray_length);
        view_ray_optical_depth = optical_depth(scatter_point, -ray_direction, step_size * i);
        vec3 t = exp(-(sun_optical_depth + view_ray_optical_depth) * vert_color.rgb);
        float d = density(scatter_point);

        scattered += step_size * d * t * vert_color.rgb;
        scatter_point += step_size * ray_direction;
    }
    return vec4(scattered, 1 - exp(-view_ray_optical_depth));
}

void main()
{
    vec3 eye_normal = normalize(vert_position - eye_position);
    if (dot(eye_normal, vert_normal) < 0)
    {
        discard;
    }

    vec2 outer_intersection = intersect_sphere(center_position, outer_radius, eye_position, eye_normal);
    vec2 inner_intersection = intersect_sphere(center_position, inner_radius, eye_position, eye_normal);
    float d = min(inner_intersection.x, outer_intersection.y) - outer_intersection.x;
    if (d > 0)
    {
        vec3 light_direction = light_position - vert_position;
        float l = light_luminance / (light_attenuation * dot(light_direction, light_direction));
        vec4 a = atmosphere_light(eye_position + eye_normal * outer_intersection.x, eye_normal, d);
        a.a = clamp(a.a, 0, 0.8f);
        out_color = vec4(clamp(l * light_color.rgb * a.rgb, 0f, 1f), a.a);
    }
    else
    {
        discard;
    }
}