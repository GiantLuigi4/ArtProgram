#version 430
precision highp float;
precision highp int;
precision highp sampler2D;

#ifdef COMPUTE
    layout (local_size_x = 32, local_size_y = 32, local_size_z = 1) in;
    layout(rgba16, binding = 0) uniform image2D Output;
    layout(binding = 1) uniform sampler2D Tex0;
    layout(binding = 2) uniform sampler2D Tex1;
#else
    in vec2 TexCoord;
    out vec4 color;
    uniform sampler2D Tex0;
    uniform sampler2D Tex1;
#endif

uniform float Alpha;
uniform int Function;
uniform vec2 Resolution;

//@formatter:off
vec4 blend(vec4 dst, vec4 src) {
    // alpha
    if (Function == 0) {
        if (src.a == 0) return dst;
        if (dst.a == 0) return src;

        float minAlpha = min(src.a, dst.a);
        float maxAlpha = max(src.a, dst.a);
        return vec4(
            (dst.rgb * (1.0 - src.a)) +
            (src.rgb * src.a),
            minAlpha / 2 + maxAlpha
        );
    } else

    // additive
    if (Function == 1) {
        // TODO: test to see if this makes sense on the alpha end
        return dst.rgba + vec4(src.rgb * src.a, src.a + dst.a);
    } else

    // multiplicative
    if (Function == 2) {
        // TODO: test to see if this makes sense on the alpha end
        return vec4(((dst.rgb * src.rgb) * src.a) + (dst.rgb * (1 - src.a)), src.a * dst.a);
    } else

    // invalid
    return vec4(0, 0, 0, 0);
}
//@formatter:on

void main() {
    #ifdef COMPUTE
        ivec2 coord = ivec2(gl_GlobalInvocationID.xy);
    #else
        ivec2 coord = ivec2(TexCoord * Resolution);
    #endif

    vec4 bottom;
    if (Resolution == textureSize(Tex0, 0)) { bottom = texelFetch(Tex0, coord, 0); }
    else { bottom = texture2D(Tex0, coord.xy / Resolution); }
    vec4 top;
    if (Resolution == textureSize(Tex1, 0)) { top = texelFetch(Tex1, coord, 0); }
    else { top = texture2D(Tex1, coord.xy / Resolution); }
    top.a *= Alpha;

    color = blend(bottom, top);
}
