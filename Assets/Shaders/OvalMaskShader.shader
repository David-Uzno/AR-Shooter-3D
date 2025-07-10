Shader "Custom/OvalMaskShader"
{
    Properties
    {
        _MaskWidth("Mask Width", Float) = 0.75
        _MaskHeight("Mask Height", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MaskWidth;
            float _MaskHeight;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 normalizedUV = float2((i.uv.x - 0.5) / _MaskWidth, (i.uv.y - 0.5) / _MaskHeight);
                float dist = length(normalizedUV);
                if (dist > 0.5) discard;
                return col;
            }
            ENDCG
        }
    }
}