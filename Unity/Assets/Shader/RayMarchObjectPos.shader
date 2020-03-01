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
            uniform float3 positionsSphere1;
            uniform float3 positionsSphere2;

            uniform float3 _CamForward;
            uniform float3 _CamRight;
            uniform float3 _CamUp;

            uniform float4x4 CameraToWorldMatrix;
            uniform float4x4 CameraFrustrum;
            uniform float3 CameraWorldSPace;
            uniform float ratio;
            uniform float fov;

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

            //----------------creation de la plane----------------------------
            float sdPlane(float3 p, float4 n)
            {
                // n must be normalized
                return dot(p, n.xyz) + n.w;
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
				float sphere = getDistance(position - positionsSphere1);
				float result = sphere;

				float sphere2 = getDistance(position - positionsSphere2);
				result = smooth(result, sphere2, 0.5);

                //float plane = sdPlane(position, float4(0, 1, 0, 0));
                //result = smooth(result, plane, 0.5);

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
                    dS = createScene(p);
                    dO += dS;

                    if (dS < SURFACE_DISTANCE || dO > MAX_DISTANCE)
                        break;
                }

                return dO;
            }

            //---------------NORMAL----------------------------------
            float3 estimateNormal(float3 p)
            {
                float EPSILON = 0.01f;

                return normalize(float3(
                    createScene(float3(p.x + EPSILON, p.y, p.z)) - createScene(float3(p.x - EPSILON, p.y, p.z)),
                    createScene(float3(p.x, p.y + EPSILON, p.z)) - createScene(float3(p.x, p.y - EPSILON, p.z)),
                    createScene(float3(p.x, p.y, p.z + EPSILON)) - createScene(float3(p.x, p.y, p.z - EPSILON))
                    ));
            }

            //fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
				fixed4 col = 0;
                float2 uv = i.uv;
                float3 rayOrigin = i.rayOrigin;
                float3 rayDirection = normalize(i.hitPos - rayOrigin);

                //uv.x = (2.0 * i.uv.x - 1.0) * ratio;
                //uv.y = (1.0 - 2.0 * i.uv.y) * fov;
                
				float d = RaymarchHit(rayOrigin, rayDirection);

                if (d < MAX_DISTANCE)
                {
					float3 p = rayOrigin + rayDirection * d;
					float3 n = estimateNormal(p);
                    //float3 viewVec = normalize(rayOrigin - p);
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
