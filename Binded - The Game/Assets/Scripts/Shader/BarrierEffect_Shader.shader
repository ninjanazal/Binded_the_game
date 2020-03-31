// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Binded/BarrierEffect_Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)     //cor base do objecto   
        

        _ClipNoise("Textura de Cull", 2D) = "white" {}  // textura de noise para o cull da barreira
        _ClipAmount("Tamanho de clip", Range(0,1)) = 1   // valor do clip base
        _EdgeSize("Tamanho da borda", Range(0,0.4)) = 1 // tamanho da borda de Cull
        [HDR]_EdgeColor("Cor da borda", Color) = (1,1,1,1)   // cor da borda

        _ClipFloatuation("Flutuaçao do clip", Range(0,0.5)) = 0.1   // flotuaçao do valor

        _XOffsetValue("Offset em x", Float) = 0.1   // valor de offset em x
        _YOffsetValue("Offset em y", Float) = 0.1   // valor de offset em y
    }
    SubShader
    {
        //Blend SrcAlpha OneMinusSrcAlpha
        //Cull Back
        //ZWrite On
        
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Overlay"
            "ForceNoShadowCasting" = "True"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade noambient 
        #pragma target 4.0
        
        // cor
        fixed4 _Color;  // cor base do objecto

        // Clip
        sampler2D _ClipNoise;   // textura de noise para o cull
        fixed _ClipAmount;  // quantidade de clip
        fixed _EdgeSize;     // largura da borda
        fixed4 _EdgeColor;  // Cor da borda
        
        float _ClipFloatuation; // flotuaçao do clip

        fixed _XOffsetValue, _YOffsetValue;
        
        // Input data
        struct Input
        {
            //float3 viewDir; // direcçao da vista
            float2 uv_ClipNoise;    // uv para a textura de nosie            
        };
        

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
            
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 col = _Color;
            fixed calculatedAmount = _ClipAmount + (abs(sin(_Time * 50)) * _ClipFloatuation);            

            // determina o valor de clip par o fragmento
            half clip_val = 1- tex2D(_ClipNoise , IN.uv_ClipNoise + fixed2(_Time.x * _XOffsetValue , -_Time.x * _YOffsetValue)).x;

            // calcula se o prixel representa a borda do efeito
            col.xyz = (step(clip_val - calculatedAmount , _EdgeSize)>0) ?  _EdgeColor.xyz : _Color.xyz;
            
            // discarta os fragmentos em que o valor da textura no ponto é menor que a quantidade
            // de clip
            clip(clip_val - calculatedAmount);

            // valores de saida
            o.Emission =  col.rgb ;    // atribui o valor da cor            
            o.Alpha = col.w;   // atribui o valor de alpha           
        }
        ENDCG
    }
    FallBack "Diffuse"
}