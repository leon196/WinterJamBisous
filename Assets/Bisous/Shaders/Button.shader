﻿Shader "Unlit/Button"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha 
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Utils.cginc"

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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 uv = i.uv;
				
				/*if(uv.y <= 0.2 || uv.y >= 0.8 || uv.x <= 0.2 || uv.x >= 0.8){
					uv.x += (rand(uv)*2-1)*0.7;	
					uv.y += (rand(uv)*2-1)*0.5;	
				}*/
				
				fixed4 col = tex2D(_MainTex, i.uv);

				/*if(uv.y <= 0.2 || uv.y >= 0.8 || uv.x <= 0.2 || uv.x >= 0.8){
					col.rgb = 1;
				}*/
				
				//col.rgb *= rand(i.uv);
				//col.rgb = uv.xyx;
				return col;
			}
			ENDCG
		}
	}
}
