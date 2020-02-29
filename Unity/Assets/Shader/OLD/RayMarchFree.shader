Shader "Margot/RayMarchFree"
{
    /*Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }*/

    SubShader
    {
        Tags { "Queue"="Transparent" }
        //blend object behind object with this shader
        //Blend SrcAlpha OneMinusSrcAlpha;

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
                float3 wPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_WorldToObject, v.vertex).xyz;
                return o;
            }

            //---------------raymarching--------------------------
            //how many steps to looking for a hit
            #define STEPS 64
            #define STEP_SIZE 0.01

            bool SphereHit(float3 p, float3 centre, float radius)
            {
                return distance(p, centre) < radius;
            }

            //Search for raymarch hit : take position of the pixel and the ray from the camera to the object
            float RaymarchHit(float3 position, float3 direction)
            {
                //We increase the length of the ray -> !!depuis le début de la forme qui rend le shader et pas depuis la camera
                for(int i = 0; i < STEPS; i++)
                {
                    if(SphereHit(position, float3(0, 0, 0), 0.5))
                    {
                        return position;
                    }

                    position += direction * STEP_SIZE;
                }

                return 0;
            }


            //fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
                float3 worldPosition = i.wPos;
                float depth = RaymarchHit(worldPosition, viewDirection);

                if(depth != 0)
                    return fixed4(1, 0, 0, 1);
                else
                    return fixed4(1, 1, 1, 0);
            }
            ENDCG
        }
    }
}
