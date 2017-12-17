Shader "Custom/Outline" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Thin ("Thin", Float) = .1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass {
			Cull Front
			CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      uniform float _Thin;

			struct attribute {
				float4 position : POSITION;
				float3 normal : NORMAL;
			};

			struct varying {
				float4 position : POSITION;
			};

			varying vert (attribute v) {
				varying o;
				o.position = v.position;
				o.position.xyz += v.normal * _Thin;
				o.position = mul(UNITY_MATRIX_M, o.position);
				o.position = mul(UNITY_MATRIX_VP, o.position);
				return o;
			}

			fixed4 frag (varying i) : COLOR {
				return fixed4(0,0,0,0);
			}
			ENDCG
		}
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
