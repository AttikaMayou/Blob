Shader "Custom/RaymarchingSurfaceShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Glossiness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow
        #pragma target 5.0
        #include "../Shaders/Arthur/DistanceFunctions.cginc"

        uniform float _CameraFOV;
        uniform float3 _CameraForward, _CameraRight, _CameraUp, _CameraPosition;

        uniform float smoothness;

        uniform int numberOfSpheres;
        uniform float4 sphereLocation[1000];

        uniform float4 _Color;

        uniform half _Metallic;
        uniform half _Glossiness;

        uniform int MAX_MARCHING_STEPS;

        uniform float _Width, _Height;

        struct Input
        {
            float3 color;
            float3 emission;
        };

        struct attribute
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
            uint id : SV_VertexID;
        };

        float4 distanceField(float3 p)
        {
            //Distance
            float4 sphereAdd = float4(_Color.rgb, sdSphere(p - sphereLocation[0].xyz, sphereLocation[0].w));
            float4 result = 1.0;

            for (int i = 1; i < numberOfSpheres; i++)
            {
                //Distance
                sphereAdd = float4(_Color.rgb, sdSphere(p - sphereLocation[i].xyz, sphereLocation[i].w));

                result = opUS(result, sphereAdd, smoothness);
            }

            return result;
        }

        float4 raymarching(float3 eye, float3 ray) 
        {
            float shade = 0.;
            float3 pos = eye;
            for (int index = 100; index > 0; --index) {

                float dist = distanceField(pos).w;

                if (dist < 0.001) 
                {
                    shade = (float)index / (float)100;
                    break;
                }
                pos += ray * dist;
            }
            return float4(pos, shade);
        }

        float3 getNormal(float3 pos) {
            float2 e = float2(.001, 0);
            return normalize(float3(distanceField(pos + e.xyy).w - distanceField(pos - e.xyy).w, distanceField(pos + e.yxy).w - distanceField(pos - e.yxy).w, distanceField(pos + e.yyx).w - distanceField(pos - e.yyx).w));
        }

        void vert(inout attribute v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float width = _Width;
            float height = _Height;
            float aspect = width / height;
            float fov = 1.0 / tan(_CameraFOV * PI / 180. / 2.);

            float2 uv = v.vertex.xy;
            float3 ray = normalize(_CameraForward * fov + _CameraRight * uv.x * aspect + _CameraUp * uv.y);
            float4 result = raymarching(_CameraPosition, ray);
            float3 pos = result.xyz;
            float shade = result.w;
            v.vertex.xyz = pos;
            v.normal = getNormal(pos);

            o.color = float4(distanceField(pos).xyz, 1);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
