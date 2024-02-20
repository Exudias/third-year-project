Shader "Custom/SemiTransparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        LOD 100

        Pass
        {
            Stencil {
                Ref 0
                Comp Equal
                Pass IncrSat
                Fail IncrSat
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color * i.color;
                return col;
            }
            ENDCG
        }
    }
}