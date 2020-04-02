Shader "Binded/VoronoiShader"
{
    Properties
    {
        _BaseColor("Cor",Color) = (1,1,1,1)
        _AngleOff("Angle offset", Float) = 1
        _CellDensity("Densidade", Float) = 1
        _Amount("Amount", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { }
        LOD 100
        GrabPass{}
        Cull Off ZWrite Off

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

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // variaveis
            fixed4 _BaseColor;
            fixed _AngleOff, _CellDensity,_Amount;

            sampler2D _GrabTexture;


            // psedoRandom
            inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
            {
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                UV = frac(sin(mul(UV, m)) * 46839.32);
                return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
            }
            
            // Voronoi 
            // funçao copiada a partir do node do shader graph
            void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                float t = 8.0;
                float3 res = float3(8.0, 0.0, 0.0);

                for(int y=-1; y<=1; y++)
                {
                    for(int x=-1; x<=1; x++)
                    {
                        float2 lattice = float2(x,y);
                        float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                        float d = distance(lattice + offset, f);
                        if(d < res.x)
                        {
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
                
                return col % grabColor.x;   // divisao de resto sobre a cor de parametro e a cor de grabPass
                //return voronoiDisplace.x;
            }
            ENDCG
        }
    }
}
