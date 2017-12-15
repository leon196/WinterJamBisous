
Shader "Unlit/Crop"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Utils.cginc"

			sampler2D _MainTex;
			float2 _ImageResolution;
			float4 _Face;
			
			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = i.uv;
				uv.y = 1.-uv.y;
				fixed4 col = tex2D(_MainTex, uv);
				float2 size = _Face.zw/_ImageResolution;
				float2 center = _Face.xy/_ImageResolution + size/2.;
				col *= step(abs(center.x-i.uv.x), size.x/2.);
				col *= step(abs(center.y-i.uv.y), size.y/2.);
				return col;
			}
			ENDCG
		}
	}
}
