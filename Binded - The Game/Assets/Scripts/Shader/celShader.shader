Shader "Binded/celShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)                     // cor base do modelo
        _MainTex ("Albedo (RGB)", 2D) = "white" {}              // Textura de Albedo
        [Normal]_Normal("Normal", 2D) = "bump" {}               // Mapa de normais
        _LightCutoff("Light cutoff", Range(0,1)) = 0.5          // Cut off da luz
        _ShadowBands("Shadow bands", Range(1,4)) = 1            // numero de bandas para a luz
        
        [Header(Specular)]
        _SpecularMap("Specular map", 2D) = "white" {}           // Mapa especular
        _Glossiness ("Smoothness", Range(0,1)) = 0.5            // valor de especular
        [HDR]_SpecularColor("Specular color", Color) = (0,0,0,1)    // cor do especular
        
        [Header(Rim)]
        _RimSize("Rim size", Range(0,1)) = 0                    // linha de rim, largura
        [HDR]_RimColor("Rim color", Color) = (0,0,0,1)          // cor de rim
        [Toggle(SHADOWED_RIM)]
        _ShadowedRim("Rim affected by shadow", float) = 0       // toogle se a rim deve ser afectada pelas sombras
        
        [Header(Emission)]
        [HDR]_Emission("Emission", Color) = (0,0,0,1)           // cor de emissao do objecto

        [Header(OutLine)]
        _OutlineColor("Outline Color", Color) =(0,0,0,0)
        _OutlineWidth("Outline Width", Range(0,0.01)) = 0.003

    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque" 
            "LightMode" = "ForwardBase"
            "PassFlags" = "OnlyDirectional"
        }
        LOD 200
        
        Cull Back

        // stencil compara todos os pixel com o valor de referencia
        // como no buffer de stencil de entrada os valores sao sempre 0
        // substitui todos os valores dos pixeis renderizados para 1
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        // definiçao do surface, iluminaçao personalizada, full shadows
        #pragma surface surf CelShaded fullforwardshadows 
        // feature
        #pragma shader_feature SHADOWED_RIM
        #pragma shader_feature ENABLE_OUTLINE
        #pragma target 5.0

        // lib da iluminaçao global
        #include "UnityPBSLighting.cginc"

        // definiçao das vars subShader
        fixed4 _Color;          // cor base do objecto
        sampler2D _MainTex;     // textura de albedo
        sampler2D _Normal;      // textura de normais
        float _LightCutoff;     // valor de cutOff para as bandas
        float _ShadowBands;     // numero de bandas
        
        sampler2D _SpecularMap; // imagem de mapa especular
        half _Glossiness;       // valor especular
        fixed4 _SpecularColor;  // cor especular
        
        float _RimSize;         // largura de rim
        fixed4 _RimColor;       // cor de rim
        
        fixed4 _Emission;       // cor de emissao


        // estrutura de entrada
        struct Input
        {
            float2 uv_MainTex;  // uvs para a textura de albedo
            float2 uv_Normal;   // uvs para a textura de normais
            float2 uv_SpecularMap;  // uvs para a textura especular

            float4 screenPos;
        };
        
        
        // custom avaliaçao de luzes, tem em atençao á atribuiçao de valores calculador atraves da global ilumination
        inline half4 LightingCelShaded (SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {            
            // produto escalar saturado 
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
        
        // num pass final
        Pass{
            // imprime apenas a faces nao voltadas para a camera
            Cull Front

            Stencil 
            {
                Ref 1
                Comp Greater
            }

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            float _OutlineWidth;
            float4 _OutlineColor;
            
            struct v2f{
                float4 clipPos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            // vertex
            v2f vert(float4 position : POSITION , float3 normal : NORMAL) {
                v2f o;

                float4 clipPosition = UnityObjectToClipPos(position);
                float3 clipNormal = mul((float3x3)UNITY_MATRIX_MVP, normal);
                float2 offset = normalize(clipNormal.xy) * _OutlineWidth * clipPosition.w;

                clipPosition.xyz += normalize(clipNormal) * _OutlineWidth;
                clipPosition.xy += offset;
                o.clipPos = clipPosition;

                o.screenPos = ComputeScreenPos(position);
                return o;
            }

            // frag
            half4 frag(v2f i) : SV_TARGET{
                return _OutlineColor;
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
}
