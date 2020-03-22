Shader "Margot/RaymarchingSmooth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,0,0,1)
		_CubeMap ("CubeMap", CUBE) = "" {}
		_AmbiantLight("AmbiantLight", Range(0,1)) = 0.3
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
		SubShader
	{
		Tags { "RenderType" = "Translucent" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{

			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
#include "UnityCG.cginc"

			uniform float3 _CamForward;
			uniform float3 _CamRight;
			uniform float3 _CamUp;
			uniform float  _Fov;
			uniform float _Aspect;

			uniform int smoothFunctionChoosed;
			uniform float k;

			uniform float3 lightPosition;
			uniform float lightIntensity;
			uniform float3 lightColor;

			uniform int numberOfSpheres;
			uniform float4 sphereLocation[40];

			uniform sampler2D _CameraDepthTexture;

			uniform float4 _Color;
			float4 _SpecularColor;
			uniform float _Metallic;
			uniform float _Glossiness;
			uniform float _AmbiantLight;

			uniform int isMoving;
			uniform float3 forwardVector;

			uniform int MAX_MARCHING_STEPS;
			static const float EPSILON = 0.00001;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			samplerCUBE _CubeMap;

//-----------------------------SDF Functions-------------------------
			float signedSphere(float3 position, float radius) 
			{
				return length(position) - radius;
			}

			float sdPlane(float3 p, float4 n)
			{
				// n must be normalized
				return dot(p, n.xyz) + n.w;
			}

			//TEST DES DIFFERENTS SMOOTH
			// http://iquilezles.org/www/articles/smin/smin.htm
			float smin(float a, float b, float k) {
				float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
				return lerp(b, a, h) - k * h * (1.0 - h);
				return a;
			}
			// exponential smooth min (k = 32) => un peu moins bien
			float sminExp(float a, float b, float k)
			{
				float res = exp2(-k * a) + exp2(-k * b);
				return -log2(res) / k;
			}

			// polynomial smooth min (k = 0.1) => From Dreams 
			float sminDreams(float a, float b, float k)
			{
				float h = max(k - abs(a - b), 0.0) / k;
				return min(a, b) - h * h*k*(1.0 / 4.0);
			}

			// polynomial smooth min (k = 0.1);
			float sminCubic(float a, float b, float k)
			{
				float h = max(k - abs(a - b), 0.0) / k;
				return min(a, b) - h * h*h*k*(1.0 / 6.0);
			}

//---------------------------SCENE SDF --------------------------------------
			float sceneSDF(float3 position) {

				float sphere = signedSphere(position - sphereLocation[0].xyz, sphereLocation[0].w);
				float result = sphere;

				if (smoothFunctionChoosed == 0)
				{
					clamp(k, 0, 1);
					for (int i = 1; i < numberOfSpheres; ++i)
					{
						sphere = signedSphere(position - sphereLocation[i].xyz, sphereLocation[i].w);
						result = smin(result, sphere, k);
					}
				}
				else if (smoothFunctionChoosed == 1)
				{
					for (int i = 1; i < numberOfSpheres; ++i)
					{
						sphere = signedSphere(position - sphereLocation[i].xyz, sphereLocation[0].w);
						result = smin(result, sphere, k);
					}
				}
				else if (smoothFunctionChoosed == 2)
				{
					for (int i = 1; i < numberOfSpheres; ++i)
					{
						sphere = signedSphere(position - sphereLocation[i].xyz, sphereLocation[0].w);
						result = smin(result, sphere, k);
					}
				}
				else if (smoothFunctionChoosed == 3)
				{
					for (int i = 1; i < numberOfSpheres; ++i)
					{
						sphere = signedSphere(position - sphereLocation[i].xyz, sphereLocation[0].w);
						result = smin(result, sphere, k);
					}
				}
				
				return result;
			}

//----------------------------------RAYMARCHING----------------------------
			float rayMarching(float3 rayOrigin, float3 rayDirection, float min, float max) {

				float t = min;
				for (int i = 0; i < MAX_MARCHING_STEPS; i++) {
					float3 p = rayOrigin + (t * rayDirection);

					float dist = sceneSDF(p);

					if (dist < EPSILON) 
					{
						return t;
					}
					t += dist;

					if (t >= max) 
					{

						return max;
					}
				}
				return max;
			}

//------------------------------------------LIGHTING FUNCTION----------------------------------------------
			//the distribution of microfacet normals on a surface => BRDF
			float GGXNormalDistribution(float roughness, float NdotH)
			{
				float roughnessSqr = roughness * roughness;
				float NdotHSqr = NdotH * NdotH;
				float TanNdotHSqr = (1 - NdotHSqr) / NdotHSqr;
				return (1.0 / 3.1415926535) * sqrt(roughness / (NdotHSqr * (roughnessSqr + TanNdotHSqr)));
			}

			float3 calculateLighting(float3 normal, float3 viewDirection)
			{
				float3 texCube = texCUBE(_CubeMap, normal).rgb;
				_SpecularColor = float4(1, 1, 1, 1);

				float3 ambientLighting = _AmbiantLight * _Color;
				float3 normalDirection = normalize(normal);
				float3 lightDirection = _WorldSpaceLightPos0.xyz;
				float3 halfDirection = normalize(viewDirection + lightDirection);
				float NdotL = max(0.0, dot(normalDirection, lightDirection));
				float NdotH = max(0.0, dot(normalDirection, halfDirection));
				float NdotV = max(0.0, dot(normalDirection, viewDirection));
				float LdotH = max(0.0, dot(lightDirection, halfDirection));
				float attenuation = 1.f;
				float3 attenColor = attenuation * lightColor;

				//Roughness
				float roughness = 1 - (_Glossiness * _Glossiness);
				roughness = roughness * roughness;

				//Diffuse
				float3 diffuseColor = _Color.rgb * (1 - _Metallic);
				//float3 specColor = lerp(_SpecularColor.rgb, _Color.rgb, _Metallic * 0.5);

				//GGX NDF => Specular
				float SpecularDistribution = GGXNormalDistribution(roughness, NdotH);
				SpecularDistribution = SpecularDistribution * SpecularDistribution;

				float3 lightingModel = diffuseColor + saturate(SpecularDistribution);
				lightingModel = (lightingModel * saturate(NdotL));

				float4 finalDiffuse = float4((ambientLighting + lightingModel * attenColor), 1);
				return finalDiffuse;
			}

//----------------------------------normal---------------------------
			//http://jamie-wong.com/2016/07/15/ray-marching-signed-distance-functions/
			float3 estimateNormal(float3 p) {
				return normalize(float3(
					sceneSDF(float3(p.x + EPSILON, p.y, p.z)) - sceneSDF(float3(p.x - EPSILON, p.y, p.z)),
					sceneSDF(float3(p.x, p.y + EPSILON, p.z)) - sceneSDF(float3(p.x, p.y - EPSILON, p.z)),
					sceneSDF(float3(p.x, p.y, p.z + EPSILON)) - sceneSDF(float3(p.x, p.y, p.z - EPSILON))
					));
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				float3 rayOrigin = _WorldSpaceCameraPos;
				float2 myUv = i.uv;

				float fov = tan(_Fov);
				myUv.x = (2.0 * i.uv.x - 1.0) * _Aspect * fov;
				myUv.y = (1.0 - 2.0 * i.uv.y) * fov;
				float3 rayDirection = normalize(1.0 * _CamForward + _CamRight * myUv.x + _CamUp * myUv.y);

				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, myUv).r);
				depth *= length(rayDirection);

				float t = rayMarching(rayOrigin, rayDirection, _ProjectionParams.y, _ProjectionParams.z);// -depth);

				float attenuation;
				if (t < _ProjectionParams.z) 
				{
					float3 p = rayOrigin + (t * rayDirection);
					float3 normal = estimateNormal(p);
					float3 viewDirection = normalize(rayOrigin - p);

					col.rgb = calculateLighting(normal, viewDirection);
				}

				return col;
			}
			ENDCG
		}
	}
}