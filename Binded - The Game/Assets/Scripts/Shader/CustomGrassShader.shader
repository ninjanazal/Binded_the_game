// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Binded/CustomGrassShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1) // cor da erva
        _StartColor("Influencia de cor", Range(0,1)) = 0.5  // influencia inicial da cor
        _ColorTexture("Textura de Cor", 2D) = "white" {}    // textura para atribuir cor ao vertice
        
        // manipulaçao de vertices
        _RandomAmount("Aleatoriedade", Float) = 2   // valor de aleatoriedade
        _GrassBendValue("Curvatura da erva", Range(0,1)) = 0.1  // curvatura da erva
        _GrassWidth("Espeçura da erva", Float) = 0.5    // largura da erva
        _GrassHeight("Altura da erva", Float) = 2       // altura da erva
        _GrassCurvature("Curvatura ao longo da erva", Range(0,1)) = 1 //curvatura da erva
        // clip da relva
        _ClipMaxDistance("Distancia para clip da erva", Float) = 2  // distancia para clip
        _AlphavalueForDistance("Valor de alpha para distancia", Range(0,1)) = 0.5   // distancia de alpha
        // influencia do vento
        _WindTexture("Textura de vento", 2D) = "white" {}   // textura de vento
        _WindForce("Intensidade do vento", Float) = 0   // intensiadade do vento
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200 // vertexLit detail
       
        Pass
        {   
            ZWrite on 
             Blend SrcAlpha OneMinusSrcAlpha 

            //Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            // definiçao de vertex, frag e geometry shaders func
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            // de acordo com a documentaçao, geometry só é possivel a partir da 4
            #pragma target 5.0

            // estruturas
            // entrada
            struct appdata
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 uv       : TEXCOORD0;
            };
            // vertex para geometry
            struct v2g
            {
                float4 vertex       : SV_POSITION;
                float3 normal       : NORMAL;
                float2 uv           : TEXCOORD0;
                float3 vertcolor    : TEXCOORD1;
                float2 windDir      : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            // geometry para frag
            struct g2f{
                float4 vertex       : SV_POSITION;
                float3 normal       : NORMAL;
                float3 vertcolor    : TEXCOORD1;
                float4 screenPos    : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            // subShader vars
            fixed4 _BaseColor;  // cor base 
            sampler2D _ColorTexture;    // textura de cor
            
            fixed _RandomAmount;    // valor de aleatoriedade
            fixed _GrassBendValue;  // valor de curvatura
            fixed _GrassWidth, _GrassHeight;    // altura e largura da erva
            fixed _GrassCurvature;  // curvatura da erva
            half _StartColor;   // influencia inicial da cor

            half _ClipMaxDistance;  // distancia de clip
            half _AlphavalueForDistance;    // alpha da distancia
            
            sampler2D _WindTexture; // textura de vento
            half _WindForce;

            // vertex shader
            v2g vert (appdata v)
            {
                v2g o;
                // transporta os valores do vertice de entrada para o geometry
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = v.uv.xy;
                // atribui valor de cor ao vertice
                // TODO : valor do fragmento de acordo com a textura
                o.vertcolor = tex2Dlod(_ColorTexture, v.uv).rgb;
                
                // valor da displace do vento com base na Textura   
                o.windDir = (float2)(tex2Dlod(_WindTexture, v.uv));
                o.windDir = float2(sin(_Time.x * (o.windDir.x -_WindForce *10)) , cos(_Time.x *( o.windDir.y -_WindForce *10))) * (_WindForce * 0.2);
                // mantem funcionalidade da fog
                UNITY_TRANSFER_FOG(o,UnityObjectToClipPos(v.vertex));
                return o;
            }
            
            // para determinar um valor aleatorio
            float rand(float3 co)
            {
                return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
            }
            // retorna uma matriz de rotaçao de acordo com o angulo em rad e o vector de eixo
            float3x3 AngleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3(
                    t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
                    t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
                    t * x * z - s * y,  t * y * z + s * x,  t * z * z + c);
            }            

            // geometry shader
            [maxvertexcount(11)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> stream)
            {
                // inicia estrutura de saida
                g2f o;

                // posiçao do vertice mae
                float3 vertPos = IN[0].vertex.xyz;
                // normal do vertice
                float3 vertNormal = IN[0].normal;
            
                // matriz de rotaçao para que as ervas apontem para direcçoes aleatoreas
                float3x3 facingRotMatrix = AngleAxis3x3( rand(vertPos) * UNITY_TWO_PI, float3(0,1,0));
                // matriz de bending
                float3x3 bendingMatrix = AngleAxis3x3( rand(vertPos.zzz) * _GrassBendValue * UNITY_PI, float3(-1,0,0));
                // matriz de tranformaçao geral
                float3x3 calculatedeTotalValues = mul(facingRotMatrix, bendingMatrix);   
                
                // influencia aleatoria do flex frontal
                float randForward = rand(vertPos.yyz) * _GrassCurvature;
    
                //altura aleatoria
                float randomHeight = (rand(vertPos.zxy)* 2-1 ) * _RandomAmount + _GrassHeight;
                // largura aleatoria
                float randomWidht = (rand(vertPos.zyy) * 2 - 1) * _RandomAmount + _GrassWidth;

                // for para criaçao dos segmentos da erva
                for(int i = 0; i < 5; i++)
                {
                    // multiplicador para manipulaçao progressiva dos valores relativos ao segmento
                    fixed segmentRatio = (fixed)i / 5;

                    // altura de acordo com o segmento
                    fixed segmWidth = randomWidht * (1 - segmentRatio);
                    // largura de acordo com o segmento
                    fixed segmHeight = randomHeight * segmentRatio;
                    // deslocamento frontal de acordo com o segmento
                    fixed segmLocalBend = clamp(pow(segmentRatio, _GrassBendValue) * randForward,0,1);
                    // cor do segmento
                    fixed segmColor = pow(segmentRatio + _StartColor,2);
                    // Influencia do vento
                    fixed3 segWind = fixed3((IN[0].windDir * segmentRatio).x,0,(IN[0].windDir * segmentRatio).y); 
                    
                    // *************************************
                    // * vertice -> direita                +
                    // *************************************

                    // calculo dos vertices do segmento
                    fixed3 positionToVertex = fixed3(segmWidth + segWind.x, segmHeight, segmLocalBend + segWind.z);
                    // valor da posiçao do vertice 
                    fixed3 localPosition = vertPos + mul(calculatedeTotalValues, positionToVertex);

                    // adiciona os valores determinados
                    o.vertex = UnityObjectToClipPos(localPosition);
                    o.normal = vertNormal;
                    o.vertcolor = IN[0].vertcolor * segmColor;
                    o.screenPos = ComputeScreenPos(o.vertex);

                    UNITY_TRANSFER_FOG(o, o.vertex);
                    // adiciona á stream de saida
                    stream.Append(o);


                    // *************************************
                    // * vertice -> esquerda               +
                    // *************************************

                    // calculo dos vertices do segmento
                    positionToVertex = fixed3(-segmWidth + segWind.x, segmHeight, segmLocalBend + segWind.z);
                    // valor da posiçao do vertice 
                    localPosition = vertPos + mul(calculatedeTotalValues, positionToVertex);

                    // adiciona os valores determinados
                    o.vertex = UnityObjectToClipPos(localPosition);
                    o.normal = vertNormal;
                    o.vertcolor = IN[0].vertcolor * segmColor;
                    o.screenPos = ComputeScreenPos(o.vertex);
                    
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    // adiciona á stream de saida
                    stream.Append(o);
                }

                
                // *************************************
                // * vertice -> Pico                   +
                // *************************************
                fixed3 localTopVec =vertPos + mul(calculatedeTotalValues, fixed3(0 + IN[0].windDir.x,randomHeight, randForward + IN[0].windDir.y));
                o.vertex = UnityObjectToClipPos(localTopVec);
                o.normal = vertNormal;
                o.vertcolor = IN[0].vertcolor * 3;
                o.screenPos = ComputeScreenPos(o.vertex);

                UNITY_TRANSFER_FOG(o, o.vertex);
                // adiciona á stream de saida
                stream.Append(o);
            }
                    
    
            // fragment shader
            fixed4 frag (g2f i) : COLOR
            {
                // atribui a cor base ao fragmento
                fixed4 col = float4(i.vertcolor,1) * _BaseColor;                
                col.w = _BaseColor.w;
                // avaliar o clip
                col.a = smoothstep(_AlphavalueForDistance, 1, clamp(i.screenPos.w /_ClipMaxDistance, 0, 1));
                clip( (col.w < _AlphavalueForDistance * 2 ? -1: 1));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}