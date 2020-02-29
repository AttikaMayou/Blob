Shader "Margot/RayMarchSmooth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			//#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
			//#include "Lighting.cginc"
			//#include "UnityDeferredLibrary.cginc"
			uniform float3 _CamForward;
			uniform float3 _CamRight;
			uniform float3 _CamUp;
			uniform float  _Fov;

			uniform float3  _p1;
			uniform float3  _p2;
			uniform float radius;
			float rayOrigin; 

			StructuredBuffer<float> _LightInfo;

			//how many steps to looking for a hit
			#define STEPS 64
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
			//-----------------------------RAYMARCHING--------------------------
			//TODO : pass radius uniform
			float signedSphere(float3 position, float radius) {
				return length(position) - radius;
			}
			
			float smin(float a, float b, float k) {
				float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
				return lerp(b, a, h) - k * h * (1.0 - h);
				return a;
			}
			
			float sceneSDF(float3 position) {

				float sphere = signedSphere(position - _p1, 0.3);
				float result = sphere;

				float sphere2 = signedSphere(position - _p2, 0.3);
				result = smin(result, sphere2, 0.5);

				return result;
			}

			//Search for raymarch hit : take position of the pixel and the ray from the camera to the object
			float RaymarchHit(float3 rayOrigin, float3 rayDirection, float min, float max)
			{
				float t = min;
				//We increase the length of the ray -> !!depuis le début de la forme qui rend le shader et pas depuis la camera
				for (int i = 0; i < STEPS; i++)
				{
					float3 p = rayOrigin + (t * rayDirection);

					float dist = sceneSDF(p);

					if (dist < EPSILON) {
						return t;
					}

					t += dist;

					if (t >= max) {

						return max;
					}
				}
				return max;
			}
			//---------------------LIGHT--------------------
			float3 lambertShading(float3 normal, float3 lightDirection, float3 diffuse, float3 lightColor) {
				float3 col = lightColor * (dot(normal, lightDirection) * diffuse);
				return clamp(col, 0.0, 1.0);
			}

			float3 phongShading(float3 normal, float3 lightDirection, float3 diffuse, float3 viewVec, float3 lightColor) {
				float3 lambertian = lambertShading(normal, lightDirection, diffuse, lightColor);
				float3 h = normalize(viewVec + lightDirection);
				float3 specular = float3(0.1, 0.1, 0.1) * pow(max(0, dot(normal, h)), 10);
				float3 col = lambertian + specular;
				return clamp(col, 0.0, 1.0);
			}
			
			float3 calculateLight(float3 color, float3 p, float3 viewVec, float3 normal, float3 aLightPos, float3 lightColor) {
				float3 lightDirection = normalize(aLightPos - p);

				float lightDistance = length(aLightPos - p);
				color += phongShading(normal, lightDirection, float3(1.0, 0.0, 0.0), viewVec, lightColor);
				return color;
			}

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

				float3 rayOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0f));
				float2 myUv = i.uv;

				float fov = tan(_Fov);

				myUv.x = (2.0 * i.uv.x - 1.0) * (_ScreenParams.x / _ScreenParams.y) * fov;
				myUv.y = (1.0 - 2.0 * i.uv.y) * fov;
				float3 rayDirection = normalize(1.0 * _CamForward + _CamRight * myUv.x + _CamUp * myUv.y);
				//-----------------RAYMARCHING------------------
				float t = RaymarchHit(rayOrigin, 1.0, _ProjectionParams.y, _ProjectionParams.z);

				if (t < _ProjectionParams.z) {
					float3 color = float3(0.0, 0.0, 0.0);
					float3 p = rayOrigin + (t * rayDirection);
					float3 normal = estimateNormal(p);
					float3 viewVec = normalize(rayOrigin - p);

					uint numStructs = 0;
					uint numStrides = 0;
					_LightInfo.GetDimensions(numStructs, numStrides);
					for (uint i = 0; i < numStructs / 4; i++) {

						float4 lightPos = float4(_LightInfo[(i * 4) + 0], _LightInfo[(i * 4) + 1], _LightInfo[(i * 4) + 2], _LightInfo[(i * 4) + 3]);

						color += calculateLight(color, p, viewVec, normal, lightPos.xyz, float3(1.0, 1.0, 1.0));
					}


					color = clamp(color, 0.0, 1.0);
					col.rgb = color;
				}

				return col;
			}
			ENDCG
		}
	}
}