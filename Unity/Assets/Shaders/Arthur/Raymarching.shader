Shader "Arthur/Raymarching"
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
			#pragma target 3.0

            #include "UnityCG.cginc"
			#include "DistanceFunctions.cginc"

			sampler2D _MainTex;

			//Camera
			uniform sampler2D _CameraDepthTexture;
			uniform float4x4 _CamFrustum;
			uniform float4x4 _CamToWorld;
			uniform float3 _camPos;
			uniform float _maxDistance;
			uniform int _maxIterations;
			uniform float _accuracy;
			
			//Light
			uniform float3 _lightDir;
			uniform float3 _lightCol;
			uniform float _lightIntensity;

			//Shadows
			uniform float2 _shadowDistance;
			uniform float _shadowIntensity;
			uniform float _shadowPenumbra;

			//Ambiant occlusion
			uniform float _aoStepSize;
			uniform int _aoIterations;
			uniform float _aoIntensity;

			//Reflection
			uniform int _reflectionCount;
			uniform float _reflectionIntensity;
			uniform float _envReflectionIntensity;
			uniform samplerCUBE _reflectionCube;
			
			//SDF
			uniform int _nbSphere;
			uniform float4 _spheres[999];
			uniform float _sphereSmooth;
			uniform float _degreRotate;
			uniform float _rotationSpeed;

			//Color
			uniform fixed4 _groundColor;
			uniform fixed4 _sphereColor[999];
			uniform float _colorIntensity;


			//Input
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			//Output
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 ray : TEXCOORD1;
            };
			
			//Vertex
            v2f vert (appdata v)
            {
                v2f o;
				half index = v.vertex.z;
				v.vertex.z = 0;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				o.ray = _CamFrustum[(int)index].xyz;

				o.ray /= abs(o.ray.z);

				o.ray = mul(_CamToWorld, o.ray);

                return o;
            }

			float4 distanceField(float3 p, float depth) 
			{
				//float4 distanceScene = float4(_planeColor.rgb, depth - length(p - _camPos));

				//Distance
				float4 sphereAdd = float4(_sphereColor[0].rgb, sdSphere(p - _spheres[0].xyz, _spheres[0].w));
				float4 result = sphereAdd;

				for (int i = 1; i < _nbSphere; i++)
				{
					//Distance
					sphereAdd = float4(_sphereColor[i].rgb, sdSphere(p - _spheres[i].xyz, _spheres[i].w));				
				
					result = opUS(result, sphereAdd, _sphereSmooth);
				}

				return result;
			}

			float3 getNormal(float3 p, float depth) 
			{
				const float2 offset = float2(0.001, 0);
				float3 n = float3(
					distanceField(p + offset.xyy, depth).w - distanceField(p - offset.xyy, depth).w,
					distanceField(p + offset.yxy, depth).w - distanceField(p - offset.yxy, depth).w,
					distanceField(p + offset.yyx, depth).w - distanceField(p - offset.yyx, depth).w);

				return normalize(n);
			}

			float hardShadow(float3 ro, float3 rd, float minT, float maxT, float depth)
			{
				for (float t = minT; t < maxT;)
				{
					float h = distanceField(ro + rd * t, depth).w;
					if (h < 0.001)
					{
						return 0.0;
					}

					t += h;
				}

				return 1.0;
			}

			float softShadow(float3 ro, float3 rd, float minT, float maxT, float k, float depth)
			{
				float result = 1.0;

				for (float t = minT; t < maxT;)
				{
					float h = distanceField(ro + rd * t, depth).w;
					if (h < 0.001)
					{
						return 0.0;
					}
					result = min(result, k * h / t);

					t += h;
				}

				return result;
			}

			float AmbiantOcclusion(float3 p, float3 n, float depth)
			{
				float step = _aoStepSize;
				float ao = 0.0;
				float dist;

				for (int i = 1; i <= _aoIterations; i++)
				{
					dist = step * i;
					ao += max(0.0, (dist - distanceField(p + n * dist, depth).w) / dist);
				}

				return (1.0 - ao * _aoIntensity);
			}

			float3 Shading(float3 p, float3 n, fixed3 c, float depth) 
			{
				float3 result;

				//Diffuse color
				float3 color = c.rgb * _colorIntensity;

				//Directional light
				float3 light = (_lightCol * dot(-_lightDir, n) * 0.5 + 0.5) * _lightIntensity;

				////Shadows
				float shadow = softShadow(p, -_lightDir, _shadowDistance.x, _shadowDistance.y, _shadowPenumbra, depth) * 0.5 + 0.5;
				shadow = max(0.0, pow(shadow, _shadowIntensity));

				//Ambant occlusion
				float ao = AmbiantOcclusion(p, n, depth);
				
				result = color * light * shadow * ao;

				return result;
			}

			bool raymarching(float3 ro, float3 rd, float depth, float maxDistance, int maxIteration, inout float3 p, inout fixed3 dColor)
			{
				bool hit;

				float t = 0; //distance travelled along the ray direction

				for (int i = 0; i < maxIteration; i++)
				{
					if (t > maxDistance || t >= depth)
					{
						//Environment
						hit = false;
						break;
					}

					p = ro + rd * t;

					//Check for a hit
					float4 d = distanceField(p, depth);
					if (d.w < _accuracy) //Hit !
					{
						dColor = d.rgb;
						hit = true;
						break;
					}

					t += d.w;
				}

				return hit;
			}

			//Fragment
			fixed4 frag(v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
				depth *= length(i.ray);

				fixed3 col = tex2D(_MainTex, i.uv);
				float3 rayDirection = normalize(i.ray.xyz);
				float3 rayOrigin = _WorldSpaceCameraPos;
				fixed4 result;
				float3 hitPosition;
				fixed3 dColor;

				bool hit = raymarching(rayOrigin, rayDirection, depth, _maxDistance, _maxIterations, hitPosition, dColor);

				if (hit)
				{
					//Shading
					float3 n = getNormal(hitPosition, depth);
					float3 s = Shading(hitPosition, n, dColor, depth);
					result = fixed4(s, 1);
					result += fixed4(texCUBE(_reflectionCube, n).rgb * _envReflectionIntensity * _reflectionIntensity, 0);

					//Reflection
					if (_reflectionCount > 0)
					{
						rayDirection = normalize(reflect(rayDirection, n));
						rayOrigin = hitPosition + (rayDirection * 0.01);
						hit = raymarching(rayOrigin, rayDirection, _maxDistance, _maxDistance * 0.5, _maxIterations * 0.5, hitPosition, dColor); //Pas de depth pour la reflection

						if (hit)
						{
							n = getNormal(hitPosition, depth);
							s = Shading(hitPosition, n, dColor, depth);
							result += fixed4(s * _reflectionIntensity, 0);

							if (_reflectionCount > 1)
							{
								rayDirection = normalize(reflect(rayDirection, n));
								rayOrigin = hitPosition + (rayDirection * 0.01);
								hit = raymarching(rayOrigin, rayDirection, _maxDistance, _maxDistance * 0.25, _maxIterations * 0.25, hitPosition, dColor); //Pas de depth pour la reflection

								if (hit)
								{
									n = getNormal(hitPosition, depth);
									s = Shading(hitPosition, n, dColor, depth);
									result += fixed4(s * _reflectionIntensity * 0.5, 0);
								}
							}
						}
					}
				}
				else
				{
					result = fixed4(0, 0, 0, 0);
				}

				return fixed4(col * (1.0 - result.w) + result.xyz * result.w ,1.0);
            }
            ENDCG
        }
    }
}
