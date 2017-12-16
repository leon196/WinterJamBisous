Shader "Hidden/Debug"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex, _BoidBuffer, _InfoBuffer;

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 col = tex2D(_InfoBuffer, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
