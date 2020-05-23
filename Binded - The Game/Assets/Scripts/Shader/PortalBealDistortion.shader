/// Binded Voronoi, transcrito a partir de 
/// https://www.shadertoy.com/view/MslGD8
Shader "Binded/VoronoiShader"
{
    Properties
    {
        _BaseColor("Cor",Color) = (1,1,1,1)     // cor base do efeito
        _AngleOff("Angle offset", Float) = 1    // angulo de cutOff
        _CellDensity("Densidade", Float) = 1    // valor da densidade
        _Amount("Amount", Range(0,1)) = 0       // valor da quantidade de influencia do efeito
    }
    SubShader
    {
        // tag de transparencia acrescida, garante que ocorre apos maioria dos renderes transparentes
        Tags { "Queue"="Transparent" }      
        LOD 100
        GrabPass{}  // grabpass para a textura da camara atual
        Cull Off    // cull off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            // estrutura de entrada
            struct appdata
            {
                float4 vertex : POSITION;   // posiçao dos vertices em obj space
                float2 uv : TEXCOORD0;      // coordenadas da textura
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;      // passa coordenadas de textura
                float4 vertex : SV_POSITION;    // posiçoes transformadas
            };

            // variaveis
            // declaraçao
            fixed4 _BaseColor;  // cor base
            fixed _AngleOff, _CellDensity,_Amount;  // valores de angulo densidade e quantidade

            sampler2D _GrabTexture; // grab Texture


            // psedoRandom
            inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
            {
                // um metodo generalista para determinar um valor aleatorio dado 3 componentes
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                UV = frac(sin(mul(UV, m)) * 46839.32);
                return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
            }
            
            // Voronoi 
            // funçao copiada a partir do node do shader graph
            void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                // distingue a parte inteira da parte fracionaria da uv multiplicada pela dencidade, isto resulta num controlo da repetiçao das uvs
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                // vector para guardar valores de saida
                float3 res = float3(8.0, 0.0, 0.0);
            
                // para cada pixel avalia valores á esquerda e direita, cima e baixo
                for(int y=-1; y<=1; y++)
                {
                    for(int x=-1; x<=1; x++)
                    {
                        float2 lattice = float2(x,y);   // vector com o valor a que pixel está a ser avaliado
                        float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset); // retorna random influenciado pelo valor de uv
                        // determina a distancia do valor correspondente á uv em vista somado o aleatorio, até á componente fracional da uv
                        float d = distance(lattice + offset, f);
                        if(d < res.x)   // avalia se a distancia resultante é menor que o valor definido
                        {
                            // se sim atribui valores e retorna os componentes
                            res = float3(d, offset.x, offset.y);
                            Out = res.x;
                            Cells = res.y;
                        }
                    }
                }
            }

            // vertex
            v2f vert (appdata v)
            {
                v2f o;
                // vertices transformados para clip space
                o.vertex = UnityObjectToClipPos(v.vertex);
                // metodo para correctamente obter a posiçao das uvs da grabPass
                o.uv = ComputeGrabScreenPos(o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // guarda na cor de saida a definida por parametro
                fixed4 col = _BaseColor;
                // cria um vector para o displace do voronoi
                float4 voronoiDisplace = float4(0,0,0,0);
                // chama o metodo de voronoi, com valores de saida x e y da var anterior
                Unity_Voronoi_float(i.uv.xy, _AngleOff * _SinTime.y, _CellDensity, voronoiDisplace.x,voronoiDisplace.y);                
                // pega no valor da cor de frag na textura de grab, influenciado pelo voronoi e a intensidade
                fixed4 grabColor = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uv + (voronoiDisplace.x * _Amount)));
                
                return (col % grabColor.x) * grabColor.z;   // divisao de resto sobre a cor de parametro e a cor de grabPass
                //return col % voronoiDisplace.y;
                //return voronoiDisplace.x;
            }
            ENDCG
        }
    }
}
