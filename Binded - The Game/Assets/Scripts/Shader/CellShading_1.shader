Shader "Binded/CellShading_1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)                     // cor base do modelo
        _MainTex ("Albedo (RGB)", 2D) = "white" {}              // Textura de Albedo
        [Normal]_Normal("Normal", 2D) = "bump" {}               // Mapa de normais
        _LightCutoff("Light cutoff", Range(0,1)) = 0.5          // Cut off da luz
        _ShadowBands("Shadow bands", Range(1,4)) = 1            // numero de bandas para a luz
 
        [Header(Specular)]
        _SpecularMap("Specular map", 2D) = "white" {} 
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [HDR]_SpecularColor("Specular color", Color) = (0,0,0,1)
 
        [Header(Rim)]
        _RimSize("Rim size", Range(0,1)) = 0
        [HDR]_RimColor("Rim color", Color) = (0,0,0,1)
        [Toggle(SHADOWED_RIM)]
        _ShadowedRim("Rim affected by shadow", float) = 0
         
        [Header(Emission)]
        [HDR]_Emission("Emission", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags {"RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM     
        #pragma surface surf CelShaded fullforwardshadows 
        #pragma shader_feature SHADOWED_RIM
        #pragma target 4.6

        #include "UnityPBSLighting.cginc"


        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _Normal;
        float _LightCutoff;
        float _ShadowBands;
 
 
        sampler2D _SpecularMap;
        half _Glossiness;
        fixed4 _SpecularColor;
 
        float _RimSize;
        fixed4 _RimColor;
 
        fixed4 _Emission;
 
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
            float2 uv_SpecularMap;
        };
 
        inline half4 LightingCelShaded (SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {            

            half nDotL = saturate(dot(s.Normal, normalize(gi.light.dir)));
            half diff = round(saturate(nDotL/ _LightCutoff) * _ShadowBands) / _ShadowBands;
 
            float3 refl = reflect(normalize(gi.light.dir), s.Normal);
            float vDotRefl = dot(viewDir, -refl);
            float3 specular = _SpecularColor.rgb * step(1 - s.Smoothness, vDotRefl);
             
            float3 rim = _RimColor * step(1 - _RimSize ,1 - saturate(dot(normalize(viewDir), s.Normal)));
             
            half3 col = (s.Albedo + specular) * gi.light.color;
 
            half4 c;
            
            #ifdef SHADOWED_RIM
            c.rgb = (col + rim) * diff;
            #else
            c.rgb = col * diff + rim;
            #endif            
            c.a = s.Alpha;

            return c;
        }
        
        inline void LightingCelShaded_GI(SurfaceOutputStandard s,UnityGIInput data, inout UnityGI gi)
        {
               LightingStandard_GI(s,data, gi);
        }       
        

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
            o.Smoothness = tex2D(_SpecularMap, IN.uv_SpecularMap).x * _Glossiness;
            o.Emission = o.Albedo * _Emission;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
