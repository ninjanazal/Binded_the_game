Shader "Binded/CameraDimmerShader"
{
// shader mistura os campos de cor e determina a media, atribuido de volta ao fragmento
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}  // textura de entrada da CameraDimmerShader
        _DimmAmount("Dim Amount", Range(0,1)) = 0   // influencia do efeito
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;   // posiçao dos vertex de entrada
                float2 uv : TEXCOORD0;  // posiçoes de uv de entrada
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;  // texcords para frag
                float4 vertex : SV_POSITION;    // posiçoes em clip
            };

            sampler2D _MainTex; // textura de entrada
            float4 _MainTex_ST;
            
            half _DimmAmount;   // quantidade de efeito
            // vertex
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  // clip positions
                o.uv = v.uv;    // transfere uvs
                return o;
            }
            
            // frag
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = ((col.r + col.g + col.b) / 3) * _DimmAmount + col *(1 - _DimmAmount);
                col2 = 1- step(col.r, 0.5);
                return (col / col2) % _DimmAmount;
                //return col2;
            }
            ENDCG
        }
    }
}
