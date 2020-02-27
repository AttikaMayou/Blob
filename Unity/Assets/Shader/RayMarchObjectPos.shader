Shader "Margot/RayMarchObjectPos"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {"RenderType"="Opaque"}
        //blend object behind object with this shader
       // Blend SrcAlpha OneMinusSrcAlpha;

        // No culling or depth
       // Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            //vertex to fragment 
            struct v2f
            {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 rayOrigin : TEXTCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.rayOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0f));
                o.hitPos = v.vertex;
                return o;
            }
#define radius 0.3
            //---------------creation de la sphere--------------------------
            float getDistance(float3 p)
            {
                float dist = length(p) - radius;
                return dist;
            }

			//---------------smooth objet de la scene entre eux-------------
			float smooth(float a, float b, float k) {
				float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
				return lerp(b, a, h) - k * h * (1.0 - h);
				return a;
			}

			//TODO : finir creation de la scene
			float createScene(float3 position) 
			{
				float sphere = getDistance(position);
				float result = sphere;

				float sphere2 = getDistance(position);
				result = smooth(result, sphere2, 0.5);

				//Plane en sdf -> voir pour récupérer la vraie scene
				/*float sdPlane( vec3 p, vec4 n )
				{
					// n must be normalized
					return dot(p, n.xyz) + n.w;
				}*/
				return result;
			}



#define STEPS 100
#define MAX_DISTANCE 100
#define SURFACE_DISTANCE 0.001

            //Search for raymarch hit : take position of the pixel and the ray from the camera to the object
            float RaymarchHit(float3 rayOrigin, float3 rayDirection)
            {
                float dO = 0.f;
                float dS;

                //We increase the length of the ray -> !!depuis le début de la forme qui rend le shader et pas depuis la camera
                for(int i = 0; i < STEPS; i++)
                {
                    float3 p = rayOrigin + dO * rayDirection;
                    dS = getDistance(p);
                    dO += dS;

                    if (dS < SURFACE_DISTANCE || dO > MAX_DISTANCE)
                        break;
                }

                return dO;
            }

            //---------------NORMAL----------------------------------
            float3 estimateNormal(float3 p)
            {
                float2 epsilon = float2(0.01f, 0.f);
                float3 normal = getDistance(p) - float3(getDistance(p + epsilon.xyy), getDistance(p + epsilon.yxy), getDistance(p + epsilon.yyx));
                return normalize(normal);
            }



            //fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
				fixed4 col = 0;
                float2 uv = i.uv - 0.5f;
                float3 rayOrigin = i.rayOrigin;
                float3 rayDirection = normalize(i.hitPos - rayOrigin);

				float d = RaymarchHit(rayOrigin, rayDirection);

                if (d < MAX_DISTANCE)
                {
					float3 p = rayOrigin + rayDirection * d;
					float3 n = estimateNormal(p);
                    col.rbg = n;
                }
				else
				{
					discard;
				}
				//col.rg = uv;
                return col;
            }
            ENDCG
        }
    }
}
