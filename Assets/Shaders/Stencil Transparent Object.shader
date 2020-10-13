﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Solid Transparency Color" 
{ 
    Properties 
    { 
        _Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5) 
    } 

    SubShader 
        { 
            Tags {"Queue" = "Transparent"} 
			/*Stencil {
				Ref 1
				Comp equal
			}*/

            ZWrite Off GrabPass { } 
            Pass { Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha

         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag

         fixed4 _Color;
         sampler2D _GrabTexture;
         struct appdata
         {
             float4 vertex : POSITION;
         };
         struct v2f
         {
             float4 pos : SV_POSITION;
             float4 uv : TEXCOORD0;
         };
         v2f vert (appdata v)
         {
             v2f o;
             o.pos = UnityObjectToClipPos(v.vertex);
             o.uv = o.pos;
             return o;
         }
         half4 frag(v2f i) : COLOR
         {
             float2 coord = 0.5 + 0.5 * i.uv.xy / i.uv.w;
			 // For Desktop builds
             fixed4 tex = tex2D(_GrabTexture, float2(coord.x, 1 - coord.y));
			 // For WebGL builds
			 //fixed4 tex = tex2D(_GrabTexture, float2(coord.x, coord.y));
             return fixed4(lerp(tex.rgb, _Color.rgb, _Color.a), 1);
         }
         ENDCG
     }
 }
}