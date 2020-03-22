// Sphere
// s: radius
float sdSphere(float3 p, float s)
{
	return length(p) - s;
}

//(Infinite)Plane
// n.xyz : normal of the plane (normalized)
// n.w : offset
float sdPlane(float3 p, float4 n)
{
	return dot(p, n.xyz) + n.w;
}

// Box
// b: size of box in x/y/z
float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) +
		length(max(d, 0.0));
}

//Rounded box
float sdRoundBox(in float3 p, in float3 b, in float3 r) 
{
	float3 q = abs(p) - b;
	return min(max(q.x, max(q.y, q.z)), 0.0) + length(max(q, 0.0)) - r;
}

// BOOLEAN OPERATORS //

// Union
float4 opU(float4 d1, float4 d2)
{
	return (d1.w < d2.w) ? d1 : d2;
}

// Subtraction
float4 opS(float4 d1, float4 d2)
{
	return max(-d1.w, d2.w);
}

// Intersection
float4 opI(float4 d1, float4 d2)
{
	return max(d1.w, d2.w);
}

// Mod Position Axis
float pMod1 (inout float p, float size)
{
	float halfsize = size * 0.5;
	float c = floor((p+halfsize)/size);
	p = fmod(p+halfsize,size)-halfsize;
	p = fmod(-p+halfsize,size)-halfsize;
	return c;
}


// SMOOTH BOOLEAN OPERATORS //

// Union smooth
float4 opUS(float4 d1, float4 d2, float k) 
{
	float h = clamp(0.5 + 0.5*(d2.w - d1.w) / k, 0.0, 1.0);
	float3 color = lerp(d2.rgb, d1.rgb, h);
	float dist = lerp(d2.w, d1.w, h) - k * h * (1.0 - h);

	return float4(color, dist);
}

//Subtraction smooth
float4 opSS(float4 d1, float4 d2, float k)
{
	float h = clamp(0.5 - 0.5*(d2.w + d1.w) / k, 0.0, 1.0);
	float3 color = lerp(d2.rgb, d1.rgb, h);
	float dist = lerp(d2.w, -d1.w, h) + k * h*(1.0 - h);

	return float4(color, dist);
}

//Intersect smooth
float4 opIS(float4 d1, float4 d2, float k)
{
	float h = clamp(0.5 - 0.5*(d2.w - d1.w) / k, 0.0, 1.0);
	float3 color = lerp(d2.rgb, d1.rgb, h);
	float dist = lerp(d2.w, d1.w, h) + k * h * (1.0 - h);

	return float4(color, dist);
}