Shader "Margot/RayMarch"
{
    /*Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }*/

    SubShader
    {
        Tags {"Queue"="Transparent"}
        //blend object behind object with this shader
       // Blend SrcAlpha OneMinusSrcAlpha;

        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

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
               // float3 hitPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //o.uv = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0f));
                //o.hitPos = v.vertex;
                return o;
            }

            //---------------raymarching--------------------------
            float getDistance(float3 p)
            {
                float dist = length(p) - 1.2f;
                return dist;
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
                float2 uv = i.uv;
                float3 rayOrigin = float3(0, 0, -3.f);
                float3 rayDirection = normalize(float3(uv.x, uv.y, 1.f));

                float d = RaymarchHit(rayOrigin, rayDirection);
                fixed4 col = 0;
/*
                if (d < MAX_DISTANCE)
                {
                    col.r = 1.f;
                }*/
                col.rg = uv;
                return col;
            }
            ENDCG
        }
    }
}
