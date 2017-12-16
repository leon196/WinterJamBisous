// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Unlit/Body"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Head ("Head", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		// Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		// Blend SrcAlpha OneMinusSrcAlpha 
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
				float salt : TEXCOORD2;
			};

			sampler2D _MainTex, _Head;
			fixed4 _Color;
			float _Size;
			float2 _Seed;
			
			varying vert (attribute v)
			{
				varying o;
				float4 position = v.position;
				position.xyz = 0.;
				position = mul(UNITY_MATRIX_M, position);
				// float4 position = v.position;
				o.salt = rand(v.position.xz);

				float2 anchor = v.uv*2.-1.;
				anchor.y *= -1.;

				float salt = rand(_Seed) * PI * 2.;
				float time = _Time.y * 8.+salt;

				// uv.y = 1.-uv.y;
				// uv.x -= .5;
				float breathX = 1. + .2 * cos(time);
				float breathY = 1. + .1 * sin(time);
				anchor.x *= breathX;
				anchor.y *= breathY;
				// uv.x += .5;
				// uv.y = 1.-uv.y;

				float3 view = position-_WorldSpaceCameraPos;
				view.y = 0;
				view = normalize(view);
				float3 right = normalize(cross(view, float3(0,1,0)));
				float3 direction = normalize(cross(view, right));
				// rotation2D(direction.xy, sin(v.position.z/4.+_Time.y));

				float size = _Size * 2.;
				position.xyz += (direction * anchor.y + right * anchor.x) * size;
				position.y -= _Size * 2.5;

				// o.position = UnityObjectToClipPos(position);
				o.position = mul(UNITY_MATRIX_VP, position);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (varying i) : SV_Target
			{

				float2 p = i.uv;
				p -= .5;
				rotation2D(p, sin(p.y*5.+_Time.y*5.)*.2 * (1.-i.uv.y));
				p += .5;

				fixed4 col = tex2D(_MainTex, p);

				float shouldClip = col.a - .5;
				shouldClip -= step(0.,abs(p.x-.5)-.5);
				shouldClip -= step(0.,abs(p.y-.5)-.5);
				clip(shouldClip);
				col.rgb *= p.y*.5+.5;
				return col;
			}
			ENDCG
		}
	}
}
