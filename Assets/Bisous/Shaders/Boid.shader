Shader "Unlit/Boid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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

			struct attribute
			{
				float4 position : POSITION;
				float2 anchor : TEXCOORD0;
				float4 indexMap : TEXCOORD1;
			};

			struct varying
			{
				float4 position : SV_POSITION;
				float2 anchor : TEXCOORD0;
			};

			sampler2D _MainTex, _BoidBuffer;
			float4 _MainTex_ST;
			
			varying vert (attribute v)
			{
				varying o;
				o.position = mul(UNITY_MATRIX_M, float4(0,0,0,1));
				o.position.xz = tex2Dlod(_BoidBuffer, v.indexMap).xy;
				o.position = mul(UNITY_MATRIX_VP, o.position);
				float2 anchor = v.anchor;
				anchor.x *= _ScreenParams.y/_ScreenParams.x;
				o.position.xy += anchor * .2;
				o.anchor = v.anchor;
				return o;
			}
			
			fixed4 frag (varying i) : SV_Target
			{
				float2 uv = i.anchor*.5+.5;
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
