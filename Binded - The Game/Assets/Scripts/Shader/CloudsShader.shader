Shader "Binded/CloudsShader"
{
    Properties
    {
        [HDR]_BaseColor("Cor de base", Color) = (1,1,1,1)           // cor Base
        _NoiseTex("Textura de Noise", 2D) ="white" {}               // imagem de noise
        _NoiseThreshold("Threshold do noise", Range(0,1)) = 0.5     // threshold do noise
        _DisplaceAmount("Amount displace", float) = 2               // valor de displace
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        
        Tags { "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            //Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // input data
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            // vertex para frag
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // declaraçao de varss
            fixed4 _BaseColor;  // cor base


            sampler2D _NoiseTex;   // textura de noise
            float4 _NoiseTex_ST; // float para uvs da textura principal
            float _NoiseTex_Texel;

            fixed _NoiseThreshold;  // threshold do noise
            float _DisplaceAmount;

            float RemapVal (float val, float from1, float to1, float from2, float to2)
            {
                return ((val - from1) / (to1 -from1) * (to2 - from2) + from2);
            }

            v2f vert (appdata v)
            {
                v2f o;

                // transiçao de vert de entrada para clip
                o.vertex = UnityObjectToClipPos(v.vertex);
                // definiçao das uvs com base no objecto e textura
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                float3 clipNormal = UnityObjectToClipPos(v.normal);
                float remapDisplace = RemapVal(_SinTime.x,-1,1,1,50) +_DisplaceAmount;

                o.vertex.xyz +=
                normalize(clipNormal) * tex2Dlod(_NoiseTex,float4(v.normal.xy,0,0)) * remapDisplace;

                o.uv.x += _SinTime.x*0.2; 
                o.uv.y += _Time.y * 0.1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 noiseTexVal = tex2D(_NoiseTex, i.uv);

                fixed4 col = _BaseColor;
                float remapSin = RemapVal(_SinTime.w,-1,1,0 ,0.1);
                col.w = smoothstep(1,(noiseTexVal.x > _NoiseThreshold + remapSin) ? 0.001 : 1 , .1);
                if(col.w != 0){col.w = _BaseColor.w;}
                
                return col;
            }
            ENDCG
        }
    }
}
