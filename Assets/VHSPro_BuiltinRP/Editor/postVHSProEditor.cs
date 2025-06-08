using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(postVHSPro))]
[CanEditMultipleObjects]
public class postVHSProEditor : Editor {

	//groups/sections folding save
	SerializedProperty g_showCRT;
	SerializedProperty g_showNoise;
	SerializedProperty g_showJitter;
	SerializedProperty g_showSignal;
	SerializedProperty g_showFeedback;
	SerializedProperty g_showExtra;
	SerializedProperty g_showBypass;


	//CRT
	SerializedProperty bleedOn; 
	SerializedProperty crtMode; 
	SerializedProperty crtLinesMode;
	SerializedProperty screenLinesNum;
	SerializedProperty bleedAmount;

	SerializedProperty bleedCurveEditModeOn;
	SerializedProperty bleedCurveY;
	SerializedProperty bleedCurveI;
	SerializedProperty bleedCurveQ;
	SerializedProperty bleedCurveIQSyncOn;
	SerializedProperty bleedLength; 

	SerializedProperty bleedDebugOn; 

	SerializedProperty fisheyeOn;
	SerializedProperty fisheyeBend; 
	SerializedProperty fisheyeType; 
	SerializedProperty cutoffX;
	SerializedProperty cutoffY;
	SerializedProperty cutoffFadeX;
	SerializedProperty cutoffFadeY;
	

	SerializedProperty vignetteOn;
	SerializedProperty vignetteAmount; 
	SerializedProperty vignetteSpeed; 


	//NOISE
	SerializedProperty noiseLinesMode;
	SerializedProperty noiseLinesNum; 
	SerializedProperty noiseQuantizeX; 

	SerializedProperty filmgrainOn;
	SerializedProperty filmGrainAmount; 
	// SerializedProperty filmGrainPower; 
	SerializedProperty tapeNoiseOn;
	SerializedProperty tapeNoiseTH; 
	SerializedProperty tapeNoiseAmount; 
	SerializedProperty tapeNoiseSpeed; 
	SerializedProperty lineNoiseOn;
	SerializedProperty lineNoiseAmount; 
	SerializedProperty lineNoiseSpeed; 


	//JITTER
	SerializedProperty scanLinesOn;
	SerializedProperty scanLineWidth;
	
	SerializedProperty linesFloatOn; 
	SerializedProperty linesFloatSpeed; 
	SerializedProperty stretchOn;

	SerializedProperty twitchHOn; 
	SerializedProperty twitchHFreq; 
	SerializedProperty twitchVOn; 
	SerializedProperty twitchVFreq; 

	SerializedProperty jitterHOn; 
	SerializedProperty jitterHAmount; 
	SerializedProperty jitterVOn; 
	SerializedProperty jitterVAmount; 
	SerializedProperty jitterVSpeed; 
	

	//SIGNAL TWEAK
	SerializedProperty signalTweakOn; 
	SerializedProperty signalAdjustY; 
	SerializedProperty signalAdjustI; 
	SerializedProperty signalAdjustQ; 

	SerializedProperty signalShiftY; 
	SerializedProperty signalShiftI; 
	SerializedProperty signalShiftQ; 

	SerializedProperty signalNoiseOn; 
	SerializedProperty signalNoiseAmount; 
	SerializedProperty signalNoisePower; 

	SerializedProperty gammaCorection; 


	//FEEDBACK
	SerializedProperty feedbackOn; 
	// SerializedProperty feedbackMode; 

	SerializedProperty feedbackAmount; 
	SerializedProperty feedbackFade; 
	SerializedProperty feedbackColor; 	

	SerializedProperty feedbackThresh; 	
	SerializedProperty feedbackDebugOn; 	
	// SerializedProperty feedbackAmp; 



	//TOOLS
	SerializedProperty independentTimeOn; 

	//BYPASS
	SerializedProperty bypassTex;
	SerializedProperty spriteTex;
	// SerializedProperty movieTex;


	//Materials 
	SerializedProperty mat;


	SerializedObject so; 

	void OnEnable () {

      so = new SerializedObject(target); //serializedObject

		// Setup the SerializedProperties.
		//groups/sections folding save
		g_showCRT = 			so.FindProperty("g_showCRT");
		g_showNoise = 			so.FindProperty("g_showNoise");
		g_showJitter = 		so.FindProperty("g_showJitter");
		g_showSignal = 		so.FindProperty("g_showSignal");
		g_showFeedback = 		so.FindProperty("g_showFeedback");
		g_showExtra = 			so.FindProperty("g_showExtra");
		g_showBypass = 		so.FindProperty("g_showBypass");


		//CRT
		bleedOn = 				so.FindProperty("bleedOn"); 
		crtMode = 				so.FindProperty("crtMode"); 
		crtLinesMode = 		so.FindProperty("crtLinesMode");
		screenLinesNum = 		so.FindProperty("screenLinesNum");
		bleedAmount = 			so.FindProperty("bleedAmount");

		bleedCurveEditModeOn =so.FindProperty("bleedCurveEditModeOn");
		bleedCurveY = 			so.FindProperty("bleedCurveY");
		bleedCurveI = 			so.FindProperty("bleedCurveI");
		bleedCurveQ = 			so.FindProperty("bleedCurveQ");
		bleedCurveIQSyncOn = so.FindProperty("bleedCurveIQSyncOn");
		bleedLength = 			so.FindProperty("bleedLength");

		bleedDebugOn = 		so.FindProperty("bleedDebugOn");

		fisheyeOn = 			so.FindProperty("fisheyeOn");
		fisheyeBend = 			so.FindProperty("fisheyeBend"); 
		fisheyeType = 			so.FindProperty("fisheyeType"); 
		cutoffX =  				so.FindProperty("cutoffX");
		cutoffY =  				so.FindProperty("cutoffY");
		cutoffFadeX =  		so.FindProperty("cutoffFadeX");
		cutoffFadeY =  		so.FindProperty("cutoffFadeY");

		
		vignetteOn = 			so.FindProperty("vignetteOn");
		vignetteAmount = 		so.FindProperty("vignetteAmount"); 
		vignetteSpeed = 		so.FindProperty("vignetteSpeed"); 

		//NOISE
		noiseLinesMode = 		so.FindProperty("noiseLinesMode");
		noiseLinesNum = 		so.FindProperty("noiseLinesNum"); 
		noiseQuantizeX = 		so.FindProperty("noiseQuantizeX"); 

		filmgrainOn = 			so.FindProperty("filmgrainOn");
		filmGrainAmount = 	so.FindProperty("filmGrainAmount"); 
		// filmGrainPower = 		so.FindProperty("filmGrainPower"); 
		tapeNoiseOn = 			so.FindProperty("tapeNoiseOn");
		tapeNoiseTH = 			so.FindProperty("tapeNoiseTH"); 
		tapeNoiseAmount = 	so.FindProperty("tapeNoiseAmount"); 
		tapeNoiseSpeed = 		so.FindProperty("tapeNoiseSpeed"); 
		lineNoiseOn = 			so.FindProperty("lineNoiseOn");
		lineNoiseAmount = 	so.FindProperty("lineNoiseAmount"); 
		lineNoiseSpeed = 		so.FindProperty("lineNoiseSpeed"); 


		//JITTER
		scanLinesOn = 			so.FindProperty("scanLinesOn");
		scanLineWidth = 		so.FindProperty("scanLineWidth");
		
		linesFloatOn = 		so.FindProperty("linesFloatOn"); 
		linesFloatSpeed = 	so.FindProperty("linesFloatSpeed"); 
		stretchOn = 			so.FindProperty("stretchOn");

		twitchHOn = 			so.FindProperty("twitchHOn"); 
		twitchHFreq = 			so.FindProperty("twitchHFreq"); 
		twitchVOn = 			so.FindProperty("twitchVOn"); 
		twitchVFreq = 			so.FindProperty("twitchVFreq"); 

		jitterHOn = 			so.FindProperty("jitterHOn"); 
		jitterHAmount = 		so.FindProperty("jitterHAmount"); 
		jitterVOn = 			so.FindProperty("jitterVOn"); 
		jitterVAmount = 		so.FindProperty("jitterVAmount"); 
		jitterVSpeed = 		so.FindProperty("jitterVSpeed"); 
		

		//SIGNAL TWEAK
		signalTweakOn = 		so.FindProperty("signalTweakOn"); 
		signalAdjustY = 		so.FindProperty("signalAdjustY"); 
		signalAdjustI = 		so.FindProperty("signalAdjustI"); 
		signalAdjustQ = 		so.FindProperty("signalAdjustQ"); 

		signalShiftY = 		so.FindProperty("signalShiftY"); 
		signalShiftI = 		so.FindProperty("signalShiftI"); 
		signalShiftQ = 		so.FindProperty("signalShiftQ"); 


		signalNoiseOn = 		so.FindProperty("signalNoiseOn"); 
		signalNoiseAmount = 	so.FindProperty("signalNoiseAmount"); 
		signalNoisePower = 	so.FindProperty("signalNoisePower"); 

		gammaCorection = 		so.FindProperty("gammaCorection"); 


		//FEEDBACK
		feedbackOn = 			so.FindProperty("feedbackOn"); 
		// feedbackMode = 		so.FindProperty("feedbackMode"); 

		feedbackAmount = 		so.FindProperty("feedbackAmount"); 
		feedbackFade = 		so.FindProperty("feedbackFade"); 
		feedbackColor = 		so.FindProperty("feedbackColor"); 

		feedbackThresh = 		so.FindProperty("feedbackThresh"); 
		feedbackDebugOn = 	so.FindProperty("feedbackDebugOn"); 
		// feedbackAmp = 		so.FindProperty("feedbackAmp"); 


		//TOOLS
		independentTimeOn = 	so.FindProperty("independentTimeOn"); 


		//BYPASS
		bypassTex = 			so.FindProperty("bypassTex");
		spriteTex = 			so.FindProperty("spriteTex");
		// movieTex = 				so.FindProperty("movieTex");


		//Materials
		mat = 			so.FindProperty("mat");

	}


   public override void OnInspectorGUI(){
   	
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		so.Update();

		GUIStyle boldFoldout = new GUIStyle(EditorStyles.foldout);
		boldFoldout.fontStyle = FontStyle.Bold;


      //BYPASS
		//if you are using movie texture or sprite the first pass will be aplied to it
		g_showBypass.boolValue = EditorGUILayout.Foldout(g_showBypass.boolValue, "Use Custom Texture", boldFoldout);
		if(g_showBypass.boolValue){

	      bypassTex.objectReferenceValue = 	EditorGUILayout.ObjectField("Bypass Texture", bypassTex.objectReferenceValue, typeof(Texture), false);
	      spriteTex.objectReferenceValue = 	EditorGUILayout.ObjectField("Sprite Texture", spriteTex.objectReferenceValue, typeof(Sprite), false);
	      // movieTex.objectReferenceValue = 	EditorGUILayout.ObjectField("Movie Texture", movieTex.objectReferenceValue, typeof(MovieTexture), false);
			EditorGUILayout.Space();

		}

      //CRT
	   g_showCRT.boolValue = EditorGUILayout.Foldout(g_showCRT.boolValue, "Cathode Ray Tube Emulation", boldFoldout);
	   if(g_showCRT.boolValue){
      	   	
	   	bleedOn.boolValue = 		EditorGUILayout.Toggle("Bleeding", 				bleedOn.boolValue);
	   	indP();
	   	
		   crtLinesMode.intValue = EditorGUILayout.Popup("Vertical Resolution", crtLinesMode.intValue, 
		   		new string[4] {"FullScreen", "PAL 240 Lines", "NTSC 480 Lines", "Custom"});

		   if(crtLinesMode.intValue==0) screenLinesNum.floatValue = 0;
		   if(crtLinesMode.intValue==1) screenLinesNum.floatValue = 240;
		   if(crtLinesMode.intValue==2) screenLinesNum.floatValue = 480;
      	if(crtLinesMode.intValue==3) {
      		// indP();
      		screenLinesNum.floatValue = EditorGUILayout.FloatField("Lines Per Height", screenLinesNum.floatValue);
      		// indM();
      	}

		   crtMode.intValue = 			EditorGUILayout.Popup("Bleed Mode", crtMode.intValue,  
		   											new string[4] {"Old Three Phase", "Three Phase", "Two Phase (slow)", "Custom Curve"});

			if(crtMode.intValue==0) bleedLength.intValue = 24;
			if(crtMode.intValue==1) bleedLength.intValue = 24;
			if(crtMode.intValue==2) bleedLength.intValue = 32;
			if(crtMode.intValue==3){

				// indP();
				bleedCurveEditModeOn.boolValue = 	EditorGUILayout.BeginToggleGroup("Edit Mode", bleedCurveEditModeOn.boolValue);	        

					bleedCurveIQSyncOn.boolValue = 	EditorGUILayout.Toggle("IQ Sync", bleedCurveIQSyncOn.boolValue);

					bleedCurveY.animationCurveValue = EditorGUILayout.CurveField("Curve Y", bleedCurveY.animationCurveValue);
					if(bleedCurveIQSyncOn.boolValue){
						bleedCurveI.animationCurveValue = EditorGUILayout.CurveField("Curve IQ", bleedCurveI.animationCurveValue);						
					}else{
						bleedCurveI.animationCurveValue = EditorGUILayout.CurveField("Curve I", bleedCurveI.animationCurveValue);
						bleedCurveQ.animationCurveValue = EditorGUILayout.CurveField("Curve Q", bleedCurveQ.animationCurveValue);						
					}

					bleedLength.intValue = 		  (int)EditorGUILayout.Slider("Bleed Length",bleedLength.intValue, 0.0f, 50.0f);
				EditorGUILayout.EndToggleGroup();

				EditorGUILayout.HelpBox("Note: \n1. Bigger 'Bleed Length' values cause slow performance. Try to use as small value as possible. Default is 21.\n2. Turn the 'Edit Mode' off for the final build for faster performance. ", MessageType.Info );

				// indM();
			}			
				
			   bleedAmount.floatValue = 	EditorGUILayout.Slider("Bleed Stretch", 			bleedAmount.floatValue, 0.0f, 15.0f); //def 1.-2.
			   indM();

		   fisheyeOn.boolValue = 		EditorGUILayout.Toggle("Fisheye", 				fisheyeOn.boolValue);
		   	indP();
		   	fisheyeType.intValue = 		EditorGUILayout.Popup("Type", fisheyeType.intValue, 
		   											new string[2] {"Default", "Hyperspace"});
		   	fisheyeBend.floatValue = 	EditorGUILayout.Slider("Bend", 				fisheyeBend.floatValue, 0.0f, 50.0f); 		   	
				cutoffX.floatValue = 		EditorGUILayout.Slider("Cutoff X", 			cutoffX.floatValue, 0.0f, 50.0f);  				
				cutoffY.floatValue = 		EditorGUILayout.Slider("Cutoff Y", 			cutoffY.floatValue, 0.0f, 50.0f);  				
				cutoffFadeX.floatValue = 	EditorGUILayout.Slider("Cutoff Fade X", 	cutoffFadeX.floatValue, 0.0f, 50.0f);  		
				cutoffFadeY.floatValue = 	EditorGUILayout.Slider("Cutoff Fade Y", 	cutoffFadeY.floatValue, 0.0f, 50.0f);  		
		   	indM();

		   vignetteOn.boolValue = 		EditorGUILayout.Toggle("Vignette", 					vignetteOn.boolValue);        
		   	indP();
		   	vignetteAmount.floatValue =EditorGUILayout.Slider("Amount", 		vignetteAmount.floatValue, 0.0f, 5.0f); 
		   	vignetteSpeed.floatValue = EditorGUILayout.Slider("Pulse Speed", 	vignetteSpeed.floatValue, 0.0f, 5.0f); 
		   	indM();
		   EditorGUILayout.Space();
		}


	   //NOISE
	   g_showNoise.boolValue = 		EditorGUILayout.Foldout(g_showNoise.boolValue, "Noise", boldFoldout);
	   if(g_showNoise.boolValue){		   
			
		   noiseLinesMode.intValue = 	EditorGUILayout.Popup("Vertical Resolution", noiseLinesMode.intValue, 
		   														new string[2] {"Global", "Custom"});
		   if(noiseLinesMode.intValue==0) noiseLinesNum.floatValue = screenLinesNum.floatValue;
      	if(noiseLinesMode.intValue==1) {
      		indP();
      		noiseLinesNum.floatValue = EditorGUILayout.FloatField("Lines Per Height", noiseLinesNum.floatValue);
      		indM();
      	}

		   noiseQuantizeX.floatValue = EditorGUILayout.Slider("Quantize Noise X", noiseQuantizeX.floatValue, 0.0f, 1.0f); 		   
			EditorGUILayout.Space();	   
		   
		   filmgrainOn.boolValue = 		EditorGUILayout.Toggle("Film Grain", 			filmgrainOn.boolValue);        
			   indP();
			   filmGrainAmount.floatValue = 	EditorGUILayout.Slider("Alpha", 		filmGrainAmount.floatValue, 0.0f, 0.1f); 
			   // filmGrainPower.floatValue = 	EditorGUILayout.Slider("Power", 		filmGrainPower.floatValue, 0.0f, 1.0f); 
			   indM();

		   signalNoiseOn.boolValue = 		EditorGUILayout.Toggle("Signal Noise", 			signalNoiseOn.boolValue);        
			   indP();
			   signalNoiseAmount.floatValue = 	EditorGUILayout.Slider(" Amount", 		signalNoiseAmount.floatValue, 0f, 1f);
			   signalNoisePower.floatValue = 	EditorGUILayout.Slider(" Power", 			signalNoisePower.floatValue, 0f, 1f);
			   indM();

		   lineNoiseOn.boolValue = 		EditorGUILayout.Toggle("Line Noise", 					lineNoiseOn.boolValue);        
		   	indP();
			   lineNoiseAmount.floatValue = 	EditorGUILayout.Slider("Alpha", 						lineNoiseAmount.floatValue, 0.0f, 10.0f); 
			   lineNoiseSpeed.floatValue = 	EditorGUILayout.Slider("Speed", 						lineNoiseSpeed.floatValue, 0.0f, 10.0f); 
		   	indM();
		   tapeNoiseOn.boolValue = 		EditorGUILayout.Toggle("Tape Noise", 					tapeNoiseOn.boolValue);
				indP();
			   tapeNoiseTH.floatValue = 		EditorGUILayout.Slider("Amount", 					tapeNoiseTH.floatValue, 0.0f, 1.5f); 
			   tapeNoiseSpeed.floatValue = 	EditorGUILayout.Slider("Speed", 						tapeNoiseSpeed.floatValue, 0.0f, 1.5f); 
			   tapeNoiseAmount.floatValue = 	EditorGUILayout.Slider("Alpha",						tapeNoiseAmount.floatValue, 0.0f, 1.5f); 
			   indM();
			EditorGUILayout.Space();	   
		}


      //JITTER
	   g_showJitter.boolValue = EditorGUILayout.Foldout(g_showJitter.boolValue, "Jitter & Twitch", boldFoldout);
	   if(g_showJitter.boolValue){

		   scanLinesOn.boolValue = 	EditorGUILayout.Toggle("Show Scanlines", 	scanLinesOn.boolValue);        
			   indP();
			   scanLineWidth.floatValue = EditorGUILayout.Slider("Width", 	scanLineWidth.floatValue, 0.0f, 20.0f);
			   indM();
			EditorGUILayout.Space();			

			linesFloatOn.boolValue = 	EditorGUILayout.Toggle("Floating Lines", 	linesFloatOn.boolValue);        
				indP();
				linesFloatSpeed.floatValue = EditorGUILayout.Slider("Speed", 	linesFloatSpeed.floatValue, -3.0f, 3.0f);
				indM();
		   stretchOn.boolValue = 		EditorGUILayout.Toggle("Stretch Noise", 	stretchOn.boolValue);        
			EditorGUILayout.Space();

	      jitterHOn.boolValue = 		EditorGUILayout.Toggle("Interlacing", 	jitterHOn.boolValue);        
	      	indP();
	      	jitterHAmount.floatValue = EditorGUILayout.Slider("Amount", 			jitterHAmount.floatValue, 0.0f, 5.0f); //default .5-1.
	      	indM();
	      jitterVOn.boolValue = 		EditorGUILayout.Toggle("Jitter", 	jitterVOn.boolValue);
	      	indP();
		      jitterVAmount.floatValue = EditorGUILayout.Slider("Amount", 	jitterVAmount.floatValue, 0.0f, 15.0f); //default .5-1
		      jitterVSpeed.floatValue = 	EditorGUILayout.Slider("Speed", 		jitterVSpeed.floatValue, 0.0f, 5.0f); //default .5-1
		      indM();
      	EditorGUILayout.Space();

			twitchHOn.boolValue = 		EditorGUILayout.Toggle("Twitch Horizontal", 	twitchHOn.boolValue);        
				indP();
	      	twitchHFreq.floatValue = 	EditorGUILayout.Slider("Frequency", 		twitchHFreq.floatValue, 0.0f, 5.0f); 
	      	indM();
			twitchVOn.boolValue = 		EditorGUILayout.Toggle("Twitch Vertical", 	twitchVOn.boolValue);        
				indP();
		      twitchVFreq.floatValue = 	EditorGUILayout.Slider("Frequency", 		twitchVFreq.floatValue, 0.0f, 5.0f); 
		      indM();
			EditorGUILayout.Space();
   	}

        
      //SIGNAL
	   g_showSignal.boolValue = EditorGUILayout.Foldout(g_showSignal.boolValue, "Signal", boldFoldout);
	   if(g_showSignal.boolValue){

			signalTweakOn.boolValue = 	EditorGUILayout.BeginToggleGroup("Signal Tweak", signalTweakOn.boolValue);	        
				indP(); 
			   signalAdjustY.floatValue = EditorGUILayout.Slider("Shift Y", 			signalAdjustY.floatValue, -0.25f, 0.25f);
			   signalAdjustI.floatValue = EditorGUILayout.Slider("Shift I", 			signalAdjustI.floatValue, -0.25f, 0.25f);
			   signalAdjustQ.floatValue = EditorGUILayout.Slider("Shift Q", 			signalAdjustQ.floatValue, -0.25f, 0.25f);
			   signalShiftY.floatValue = 	EditorGUILayout.Slider("Adjust Y", 			signalShiftY.floatValue, -2.0f, 2.0f);
			   signalShiftI.floatValue = 	EditorGUILayout.Slider("Adjust I", 			signalShiftI.floatValue, -2.0f, 2.0f);
			   signalShiftQ.floatValue = 	EditorGUILayout.Slider("Adjust Q", 			signalShiftQ.floatValue, -2.0f, 2.0f);

			   gammaCorection.floatValue =EditorGUILayout.Slider("Gamma Corection", gammaCorection.floatValue, 0.0f, 2.0f);
			   indM();
	    	EditorGUILayout.EndToggleGroup();
	   }


	   //FEEDBACK
	   g_showFeedback.boolValue = EditorGUILayout.Foldout(g_showFeedback.boolValue, "Phosphor Trail", boldFoldout);
	   if(g_showFeedback.boolValue){

			feedbackOn.boolValue = 		EditorGUILayout.Toggle("Phosphor Trail", 	feedbackOn.boolValue);   
				indP();     
			   //feedbackMode.intValue = 	EditorGUILayout.Popup("Mode", feedbackMode.intValue, new string[10] {"VHS Screen", "VHS Color Dodge", "VHS Add", "RGB Screen", "RGB Color Dodge", "RGB Add", "GLITCH Color Burn", "GLITCH Linear Burn", "GLITCH Divide", "GLITCH Subtract"});
				// EditorGUILayout.HelpBox("GLITCH modes are experimental. They ARE look broken.", MessageType.Info );

				feedbackThresh.floatValue = EditorGUILayout.Slider("Input Cutoff", feedbackThresh.floatValue, 0.0f, 1.0f);
				feedbackAmount.floatValue = EditorGUILayout.Slider("Input Amount", feedbackAmount.floatValue, 0.0f, 3.0f);
				feedbackFade.floatValue = 	 EditorGUILayout.Slider("Fade", 	 feedbackFade.floatValue, 0.0f, 1.0f);
				feedbackColor.colorValue =  EditorGUILayout.ColorField("Color", 	 feedbackColor.colorValue);
				// feedbackAmp.floatValue = EditorGUILayout.Slider("Amp", feedbackAmp.floatValue, 0.0f, 10.0f);


				indM();
		}


		//TOOLS
	   g_showExtra.boolValue = EditorGUILayout.Foldout(g_showExtra.boolValue, "Tools", boldFoldout);
	   if(g_showExtra.boolValue){
			independentTimeOn.boolValue = EditorGUILayout.Toggle("Use unscaled time", 	independentTimeOn.boolValue);   
			bleedDebugOn.boolValue = 		EditorGUILayout.Toggle("Debug Bleed Curve",  bleedDebugOn.boolValue);   
			feedbackDebugOn.boolValue = 	EditorGUILayout.Toggle("Debug Trail",  feedbackDebugOn.boolValue);   
	   }




		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		so.ApplyModifiedProperties();

   }

   //Helpers
   private void indP(){
   	EditorGUI.indentLevel++;
   	EditorGUI.indentLevel++;
   }

   private void indM(){
   	EditorGUI.indentLevel--;
   	EditorGUI.indentLevel--;
   }
   
}