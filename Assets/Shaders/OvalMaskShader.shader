Shader "Custom/OvalMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Aspect ("Texture Aspect (w/h)", Float) = 1.0
        _MaskScale ("Mask Scale", Float) = 1.0
        _OvalRatio ("Oval W/H (0.8 = 80%)", Float) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Aspect;
            float _MaskScale;
            float _OvalRatio;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Coordenadas centradas en (0,0)
                float2 centerUV = i.uv - 0.5;

                // Compensar la relación de aspecto de la textura para mantener la forma del óvalo (sin deformarlo).
                float2 compensated = centerUV;
                compensated.x *= _Aspect;

                // Radios base del óvalo con proporción fija: width = _OvalRatio * height
                // baseRadius toma la mitad del espacio UV disponible en la dimensión "alto"
                float baseRadiusY = 0.5; // ry antes de escala
                float baseRadiusX = baseRadiusY * _OvalRatio; // rx antes de escala

                // Aplicar escala proporcional de máscara (misma escala para ambos ejes)
                float rx = baseRadiusX * saturate(_MaskScale);
                float ry = baseRadiusY * saturate(_MaskScale);

                // Ecuación de la elipse (compensada en X)
                float ellipse = (compensated.x * compensated.x) / (rx * rx) + (compensated.y * compensated.y) / (ry * ry);

                if (ellipse > 1.0)
                {
                    // Fuera de la máscara: completamente transparente
                    return fixed4(0,0,0,0);
                }

                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                return mainTexColor;
            }
            ENDCG
        }
    }
}