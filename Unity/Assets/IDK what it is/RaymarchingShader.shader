// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RaymarchingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			// Provided by our script
			uniform float4x4 _FrustumCornersES;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float4x4 _CameraInvViewMatrix;
			uniform float3 _CameraWS;
			uniform float3 _LightDir;

			//Input 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
			
			//output
            struct v2f
            {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
            };

			float sdLink(float3 p, float le, float r1, float r2)
			{
				float3 q = float3(p.x, max(abs(p.y) - le, 0.0), p.z);
				return length(float2(length(q.xy) - r1, q.z)) - r2;
			}

			float sdTorus(float3 p, float2 t)
			{
				float2 q = float2(length(p.xz) - t.x, p.y);
				return length(q) - t.y;
			}

			void rotateAxis(float2 p, float a)
			{
				p = cos(a)*p + sin(a)*float2(p.y, -p.x);
			}

			// This is the distance field function.  The distance field represents the closest distance to the surface
			// of any object we put in the scene.  If the given point (point p) is inside of an object, we return a
			// negative answer.
			float map(float3 p)
			{
				// animate
				p.z += 2 * _Time.y;

				float size = 1;
				p = fmod(p, size) - size / 2.;

				// paramteres
				const float le = 0.13, r1 = 0.2, r2 = 0.09;

				// make a chain out of sdLink's
				float3 a = p; a.z = frac(a.z) - 0.5;
				float3 b = p; b.z = frac(b.z + 0.5) - 0.5;

				float geometry = 100.0;
				geometry = min(geometry, sdLink(a.xzy, le, r1, r2));
				geometry = min(geometry, sdLink(b.yzx, le, r1, r2));

				return geometry;
			}

			float3 calcNormal(float3 pos)
			{
				float2 e = float2(1.0, -1.0)*0.5773;
				const float eps = 0.0005;
				return normalize(e.xyy*map(pos + e.xyy*eps) +
					e.yyx*map(pos + e.yyx*eps) +
					e.yxy*map(pos + e.yxy*eps) +
					e.xxx*map(pos + e.xxx*eps));
			}

			// Raymarch along given ray
			// ro: ray origin
			// rd: ray direction
			fixed4 raymarch(float3 ro, float3 rd) {
				fixed4 ret = fixed4(0, 0, 0, 0);

				const int maxstep = 64;
				float t = 0; // current distance traveled along ray
				for (int i = 0; i < maxstep; ++i) {
					float3 p = ro + rd * t; // World space position of sample
					float d = map(p);       // Sample of distance field (see map())

					// If the sample <= 0, we have hit something (see map()).
					if (d < 0.001) {
						// Simply return a gray color if we have hit an object
						// We will deal with lighting later.

						//Light
						float3 n = calcNormal(p);
						ret = fixed4(dot(-_LightDir.xyz, n).rrr, 1);

						break;
					}

					// If the sample > 0, we haven't hit anything yet so we should march forward
					// We step forward by distance d, because d is the minimum distance possible to intersect
					// an object (see map()).
					t += d;
				}

				return ret;
			}

            v2f vert (appdata v)
            {
				v2f o;

				// Index passed via custom blit function in RaymarchGeneric.cs
				half index = v.vertex.z;
				v.vertex.z = 0.1;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
				#endif

				// Get the eyespace view ray (normalized)
				o.ray = _FrustumCornersES[(int)index].xyz;

				// Transform the ray from eyespace to worldspace
				// Note: _CameraInvViewMatrix was provided by the script
				o.ray = mul(_CameraInvViewMatrix, o.ray);
				return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				// ray direction
				float3 rd = normalize(i.ray.xyz);
				// ray origin (camera position)
				float3 ro = _CameraWS;

				fixed3 col = tex2D(_MainTex,i.uv); // Color of the scene before this shader was run
				fixed4 add = raymarch(ro, rd);

				// Returns final color using alpha blending
				return fixed4(col*(1.0 - add.w) + add.xyz * add.w,1.0);
			}
            ENDCG
        }
    }
}
