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
        [Toggle(SHADOWED)]
        _ShadowedRim("Rim affected by shadow", float) = 0       // toogle se a rim deve ser afectada pelas sombras
        
        [Header(Emission)]
        [HDR]_Emission("Emission", Color) = (0,0,0,1)           // cor de emissao do objecto

        [Header(OutLine)]
        _OutlineColor("Outline Color", Color) =(0,0,0,0)        // Cor de outLine
        _OutlineWidth("Outline Width", Range(0,0.01)) = 0.003   // Largura de outline
        _OutlineClipDistance("Clip Distance", float) = 1        // distancia para o clip do outline

    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque" 
            "LightMode" = "ForwardBase"
        }
        LOD 200
        
        Cull Back

        // stencil compara todos os pixel com o valor de referencia
        // como no buffer de stencil de entrada os valores sao sempre 0
        // substitui todos os valores dos pixeis renderizados para 200
        Stencil
        {
            Ref 200
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        // definiçao do surface, iluminaçao personalizada, full shadows
        #pragma surface surf CelShaded fullforwardshadows 
        // feature
        #pragma shader_feature SHADOWED
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
        };
        
        
        // custom avaliaçao de luzes, tem em atençao á atribuiçao de valores calculador atraves da global ilumination
        inline half4 LightingCelShaded (SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {            
            // produto escalar saturado, entre a normal do pixel com a direcçao da luz
            half nDotL = saturate(dot(s.Normal, normalize(gi.light.dir)));
            //determina o valor de difusao de acordo com a quantidade de bandas e o tamanho do cutoff
            half diff = round(saturate(nDotL/ _LightCutoff) * _ShadowBands) / _ShadowBands;
            
            // reflete o vetor da direcçao da luz de acordo com a normal do pixel
            float3 refl = reflect(normalize(gi.light.dir), s.Normal);
            // dado o vetor de reflecçao determinado, invertese a direçao e o produto escalar entre este e a 
            // view direction da camera retorna o produto escalar da reflecçao
            float vDotRefl = dot(viewDir, -refl);
            // para determinar o valor especular, com a cor passada para o especular e multiplica pelo steo do valor entre
            // se o valor do produto escalar da reflecçao for maior que o valor do smoth do obj (retorna 1)
            float3 specular = _SpecularColor.rgb * step(1 - s.Smoothness, vDotRefl);
            
            // determinaçao da cor de outline/ rim no objecto
            //step entre o valor de rimSize como 0->1 e o valor saturado do produto escalar entre a direcçao de vista
            // com a normal do pixel
            float3 rim = _RimColor * step(1 - _RimSize ,1 - saturate(dot(normalize(viewDir), s.Normal)));
            // determianr a cor base do objecto com luz é determinado a partir da soma do albedo e o valor calculado
            // esoecular multiplicado a totalidade pela cor da luz
            half3 col = (s.Albedo + specular) * gi.light.color;
            // cor de saida
            half4 c;
            // caso esteja activado a opçao de atribuir á cor de rim o valor da sombra
            #ifdef SHADOWED
                // cor de saida é determinada pela soma da cor determinada anteriormente com o valor de Rim 
                // multiplicado pelo valor de diff, ou seja a atenuaçao da cor com base na projecçao da sombra
                c.rgb = (col + rim) * diff;
            #else
                // caso contrario, a cor do objecto é a cor base com a luz em conta multiplicada pelo valor de atenuaçao da cor
                // somando o valor de rim
                c.rgb = col * diff + rim;
            #endif
            // o valor de alpha é preservado do valor de entrada            
            c.a = s.Alpha;

            return c;
        }
        
        // para ter acesso a variaveis da iluminaçao global, é necessario integrar uma funçao que exporte na var de UnityGI,
        // os valores gerados fora de runTime
        inline void LightingCelShaded_GI(SurfaceOutputStandard s,UnityGIInput data, inout UnityGI gi)
        {   
            // camha com base na iluminaçao generalizada passando os valor de Surface, input e global ilumination
            LightingStandard_GI(s,data, gi);
        }       
        
        // permite instanciaçao de parametros na gpu
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        // surface shader
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // projecçao da textura de albedo com base na cor da atribuida por parametro
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // atribui a cor gerada
            o.Albedo = c.rgb;
            // guarda na normal a textura passada (Caso exista, dependendo dos modelos enconrtados, é mantida just in case)
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
            // da mesma forma que o valor de normais passadas por textura, o mesmo se aplica para a smoothness
            o.Smoothness = tex2D(_SpecularMap, IN.uv_SpecularMap).x * _Glossiness;
            // o valor de emissao é multiplica ao albedo, forma de atenuar o valor das sombras por parametro
            o.Emission = o.Albedo * _Emission;
            // alpha é preservado a partir da textura de albedo
            o.Alpha = c.a;
        }

        
        ENDCG
        
        // segundo pass para desenhar á volta do objecto
        // efeito de outline
        Pass{
            // imprime apenas a faces nao voltadas para a camera
            Cull Front
            
            // utilizando o stencil
            Stencil 
            {
                // passa pixeis que tenham em buffer stencil menor ou igual
                Ref 200
                Comp GEqual
            }
            
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            
            // definida no segundo pass o valor da largura e cor da linha de outLine
            float _OutlineWidth;
            float4 _OutlineColor;
            float _OutlineClipDistance;

            // vertex to frag struct
            struct v2f{
                float4 clipPos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            // vertex
            // inicia com a posiçao do pixel e a normal
            v2f vert(float4 position : POSITION , float3 normal : NORMAL) {
                // inicia a saida do vert
                v2f o;
                // transforma a posiçao do objecto no espeço de clip
                float4 clipPosition = UnityObjectToClipPos(position);
                // transforma a normal do objecto para espaço de clip,nao é usado a call do metodo 
                // anterior pois nao estava a ocorrer resultados iguais
                float3 clipNormal = mul((float3x3)UNITY_MATRIX_MVP, normal);
                // o offset para a deslocaçao dos vertices é determinado pela normalizaçao das normais em clip
                // pela largura da linha e pelo valor em w das mesmas
                float2 offset = normalize(clipNormal.xy) * _OutlineWidth * clipPosition.w;
                // desloca o vertice de acordo com a direcçao normal do vertice no clip multiplicado pela largura
                clipPosition.xyz += normalize(clipNormal) * _OutlineWidth;
                // é adicionado o offset definido 
                clipPosition.xy += offset;
                // retornado para a estrutura de saia a posiçao do mesmo
                o.clipPos = clipPosition;
                
                // guarda na estrutura de saida as posiçoes  
                o.screenPos = ComputeScreenPos(clipPosition);
                return o;
            }

            // frag
            half4 frag(v2f i) : SV_TARGET{
                // cliping da outline se a distancia do objecto á vista for inferior do valor definido
                clip( (i.screenPos.w < _OutlineClipDistance)? -1 : 1 );
                // retorna a cor do outline
                return _OutlineColor;
            }
            ENDCG
        }        
    }
    FallBack "Diffuse"
}
