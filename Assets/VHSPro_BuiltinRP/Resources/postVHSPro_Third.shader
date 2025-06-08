// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/postVHSPro_Third" {

	Properties {
		_MainTex ("Render Input", 2D) = "white" {} 		//current frame without feedback
		_LastTex ("Render Input", 2D) = "white" {} 		//latest frame without feedback
		_FeedbackTex ("Render Input", 2D) = "white" {} 	//feedback buffer
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
				sampler2D _LastTex;
				sampler2D _FeedbackTex;				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD1;   	
				   float4 texcoord3 : TEXCOORD2;   					   
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
				float feedbackAmount = 0.0;
				float feedbackFade = 0.0;
				float feedbackThresh = 5.0;
				half3 feedbackColor = half3(1.0,0.5,0.0); //TODO maybe param
				// float time_ = 0.0;
				// int feedbackMode = 0;


				half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }

				//main
				//this shader re-generates feedback only 
				half4 frag( v2f i ) : COLOR {
					
					float2 p = i.uvn;// gl_FragCoord.xy / iResolution.xy;
					float one_x = 1.0/_ScreenParams.x;
					// feedbackColor = half3(1.0,0.5,0.0);

					//new feedback value
					half3 fc =  tex2D( _MainTex, i.uvn).rgb; 		//current frame without feedback
					half3 fl =  tex2D( _LastTex, i.uvn).rgb; 		//last frame without feedback
					float diff = abs(fl.x-fc.x + fl.y-fc.y + fl.z-fc.z)/3.0; //dfference between frames
					if(diff<feedbackThresh) diff = 0.0;

					half3 fbn = fc*diff*feedbackAmount; //feedback new
					// fbn = half3(0.0, 0.0, 0.0);
					

					//old feedback buffer
					half3 fbb = half3(0.0, 0.0, 0.0); 
					// fbb = tex2D( _FeedbackTex, i.uvn).rgb;				
					fbb = ( //blur old bufffer a bit
						tex2D( _FeedbackTex, i.uvn).rgb + 
						tex2D( _FeedbackTex, i.uvn + float2(one_x, 0.0)).rgb + 
						tex2D( _FeedbackTex, i.uvn - float2(one_x, 0.0)).rgb
					) / 3.0;
					fbb *= feedbackFade;

					fbn = bm_screen(fbn, fbb); 
				   return half4(fbn*feedbackColor, 1.0); 

				}

			ENDCG
		}
	}
}
