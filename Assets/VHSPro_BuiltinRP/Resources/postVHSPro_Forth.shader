// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/postVHSPro_Forth" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}		//current frame
		_FeedbackTex ("Render Input", 2D) = "white" {}	//feedback buffer
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }

		Pass {
			CGPROGRAM

				//#pragma exclude_renderers d3d11 xbox360			
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#include "UnityCG.cginc"
				#pragma glsl

				//properties
				sampler2D _MainTex;
				sampler2D _FeedbackTex;
				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD1;   	
				};

				struct v2f {
			   	float2 uv : TEXCOORD2; // non normilized coordinates -1..1
			   	float4 uvn : TEXCOORD1; //normilized coordicated 0..1				   	
			   	float4 pos : SV_POSITION; //The position of the vertex after being transformed into projection space
				};

				v2f vert (appdata i){
			   	v2f o;
			   	o.pos = UnityObjectToClipPos( i.vertex );
			   	o.uv = o.pos.xy/o.pos.w;
			   	o.uvn = float4( i.texcoord.xy, 0, 0);			   	
			   	return o;
				}
				///// end of initial structures


				//uniforms
				float feedbackAmp = 1.0;


				// // half3 bm_add(half3 a, half3 b){ return a+b; }
				// half3 bm_div(half3 a, half3 b){ return a/b; }
				// half3 bm_sub(half3 a, half3 b){ return a-b; }
				
				// //dark
				// // half3 bm_mul(half3 a, half3 b){ return a*b; }
				// half3 bm_colBurn(half3 a, half3 b){ return 1.0-(1.0-b)/a; }
				// half3 bm_linBurn(half3 a, half3 b){ return a+b-1.0; }

				// //light
				half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }
				// half3 bm_colDodge(half3 a, half3 b){ 	return b/(1.0-a); }
				// half3 bm_add(half3 a, half3 b){ 	return a+b; } //also called linDodge


				//main
				//this shader re-generates feedback only 
				half4 frag( v2f i ) : COLOR {
					
					float2 p = i.uvn;

					half3 col = tex2D( _MainTex, i.uvn).rgb; 		//curent frame
					half3 fbb = tex2D( _FeedbackTex, i.uvn).rgb; //feedback buffer

					
					
					
				   col = bm_screen(col, fbb*feedbackAmp);
				   // col = col+ fbb*feedbackAmp;
				   
				   return half4(col, 1.0); 

				}

			ENDCG
		}
	}
}