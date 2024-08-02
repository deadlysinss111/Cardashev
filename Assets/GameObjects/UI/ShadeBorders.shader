Shader "UI/ShadeBorders"
{
    Properties
    {
        _Color ("color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always


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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // o.uv = v.uv;
                return o;
            }

            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 outlineColor = lerp(_Color, (1, 1, 1, 1), 0.6);
                // fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col = (1, 1, 1, 1);
                if(i.uv.x < 0.5f){
                    col = col * i.uv.x * 1000;
                    }
                else{
                    col = col * (1 - i.uv.x)*1000;
                    }
                if(i.uv.y < 0.5f){
                    col = col * i.uv.y/2 ;
                    }
                else{
                    col = col * (1 - i.uv.y)/2;
                    }
                //col.rgb = 1 - col.rgb;
                if(col.r > 1) { col.r = _Color.r;}else{col.r = lerp(outlineColor.r, _Color.r, col.r);}
                if(col.g > 1) { col.g = _Color.g;}else{col.g = lerp(outlineColor.g, _Color.g, col.g);}
                if(col.b > 1) { col.b = _Color.b;}else{col.b = lerp(outlineColor.b, _Color.b, col.b);}
                return col;
            }
            ENDCG
        }
    }
}
