Shader "Custom/RaymarchingPoint"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow
        #pragma target 5.0
        #include "Common.cginc"

        uniform half _Metallic, _Glossiness;
        uniform float _Width, _Height, _CameraFOV;
        uniform float3 _CameraForward, _CameraRight, _CameraUp, _CameraPosition;

        float smoothmin (float a, float b, float r) 
        { 
            float h = clamp(.5+.5*(b-a)/r, 0., 1.); 
            return lerp(b, a, h)-r*h*(1.-h); 
        }

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

        float map (float3 pos) {
            float scene = 1.0;

            float a = 1.0;
            for (uint index = 0; index < 4; ++index) {
                rotation2D(pos.xz, _Time.y);
                rotation2D(pos.yz, _Time.y);
                pos.xz = abs(pos.xz)-2.0*a;
                scene = smoothmin(scene, length(pos)-1.0*a, 0.5);
                a /= 1.5;
            }

            return scene;
        }

        float4 raymarching (float3 eye, float3 ray) {

            float shade = 0.;
            float3 pos = eye;
            for (int index = 100; index > 0; --index) {
                float dist = map(pos);
                if (dist < 0.001) {
                    shade = (float)index/(float)100;
                    break;
                }
                pos += ray * dist;
            }
            return float4(pos, shade);
        }

        float3 getNormal (float3 pos) {
          float2 e = float2(.001,0);
          return normalize(float3(map(pos+e.xyy).x-map(pos-e.xyy).x, map(pos+e.yxy).x-map(pos-e.yxy).x, map(pos+e.yyx).x-map(pos-e.yyx).x));
        }

        void vert (inout attribute v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float width = _Width;
            float height = _Height;
            float aspect = width / height;
            float fov = 1.0 / tan( _CameraFOV*PI/180./2. );

            float2 uv = v.vertex.xy;
            float3 ray = normalize( _CameraForward*fov + _CameraRight*uv.x*aspect + _CameraUp*uv.y );
            float4 result = raymarching(_CameraPosition, ray);
            float3 pos = result.xyz;
            float shade = result.w;
            v.vertex.xyz = pos;
            v.normal = getNormal(pos);

            o.color = fixed3(1,1,1);// * shade;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
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
