shader_type canvas_item;

uniform sampler2D perlin_noise: repeat_enable;
uniform sampler2D cell_noise: repeat_enable;
uniform sampler3D my_noise: repeat_enable;
uniform vec3 color_base : source_color;
uniform vec3 color_peak : source_color;
uniform float color_multiplier = 0.1;
uniform float time;
uniform float noise_depth = 0;

#define pow2(x) (x * x)
#define iResolution 1.0/SCREEN_PIXEL_SIZE

const int samples = 35;
const float sigma = float(samples) * 0.25;

float gaussian(vec2 i) {
	return 1.0 / (2.0 * PI * pow2(sigma)) * exp(-((pow2(i.x) + pow2(i.y)) / (2.0 * pow2(sigma))));
}

vec3 blur(sampler3D sp, vec2 uv, vec2 scale, float z) {
	vec3 col = vec3(0.0);
	float accum = 0.0;
	float weight;
	vec2 offset;
	for (int x = -samples / 2; x < samples / 2; ++x) {
		for (int y = -samples / 2; y < samples / 2; ++y) {
			offset = vec2(float(x), float(y));
			weight = gaussian(offset);
			vec2 a = uv + scale * offset;
			vec3 b = vec3(a, z);
			col += texture(sp, b).rgb * weight;
			accum += weight;
		}
	}
	return col / accum;
}

void fragment() {
	vec2 uv1 = vec2(UV.x + time * .02, UV.y + time * .005);
	vec2 uv2 = vec2(UV.x + time * .03, UV.y + time * .005);
	float noise1 = blur(my_noise, uv1, SCREEN_PIXEL_SIZE, noise_depth).r;
	float noise2 = blur(my_noise, uv2, SCREEN_PIXEL_SIZE, noise_depth + 1.0).r;

	float noise = smoothstep(noise1 * noise2 * 1.0, .0, .15);
	float multiplied_noise = noise * color_multiplier;
	vec3 color = mix(color_base, color_peak, multiplied_noise);

	COLOR = vec4(color, 1);
}