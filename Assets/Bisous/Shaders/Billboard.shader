
Shader "Unlit/Billboard"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_ImageResolution ("Image Resolution", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Utils.cginc"

			struct attribute
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct varying
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			float2 _ImageResolution;
			
			varying vert (attribute v)
			{
				varying o;
				o.position = v.position;
				// o.position.xyz = 0.;
				o.position.x *= _ImageResolution.x/_ImageResolution.y;
				o.position = UnityObjectToClipPos(o.position);
				float2 anchor = v.uv*2.-1.;
				anchor.y *= -1.;
				anchor.x *= _ScreenParams.y/_ScreenParams.x;
				// o.position.xy += anchor;
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (varying i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
