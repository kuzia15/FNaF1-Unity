// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/postVHSPro_Tape" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
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
				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   //float4 texcoord2 : TEXCOORD1;   	
				};

				struct v2f {
			   	float4 uvn : TEXCOORD1; //normilized coordicated 0..1
			   	float2 uv : TEXCOORD2; // non normilized coordinates -1..1
			   	float4 pos : SV_POSITION; //The position of the vertex after being transformed into projection space
				};

				v2f vert (appdata i){
				   	v2f o;
				   	o.pos = UnityObjectToClipPos( i.vertex );
				   	o.uv = o.pos.xy/o.pos.w;
				   	o.uvn = float4( i.texcoord.xy, 0.0, 0.0);
				   	return o;
				}

				///// end of initial structures


				//uniforms
				// #pragma multi_compile ___ VHS_TAPENOISE_ON
				#pragma shader_feature VHS_TAPENOISE_ON
				float tapeNoiseTH = 0.7; 
				float tapeNoiseAmount = 1.0; 
				float tapeNoiseSpeed = 1.0; 

				// #pragma multi_compile ___ VHS_FILMGRAIN_ON
				#pragma shader_feature VHS_FILMGRAIN_ON
				float filmGrainAmount = 16.0;
				float filmGrainPower = 10.0;
				
				// #pragma multi_compile ___ VHS_LINENOISE_ON
				#pragma shader_feature VHS_LINENOISE_ON
				float lineNoiseAmount = 1.0; 
				float lineNoiseSpeed = 5.0; 


				


				//SHADER START
				#define PI 3.14159265359

				float time_ = 0.0;

				//Hash without sin coz trigonometry functions loose accuracy different GPUs
				//This set suits the coords of of 0-1 ranges..
				//there are other values https://www.shadertoy.com/view/4djSRW				
				#define MOD3 float3(443.8975,397.2973, 491.1871)

				//this hash for 0..1 values
				float hash12(float2 p){
					float3 p3  = frac(float3(p.xyx) * MOD3);
				    p3 += dot(p3, p3.yzx + 19.19);
				    return frac(p3.x * p3.z * p3.y);
				}

				//this hash for 0..1 values
				float2 hash22(float2 p) {
					float3 p3 = frac(float3(p.xyx) * MOD3);
				   p3 += dot(p3.zxy, p3.yzx+19.19);
				   return frac(float2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
				}


 				//this hash works for big values
 				//gotta use other sin indep hash for big values
				float hash( float n ){ return frac(sin(n)*43758.5453123); }

				// 3d noise function (by iq's)
				float niq( in float3 x ){
				    float3 p = floor(x);
				    float3 f = frac(x);
				    f = f*f*(3.0-2.0*f);
				    float n = p.x + p.y*57.0 + 113.0*p.z;
				    float res = lerp(lerp(	lerp( hash(n+  0.0), hash(n+  1.0),f.x),
				                        	lerp( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y),
				                    	lerp( lerp( hash(n+113.0), hash(n+114.0),f.x),
				                        	lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
				    return res;
				}



				//tape Noise

				//this part only responsible for tape noise lines
				//gotta separate it for later effects 
				float tapeNoiseLines(float2 p, float t){

					//so atm line noise is depending on hash for int values
					//i gotta rewrite to for hash for 0..1 values 
					//then i can use normilized p for generating lines

				   float y = p.y*_ScreenParams.y;
				   float s = t*2.0;
				   return  	(niq( float3(y*0.01 +s, 			1.0, 1.0) ) + 0.0)
				          	*(niq( float3(y*0.011+1000.0+s,	1.0, 1.0) ) + 0.0) 
				          	*(niq( float3(y*0.51+421.0+s, 	1.0, 1.0) ) + 0.0)   
				        ;


				}

				//my tape noise, p here shud be normilized
				float tapeNoise(float nl, float2 p, float t){

				   //TODO custom adjustable density (Probability distribution)
				   // but will be expensive (atm its ok)

				   //atm its just contrast noise 
				   
				   //this generates noise mask
				   float nm = 	hash12( frac(p+t*float2(0.234,0.637)) ) 
				   			  	// *hash12( frac(p+t*float2(0.123,0.867)) ) 
				   			  	// *hash12( frac(p+t*float2(0.441,0.23)) );
									;						
					nm = nm*nm*nm*nm +0.3; //cheap and ok
				 	//nm += 0.3 ; //just bit brighter or just more to threshold?

				   nl*= nm; // put mask
					// nl += 0.3; //Value add .3//

					if(nl<tapeNoiseTH) nl = 0.0; else nl =1.0;  //threshold
				   return nl;
				}



				#if VHS_LINENOISE_ON

					float rnd_rd(float2 co){
					     float a = 12.9898;
					     float b = 78.233;
					     float c = 43758.5453;
					     float dt= dot(co.xy ,float2(a,b));
					     float sn= fmod(dt,3.14);
					    return frac(sin(sn) * c);
					}

					float rndln(float2 p, float t){
						float sample = rnd_rd(float2(1.0,2.0*cos(t))*t*8.0 + p*1.0).x;
						sample *= sample;//*sample;
						return sample;
					}

					float lineNoise(float2 p, float t){
					   
						float n = rndln(p* float2(0.5,1.0) + float2(1.0,3.0), t)*20.0;
						
					   float freq = abs(sin(t));  //1.
						float c = n*smoothstep(fmod(p.y*4.0 + t/2.0+sin(t + sin(t*0.63)),freq), 0.0,0.95);

					   return c;
					}

				#endif



				#if VHS_FILMGRAIN_ON

					// //adjustable but expensive gausian noise
					// //t- time, c - amount 0..1
					// float n4rand( float2 n, float t, float c ) {
					// 	t = frac( t );
					// 	float nrnd0 = hash12( n + 0.07*t );
						
					// 	//float p = 1. / (1. +  8. * iMouse.y  / iResolution.y);
					//     float p = 1. / (9.0*c);
					// 	nrnd0 -= 0.5;
					// 	nrnd0 *= 2.0;
					// 	if(nrnd0<0.0) nrnd0 = pow(1.0+nrnd0, p)*0.5;
					// 	else nrnd0 = 1.0-pow(nrnd0, p)*0.5;
					// 	return nrnd0; 
					// }

					float filmGrain(float2 uv, float t, float c ){ 
						
						// expensive but controllable w c
						// return n4rand(uv,t,c);
						
						//cheap noise - is ok atm
						float nr = hash12( uv + 0.07*frac( t ) );
						return nr*nr*nr;
					}	

				#endif

				



				//MAIN
				half4 frag( v2f i ) : COLOR {

					float t = time_;//_Time.y;					
					float2 p = i.uvn; // normalized tex coordnates 0..1 (gl_FragCoord.xy / iResolution.xy)


					#if UNITY_UV_STARTS_AT_TOP 
						p.y = 1-p.y; 
					#endif

					float2 p_ = p*_ScreenParams;

					float ns = 0.0; //signal noise
					float nt = 0.0; //tape noise
					float nl =0.0; //lines for tape noise
					float ntail =0.0; //tails values for tape noise 

					#if VHS_TAPENOISE_ON

						//here is normilized p (0..1)
						nl = tapeNoiseLines(p, t*tapeNoiseSpeed)*1.0;//tapeNoiseAmount;
						nt = tapeNoise(nl, p, t*tapeNoiseSpeed)*1.0;//tapeNoiseAmount;
						ntail = hash12(p+ float2(0.01,0.02) );

				   #endif

				   #if VHS_LINENOISE_ON
						ns += lineNoise(p_, t*lineNoiseSpeed)*lineNoiseAmount;
						// col += lineNoise(pn_, t*lineNoiseSpeed)*lineNoiseAmount;//i.uvn
				   #endif

			   	//y noise from yiq
				   #if VHS_FILMGRAIN_ON	
				   	//cheap n ok atm						
				   	float bg = filmGrain((p_-0.5*_ScreenParams.xy)*0.5, t, filmGrainPower );
				   	ns += bg * filmGrainAmount;
				   #endif

					return half4(nt,nl,ns,ntail);//half4(col, 1.0); 

				}

			ENDCG
		}
	}
}
