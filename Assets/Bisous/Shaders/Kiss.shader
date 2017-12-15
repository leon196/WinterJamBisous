
Shader "Unlit/Kiss"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_ImageResolution ("Image Resolution", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha 
		ZWrite Off ZTest Always
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
			float2 _MousePosition;
			float _MouseClick;
			
			varying vert (attribute v)
			{
				varying o;
				o.position = UnityObjectToClipPos(float4(0,0,0,1));
				float2 anchor = v.uv;
				anchor = anchor*2.-1.;
				anchor *= (1.-_MouseClick);
				anchor.x *= _ScreenParams.y/_ScreenParams.x;
				anchor.y *= -1.;
				float2 mouse = _MousePosition/_ScreenParams.xy;
				mouse = mouse * 2. - 1.;
				mouse.y *= -1.;
				o.position.xy = anchor * 2. + length(_WorldSpaceCameraPos)*mouse;
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
