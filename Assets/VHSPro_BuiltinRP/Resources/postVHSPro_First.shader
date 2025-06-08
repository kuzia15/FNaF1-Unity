// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/postVHSPro_First" {
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
				sampler2D _TapeTex;
				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD3;   	
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

				//[CRT]				
				float screenLinesNum = 240.0;
				
				//for fisheye cutoff
				// #pragma multi_compile ___ VHS_FISHEYE_ON
				#pragma shader_feature VHS_FISHEYE_ON
				//TODO maybe remove params if define is off?
	   		half cutoffX = 2.0;
	   		half cutoffY = 3.0;			   		
	   		half cutoffFadeX = 100.0;
	   		half cutoffFadeY = 100.0;


				//[Noise]
				float noiseLinesNum = 240.0; //cud be separate from global
				float noiseQuantizeX = 1.0; //0..1

				// #pragma multi_compile ___ VHS_FILMGRAIN_ON
				// #pragma multi_compile ___ VHS_LINENOISE_ON
				// #pragma multi_compile ___ VHS_TAPENOISE_ON
				#pragma shader_feature VHS_FILMGRAIN_ON
				#pragma shader_feature VHS_LINENOISE_ON
				#pragma shader_feature VHS_TAPENOISE_ON
				float tapeNoiseAmount = 1.0;

				// #pragma multi_compile ___ VHS_YIQNOISE_ON
				#pragma shader_feature VHS_YIQNOISE_ON
				float signalNoisePower = 1.0f;
				float signalNoiseAmount = 1.0f;


				//[twitch]
				// #pragma multi_compile ___ VHS_LINESFLOAT_ON
				#pragma shader_feature VHS_LINESFLOAT_ON
				float linesFloatSpeed = 1.0;

				// #pragma multi_compile ___ VHS_SCANLINES_ON
				#pragma shader_feature VHS_SCANLINES_ON
				float scanLineWidth = 10.0;
				
				// #pragma multi_compile ___ VHS_STRETCH_ON
				#pragma shader_feature VHS_STRETCH_ON

				// #pragma multi_compile ___ VHS_TWITCH_H_ON
				#pragma shader_feature VHS_TWITCH_H_ON
				float twitchHFreq = 1.0;

				// #pragma multi_compile ___ VHS_TWITCH_V_ON
				#pragma shader_feature VHS_TWITCH_V_ON
				float twitchVFreq = 1.0; 

				//[jitter]
				// #pragma multi_compile ___ VHS_JITTER_H_ON
				#pragma shader_feature VHS_JITTER_H_ON
				float jitterHAmount = 0.5; //default .5-1.

				// #pragma multi_compile ___ VHS_JITTER_V_ON
				#pragma shader_feature VHS_JITTER_V_ON
				float jitterVAmount = 1.0; 
				float jitterVSpeed = 1.0;
				


				//SHADER START
				#define PI 3.14159265359

				float time_ = 0.0;

				//additional properties
				float SLN = 0.0; //screen lines number
				float SLN_Noise = 0.0; //screen lines number for noise
				float ONE_X = 0.0; // 1/width
				float ONE_Y = 0.0; // 1/height


				//TOOLS N NOISE FNs

				//blend mode screen
				half3 bms(half3 c1, half3 c2){ return 1.0- (1.0-c1)*(1.0-c2); }

				//turns sth on and off //a - how often 
				float onOff(float a, float b, float c, float t){
					return step(c, sin(t + a*cos(t*b)));
				}


				#if VHS_YIQNOISE_ON

					#define MOD3 float3(443.8975,397.2973, 491.1871)

					//this hash for 0..1 values
					float2 hash22(float2 p) {
						float3 p3 = frac(float3(p.xyx) * MOD3);
					   p3 += dot(p3.zxy, p3.yzx+19.19);
					   return frac(float2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
					}

					//different behavior - for multiplication for example 
					float2 n4rand_bw( float2 p, float t, float c ){
					    
						t = frac( t );//that's why its sort of twitching 
						float2 nrnd0 = hash22( p + 0.07*t );
					   c = 1.0 / (10.0*c); //iMouse.y  / iResolution.y
					   nrnd0 = pow(nrnd0, c);				    
						return nrnd0; //TODO try to invert 1-...
					}

				#endif

				#if VHS_JITTER_V_ON

					float rnd_rd(float2 co){
					     float a = 12.9898;
					     float b = 78.233;
					     float c = 43758.5453;
					     float dt= dot(co.xy ,float2(a,b));
					     float sn= fmod(dt,3.14);
					    return frac(sin(sn) * c);
					}

				#endif


				//DANG WINDOWS
				half3 rgb2yiq(half3 c){   
					return half3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
					);
				};

				half3 yiq2rgb(half3 c){				
					return half3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
					);
				};



				//EFFECT FNs

				#if VHS_SCANLINES_ON

				   //lines 2 floating down				   
	    			float scanLines(float2 p, float t){
			    		
			    		//cheap (maybe make an option later)
			        	// float scanLineWidth = 0.26;
			        	// float scans = 0.5*(cos((p.y*screenLinesNum+t+.5)*2.0*PI) + 1.0);
			        	// if(scans>scanLineWidth) scans = 1.; else scans = 0.;			        	

				   	float t_sl = 0.0;					   	
				   	//if lines aren't floating -> scanlines also shudn't 
				   	#if VHS_LINESFLOAT_ON
				   		t_sl = t*linesFloatSpeed;
				   	#endif
			        	
			        	//expensive but better			        	
			        	float scans = 0.5*(cos( (p.y*screenLinesNum+t_sl)*2.0*PI) + 1.0);
			        	scans = pow(scans, scanLineWidth); 
			        	scans = 1.0 - scans;
			        	return scans; 
	    			}				

    			#endif


    			#if VHS_STRETCH_ON

					float gcos(float2 uv, float s, float p){
					    return (cos( uv.y * PI * 2.0 * s + p)+1.0)*0.5;
					}

					//mw - maximum width
					//wcs = widthChangeSpeed
					//lfs = line float speed = .5
					//lf phase = line float phase = .0
					float2 stretch(float2 uv, float t, float mw, float wcs, float lfs, float lfp){	
					    
					   //width change
					   float tt = t*wcs; //widthChangeSpeed
					   float t2 = tt-fmod(tt, 0.5);
					   
					   //float dw  = hash42( vec2(0.01, t2) ).x ; //on t and not on y
					   float w = gcos(uv, 2.0*(1.0-frac(t2)), PI-t2) * clamp( gcos(uv, frac(t2), t2) , 0.5, 1.0);
					   //w = clamp(w,0.,1.);
					   w = floor(w*mw)/mw;
					   w *= mw;
						//get descreete line number
					   float ln = (1.0-frac(t*lfs + lfp)) *screenLinesNum; 
					   ln = ln - frac(ln); 
					   // float ln = (1.-fmod(t*lfs + lfp, 1.))*SLN; 
					   // ln = ln - fmod(ln, 1.); //descreete line number
					   

					   //ln = 10.;
					   //w = 4.;
					   
					   //////stretching part///////
					   
					   float oy = 1.0/SLN; //TODO global
					  	float md = fmod(ln, w); //shift within the wide line 0..width
					   float sh2 =  1.0-md/w; // shift 0..1

					   // #if VHS_LINESFLOAT_ON
					   // 	float sh = fmod(t, 1.);				   	
					   //  	uv.y = floor( uv.y *SLN  +sh )/SLN - sh/SLN;
					   // #else 
					   //  	uv.y = floor( uv.y *SLN  )/SLN;
					   //  #endif

					    // uv.y = floor( uv.y  *SLN  )/SLN ;
					    
						float slb = SLN / w; //screen lines big        

						//TODO finish
					   // #if VHS_LINESFLOAT_ON

						  //  if(uv.y<oy*ln && uv.y>oy*(ln-w)) ////if(uv.y>oy*ln && uv.y<oy*(ln+w)) 
						  //     uv.y = floor( uv.y*slb +sh2 +sh )/slb - (sh2-1.)/slb - sh/slb;

				    //   #else
						   
						   if(uv.y<oy*ln && uv.y>oy*(ln-w)) ////if(uv.y>oy*ln && uv.y<oy*(ln+w)) 
						      uv.y = floor( uv.y*slb +sh2 )/slb - (sh2-1.0)/slb ;
				      
				      // #endif

						return uv;
					}

				#endif	


				#if VHS_JITTER_V_ON

					//yiq distortion //m - amount //returns yiq value
					float3 yiqDist(float2 uv, float m, float t){

						
						m *= 0.0001; // float m = 0.0009;
						float3 offsetX = float3( uv.x, uv.x, uv.x );	

						offsetX.r += rnd_rd(float2(t*0.03, uv.y*0.42)) * 0.001 + sin(rnd_rd(float2(t*0.2, uv.y)))*m;
						offsetX.g += rnd_rd(float2(t*0.004,uv.y*0.002)) * 0.004 + sin(t*9.0)*m;
						// offsetX.b = uv.y + rnd_rd(float2(cos(t*0.01),sin(uv.y)))*m;
						// offsetX.b = uv.y + rand_rd(float2(cos(t*0.01),sin(uv.y)))*m;
					    
					   half3 signal = half3(0.0, 0.0, 0.0);
					   //it cud be optimized / but hm
					   signal.x = rgb2yiq( tex2D( _MainTex, float2(offsetX.r, uv.y) ).rgb ).x;
					   signal.y = rgb2yiq( tex2D( _MainTex, float2(offsetX.g, uv.y) ).rgb ).y;
					   signal.z = rgb2yiq( tex2D( _MainTex, float2(offsetX.b, uv.y) ).rgb ).z;

					   // signal = yiq2rgb(col);
						return signal;
					    
					}

				#endif

				#if VHS_TWITCH_V_ON	

	    			//shift part of the screen sometimes with freq
	    			float2 twitchVertical(float freq, float2 uv, float t){

					   float vShift = 0.4*onOff(freq,3.0,0.9, t);
					   vShift*=(sin(t)*sin(t*20.0) + (0.5 + 0.1*sin(t*200.0)*cos(t)));
						uv.y = fmod(uv.y + vShift, 1.0); 
						return uv;
					}

    			#endif

    			#if VHS_TWITCH_H_ON	

					//twitches the screen //freq - how often 
	    			float2 twitchHorizonal(float freq, float2 uv, float t){
	    				
						float window = 1.0/(1.0+20.0*(uv.y-fmod(t/4.0,1.0))*(uv.y-fmod(t/4.0, 1.0)));
					   	uv.x += sin(uv.y*10.0 + t)/50.0
					   		*onOff(freq,4.0,0.3, t)
					   		*(1.0+cos(t*80.0))
					   		*window;
					   
					   return uv;
	    			}

				#endif



				

				//MAIN
				half4 frag( v2f i ) : COLOR {

					float t = time_;//_Time.y;					
					float2 p = i.uvn; // normalized tex coordnates 0..1 (gl_FragCoord.xy / iResolution.xy)

					
					//basically if its 0 -> set it to fullscreen
					//TODO calc it before shader / already half done
					if(screenLinesNum==0.0) screenLinesNum = _ScreenParams.y;
					SLN = screenLinesNum; //TODO use only SLN
					SLN_Noise = noiseLinesNum; //TODO only SLN_Noise

					if(SLN_Noise==0 || SLN_Noise>SLN) SLN_Noise = SLN;									
					
					ONE_X = 1.0/_ScreenParams.x; //assigning works only here 
					ONE_Y = 1.0/_ScreenParams.y; 					
					

					#if VHS_TWITCH_V_ON			
						p = twitchVertical(0.5*twitchVFreq, p, t); 
					#endif	

					#if VHS_TWITCH_H_ON
						p = twitchHorizonal(0.1*twitchHFreq, p, t);
					#endif	

				   //make discrete lines /w or wo float 
				   #if VHS_LINESFLOAT_ON
				   	float sh = frac(-t*linesFloatSpeed); //shift  // float sh = fmod(t, 1.); //shift
				    	// if(p.x>0.5)
				    	p.y = -floor( -p.y * SLN + sh )/SLN + sh/SLN;  //v1.3
				    	// p.y = floor( p.y * SLN + sh )/SLN - sh/SLN; //v1.2
				   #else 
				    	// if(p.x>0.5)
				    	p.y = -floor( -p.y * SLN )/SLN;  //v1.3
				    	// p.y = floor( p.y * SLN )/SLN; //v1.2
				   #endif
					
					#if VHS_STRETCH_ON
					   p = stretch(p, t, 15.0, 1.0, 0.5, 0.0);
					   p = stretch(p, t, 8.0, 1.2, 0.45, 0.5);
					   p = stretch(p, t, 11.0, 0.5, -0.35, 0.25); //up   
					#endif


					#if VHS_JITTER_H_ON
				   	//interlacing. (thanks to @xra)
				    	if( fmod( p.y * SLN, 2.0)<1.0) 
				    		p.x += ONE_X*sin(t*13000.0)*jitterHAmount;
					#endif
					

					//just init
			   	half3 col = half3(0.0,0.0,0.0);
			   	half3 signal = half3(0.0,0.0,0.0);// rgb2yiq(col);


			   	//gotta initiate all these things here coz of tape noise distortion

			   	//[NOISE uv init]
			   	//if SLN_Noise different from SLN->recalc linefloat 			   	
			   	float2 pn = p;
			   	if(SLN!=SLN_Noise){
					   #if VHS_LINESFLOAT_ON
					   	float sh = frac(t); //shift  // float sh = fmod(t, 1.); //shift
					    	pn.y = floor( pn.y * SLN_Noise + sh )/SLN_Noise - sh/SLN_Noise;
					   #else 
					    	pn.y = floor( pn.y * SLN_Noise )/SLN_Noise;
					   #endif				 
				   }  	

					//SLN_X is quantization of X. goest from _ScreenParams.x to SLN_X
					float ScreenLinesNumX = SLN_Noise * _ScreenParams.x / _ScreenParams.y;
					float SLN_X = noiseQuantizeX*(_ScreenParams.x - ScreenLinesNumX) + ScreenLinesNumX;
					pn.x = floor( pn.x * SLN_X )/SLN_X;

					float2 pn_ = pn*_ScreenParams.xy;

					//TODO probably it shud be 1.0/SLN_Noise
					float ONEXN = 1.0/SLN_X;
					//[//noise uv init]


					#if VHS_TAPENOISE_ON


						//uv distortion part of tapenoise
						int distWidth = 20; 
						float distAmount = 4.0;
						float distThreshold = 0.55;
						float distShift = 0; // for 2nd part of tape noise 
						for (int ii = 0; ii < distWidth % 1023; ii++){

							//this is t.n. line value at pn.y and down each pixel
							//TODO i guess ONEXN shud be 1.0/sln noise							
							float tnl = tex2Dlod(_TapeTex, float4(0.0,pn.y-ONEXN*ii, 0.0, 0.0)).y;
							// float tnl = tex2D(_TapeTex, float2(0.0,pn.y-ONEXN*ii)).y;
							// float tnl = tapeNoiseLines(float2(0.0,pn.y-ONEXN*i), t*tapeNoiseSpeed)*tapeNoiseAmount;

							// float fadediff = hash12(float2(pn.x-ONEXN*i,pn.y)); 
							if(tnl>distThreshold) {							
								//TODO get integer part other way
								float sh = sin( 1.0*PI*(float(ii)/float(distWidth))) ; //0..1								
								p.x -= float(int(sh)*distAmount*ONEXN); //displacement
								distShift += sh ; //for 2nd part
								// p.x +=  ONEXN * float(int(((tnl-thth)/thth)*distAmount));
								// col.x = sh;	
							}

						}

					#endif	
					//uv transforms over


					//picture proccess start
					#if VHS_JITTER_V_ON						
				    	signal = yiqDist(p, jitterVAmount, t*jitterVSpeed);
			   	#else
						col = tex2D(_MainTex, p).rgb;
						// col = float3(p.xy, 0.0);//debug
						signal = rgb2yiq(col);
			   	#endif


			   	#if VHS_LINENOISE_ON || VHS_FILMGRAIN_ON
			   		signal.x += tex2D(_TapeTex, pn).z;
			   	#endif
					   
				   //iq noise from yiq
				   #if VHS_YIQNOISE_ON

				   	//TODO make cheaper noise 						
					   //type 1 (best) w Y mask
					   float2 noise = n4rand_bw( pn_,t, 1.0-signalNoisePower ) ; 
					   signal.y += (noise.x*2.0-1.0)*signalNoiseAmount*signal.x;
					   signal.z += (noise.y*2.0-1.0)*signalNoiseAmount*signal.x;

					   //type 2
					   // float2 noise = n4rand_bw( pn_,t, 1.0-signalNoisePower ) ; 
					   // signal.y += (noise.x*2.0-1.0)*signalNoiseAmount;
					   // signal.z += (noise.y*2.0-1.0)*signalNoiseAmount;

					   //type 3
					   // float2 noise = n4rand_bw( pn_,t, 1.0-signalNoisePower )*signalNoiseAmount ; 
					   // signal.y *= noise.x;
					   // signal.z *= noise.y;

					   // signal.x += (noise.x*2.0-1.0)*0.05;
					#endif


				   //2nd part with noise, tail and yiq col shift
				   #if VHS_TAPENOISE_ON


						//here is normilized p (0..1)
						half tn = tex2D(_TapeTex, pn).x;
						signal.x = bms(signal.x, tn*tapeNoiseAmount );  
						// float tn = tapeNoise(pn, t*tapeNoiseSpeed)*tapeNoiseAmount;

						//tape noise tail
						int tailLength=10; //TODO adjustable

						for(int j = 0; j < tailLength % 1023; j++){

							float jj = float(j);
							float2 d = float2(pn.x-ONEXN*jj,pn.y);
							tn = tex2Dlod(_TapeTex, float4(d,0.0,0.0) ).x;
							// tn = tex2D(_TapeTex, d).x;
							// tn = tapeNoise(float2(pn.x-ONEXN*i,pn.y), t*tapeNoiseSpeed)*tapeNoiseAmount;

							//for tails length difference
							float fadediff = tex2D(_TapeTex, d).a; //hash12(d); 

							if( tn > 0.8 ){								
								float nsx =  0.0; //new signal x
								float newlength = float(tailLength)*(1.0-fadediff); //tail lenght diff
								if( jj <= newlength ) nsx = 1.0-( jj/ newlength ); //tail
								signal.x = bms(signal.x, nsx*tapeNoiseAmount);									
							}
							
						}

						//tape noise color shift
						if(distShift>0.4){
							// float tnl = tapeNoiseLines(float2(0.0,pn.y), t*tapeNoiseSpeed)*tapeNoiseAmount;
							float tnl = tex2D(_TapeTex, pn).y;//tapeNoiseLines(float2(0.0,pn.y), t*tapeNoiseSpeed)*tapeNoiseAmount;
						   signal.y *= 1.0/distShift;//tnl*0.1;//*distShift;//*signal.x;
						   signal.z *= 1.0/distShift;//*distShift;//*signal.x;

						}

				   #endif


				   //back to rgb color space
				   //signal has negative values
					col = yiq2rgb(signal);
						
				   
				  	//TODO put it into 2nd pass
				   #if VHS_SCANLINES_ON
						col *= scanLines(i.uvn, t); 						
				   #endif

			   	//fisheye cutoff / outside fisheye
			   	//helps to remove noise outside the fisheye
			   	#if VHS_FISHEYE_ON

			   		half cof_x = cutoffFadeX;
			   		half cof_y = cutoffFadeY;


			   		p = i.uvn;

			   		half far;
						half2 hco = half2(ONE_X*cutoffX, ONE_Y*cutoffY); //hard cuttoff x
						half2 sco = half2(ONE_X*cutoffFadeX, ONE_Y*cutoffFadeY); //soft cuttoff x

			   		//hard cutoff
			   		if( p.x<=(0.0+hco.x) || p.x>=(1.0-hco.x) ||
			   			 p.y<=(0.0+hco.y) || p.y>=(1.0-hco.y) ){

			   			col = half3(0.0,0.0,0.0);
						}
						else
						{ //fades

							

							if( //X
								(p.x>(0.0+hco.x) 			 && p.x<(0.0+(sco.x+hco.x) )) ||
								(p.x>(1.0-(sco.x+hco.x)) && p.x<(1.0-hco.x)) 
							){								
								if(p.x<0.5)	far = (0.0-hco.x+p.x)/(sco.x);									
								else			far = (1.0-hco.x-p.x)/(sco.x);
								
								col *= half(far).xxx;
							}; 

							if( //Y
								(p.y>(0.0+hco.y) 			 && p.y<(0.0+(sco.y+hco.y) )) ||
								(p.y>(1.0-(sco.y+hco.y)) && p.y<(1.0-hco.y)) 
							){
								if(p.y<0.5)	far = (0.0-hco.y+p.y)/(sco.y);									
								else			far = (1.0-hco.y-p.y)/(sco.y);
								
								col *= half(far).xxx;
							}

						}

					#endif

					// col = tex2D(_TapeTex, i.uvn).x;

					return half4(col, 1.0); 

				}

			ENDCG
		}
	}
}
