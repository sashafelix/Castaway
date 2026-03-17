#ifndef __SYM_OCEAN_WAVE_DEFINED__
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
#define __SYM_OCEAN_WAVE_DEFINED__





uint sym_lowbias32(uint x) {
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return x;
}

uint pcg_hash(uint input)
{
    uint state = input * 747796405u + 2891336453u;
    uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
    return (word >> 22u) ^ word;
}

float2 sym_hash2f(float2 pos) {
    uint x = asuint(pos.x);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x ^= asuint(pos.y);
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return float2(x>>16, 0xFFFF&x) / 65535.0;
}

float2 sym_hash2f(int2 ipos) {
    uint x = asuint(ipos.x);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x ^= asuint(ipos.y);
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return float2(x>>16, 0xFFFF&x) / 65535.0;
}

// 11bits + 11bits + 10bits
float3 sym_hash3f_BBA(int2 ipos) {
    uint x = asuint(ipos.x);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x ^= asuint(ipos.y);
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return float3(x>>21, 0x7FF & (x>>10), 0x3FF & x) / float3(0x7FF, 0x7FF, 0x3FF);
}

// 10b + 10b + 6b + 6b
float4 sym_hash4f_AA66(int2 ipos) {
    uint x = asuint(ipos.x);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x ^= asuint(ipos.y);
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return float4(x>>22, 0x3FF & (x>>12), 0x3F & (x>>6), 0x3F & x) / float4(0x3FF, 0x3FF, 0x3F, 0x3F);
}

float sym_hashf(int2 ipos) {
    uint x = asuint(ipos.x);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x ^= asuint(ipos.y);
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return (x>>1)/(float)0x8FFFFFFF;
}

void sym_hash2f_float(float2 pos, out float2 h) {
    h = sym_hash2f(pos);
}

float2  hash2( float2  p ) { p = float2( dot(p,float2(127.1,311.7)), dot(p,float2(269.5,183.3)) ); return frac(sin(p)*43758.5453); }



float sym_smin_root(float a, float b, float k) {
    k *= 2.0;
    float x = b-a;
    return 0.5*( a+b-sqrt(x*x+k*k) );
}

// quadratic polynomial
float2 sym_smin_quadratic( float a, float b, float k )
{
    float h = 1.0 - min( abs(a-b)/(4.0*k), 1.0 );
    float w = h*h;
    float m = w*0.5;
    float s = w*k;
    return (a<b) ? float2(a-s,m) : float2(b-s,1.0-m);
}


void smooth_voronoi_float(float3 uvw, float k, out float h, out float3 n, out float2 c, out float ch) {
    int2 p = floor( uvw.xy );
    float2  f = frac( uvw.xy );
    
    h = 8;
    n = 0;
    c = 0;
    ch = 0;

    for (int j=-1; j<=1; j++ ) {
        for(int i=-1; i<=1; i++ ) {
            int2 b = int2( i, j );
            int2 pb = p + b;
            float4 hash = sym_hash4f_AA66(pb);
            float w = sin(uvw.z + hash.z * PI) * 0.8;

            float3 offset = float3(hash.xy, w);
            
            float3 r = float3(b, 0) - float3(f, 0) + offset;
            
            float2 nc = float3(pb.xy + offset.xy, 0);
            float nch = hash.w;

            float dsq = dot(r, r);
            float d = sqrt(dsq);

            float3 nn = float3(-r.xy,1);
            
		    float m = smoothstep( -1.0, 1.0, (h - d) / k);
            float res = m*(1.0-m)*k/(1.0+3.0*k);

            h = lerp( h, dsq, m ) - res;
            n = lerp( n, nn, m ) - res;
            c = lerp( c, nc, m ) - res;
            ch = lerp( ch, nch, m ) - res;
            
        }
    }
}



void approx_ocean_wave_float(float x, float t, float k, out float y, out float d, out float2 n) {
    x = frac(x+t);

    bool is_left = t > x;
    float a = is_left ? (t) : (1-2*t+x);
    float b = is_left ? (2*t*t/x-t) : (1-x); 
    
    float ak = pow(a, k);
    float bk = pow(b, k);
    float ak_bk = ak + bk;

    y = cos( PI * ak / ak_bk );
    y *= y;

    float c = is_left ? (-1-t) : (-1+t);

    d = k * PI * c * ak * bk * sin(2 * PI * ak / ak_bk) / (a * b * ak_bk * ak_bk);

    n = float2(-d, 1);

}


float sym_rational_smoothstep(float x, float n){
    return pow(x,n)/(pow(x,n)+pow(1.0-x,n));
}

float sym_param_smoothstep(float x, float j, float n){
    float t = sym_rational_smoothstep(x, n);
    return lerp(j*x, j*x+1-j, t);
}

float sym_reciprocal_lerp(float x, float n) {
    float nx = n * x;
    return n < 0 ? ((x - nx)/(1 - nx)) : (x/(1 + n - nx));
}


void simple_tidal_wave_float(float x, float skew, float steep, out float y) {
    x = frac(x);

    float n = sign(skew) * skew * skew * 10;
    float j = lerp(0.85, 1.5, abs(skew) / 10);

    float c = cos(sym_param_smoothstep(sym_reciprocal_lerp(x, n), j, steep) * PI);

    y = 1 - c * c;
}



#endif