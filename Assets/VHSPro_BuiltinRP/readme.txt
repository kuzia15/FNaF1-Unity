VHS Pro 1.4 Manual (URP and Built-in Render Pipeline version) 

[Intro]

VHS Pro is a camera image effect that emulates look and feel of old CRT(Cathode Ray Tube) screens. It emulates screen bleeding (ray tail on the phosphor screen), VHS tape noise (VHS data corruption), interlacing and jitter (deviation) in the analog video signal.
Best for 2D console-like games, 80's/90's/Retro/VHS look, security cameras and robot/computer vision.


[Parameters Description]

1. Custom Texture (Bypass Mode)
You can use custom movie texture or a sprite if you want it to be affected by the effect.
Select your "Movie Texture" or a "Sprite Texture" and the effect will bypass the scene and will only work on your custom texture.

2. CRT(Cathode Ray Tube) Emulation
Emulates CRT Screen. Bleeding, Fisheye, discretion (quantization) and vignette. 

	a. Vertical Resolution (Vertical Quantization)
	The picture will be quantized vertically by lines. You can choose the amount of lines:
	"FullScreen" - no quantization
	"PAL 240 Lines" - 240 Lines per height
	"NTSC 480 Lines" - 480 Lines per height
	"Custom" - You can set the number of Lines in "Lines Per Height" field
	 
	b. Bleed Mode (CRT screen bleeding mode) 
	“Bleeding” is a cathode ray tail on the phosphor screen. When the ray (electron beam) passing certain point of the phosphor screen, the point keep glowing some small amount of time after the ray is already gone. So, the ray has a fading tail behind itself while going across the screen. This effect called bleeding. 
	You can choose between these modes: "Old Three Phase", "Three Phase", "Two Phase" (works slower) and "Custom Curve". These modes emulate different screen bleeding curves. 

	"Custom Curve" Bleed Mode allows you to build your own bleeding curve. For easier understanding of this effect you can turn on "Tools->Debug Bleed Curve".
	Use "Edit Mode" while you building the curve and turn it off for the final build of your game. Otherwise the curve will be caching every frame and it will reduce the performance.
	"Curve Y", "Curve I", "Curve Q" - play with these curves to build the fading curve for each of YIQ channels. 
	You can add and remove keys, drag them back and forth but the curve must stay between 0.0 and 1.0 seconds in the end.
	"IQ Sync" - use this if you want the same curve for I and Q channels.
	"Bleed Length" - This parameter changes the length of the curve. Big values cause slow performance. Try to use as small value as possible. Default is 21. You can always use "Bleed Stretch" slider to stretch the curve. It doesn't look that nice all the time but it's very cheap.

	c. Bleed Stretch
	Makes bleeding curve longer or shorter by stretching the it. 

	d. Fisheye
	Emulates a real screen by "bending" the corners of the image and making it look like if it would be put thru a wide-angle lens.
	"Bend" - adjust fisheye amount

	e. Vignette
	Emulates vignette (reduction of an image's brightness or saturation at the periphery compared to the image center).
	"Amount" - Adjust vignette amount.
	"Pulse Speed" - change speed how vignette pulsates

3. Noise 
Emulates different sorts of noises.

	a. Vertical Resolution (Noise Vertical Quantization)
	The noise vertical quantization. 
	"Global" - choose this to inherit quantization from the CRT Vertical Resolution.
	"Custom" - if you want to use custom vertical resolution for noise which is different from CRT Vertical Resolution. Set value in "Lines Per Height" field. It should be less than  CRT Vertical resolution.

	b. Quantize Noise X
	If you want to quantize noise by width (i.e. make squares instead of tiny vertical lines) then adjust this value.

	c. Film Grain
	This is a simple background noise. 
	"Alpha" - Adjusts transparency of background noise.

	d. Signal Noise
	Emulates signal noise. Changes the YIQ colors. 
	"Amount" - Adjusts amount of noise.
	"Power" - Adjusts density of noise.

	e. Line Noise
	Emulates noise in the analog video signal and VHS cassettes. Noise lines popping up randomly within the screen.
	"Amount" - Adjusts transparency.
	"Speed" - Adjusts speed.

	f. Tape Noise
	Emulates noise which you can find on old VHS cassettes. Noise lines floating down the screen.
	"Amount" - Adjusts noise cutoff.
	"Speed" - Adjusts speed.
	"Alpha" - Adjusts transparency.


4. Jitter
Emulates deviations in the analog video signal and CRT.

	a. Show Scanlines 
	Draws the black lines in between the quantized screenlines.
	"Width" - Adjusts the width of the scanlines.

	b. Floating Lines
	After the screen was quantized vertically it consist of the horizontal lines which will float down the screen if this option is on. It works the best on the low resolution. 

	c. Stretch Noise
	Emulates noise, and data corruption on the VHS cassette plus some CRT jitter. Looks like if some of the screen lines were stretched and floating up and down the screen.

	d. Jitter Horizontal
	Emulates interlacing jitter.
	"Amount" - Adjusts the amount.

	e. Jitter Vertical
	Emulates analog video signal and CRT jitter. Adds a bit of YIQ shifting.
	"Amount" - Adjusts transparency.
	"Speed" - Adjusts speed.

	f. Twitch Horizontal
	Shifts/displaces the image horizontally sometimes.
	"Frequency" - Adjusts how often.

	g. Twitch Vertical
	Shakes/Shifts screen horizontally sometimes. The images "jumps" or "falls" vertically.
	"Frequency" - Adjusts how often.

5. Signal
YIQ is the color space used by the NTSC color TV system. The analog video signal is transmitted in YIQ and not in RGB. In this section you can adjust the YIQ values. 

	a. Shift YIQ
	"Shift Y", "Shift I", "Shift Q" - use these to tweak/shift the values.

	b. YIQ Permanent Adjustment
	"Adjust Y", "Adjust I", "Adjust Q" - Use these to make a permanent adjustment.

	c. Gamma Correction
	"Gamma Correction" - use this to balance the gamma(brightness) of the signal.
 
5. Phosphor trail
Emulates phosphor screen decay. It basically works as a feedback and adds a part of previous frame to the current one. 
For easier understanding of this effect you can turn on "Tools->Debug Trail". Then you will see only the trail. It's better to adjust it that way.

"Input Cutoff" - Adjusting brightness threshold of input. How much of each frame affects the trail. If it equal to X value, it means all pixels with brightness lower than X are not gonna affect the trail and all pixels with brightness more than X are going to affect the trail. 
"Input Amount" - Amplifies the input amount after cutoff. 
"Fade" - Adjusts how fast the trail fades. In other words it's a feedback amount.

6. Tools
Additional tools.

	a. Unscaled Time.
	When you are pausing the game using Time.timeScale = 0 (or Application.timeScale = 0) it stops all the animation, sound, etc. It also stops shader timer and it's animation. If you still need shader to run while this sort of pause you can use "unscaled time" feature. The shader will keep running even when your Time.timeScale == 0. It will use Time.unscaledTime instead of Time.time.

	b. Debug Bleed Curve
	Helps to debug bleed curve.

	c. Debug Trail
	Helps to debug phosphor trail.


[Setup, installation instructions, how to use] 

Add VHS Pro component to your camera. Adjust parameters. That's pretty much it. 



[Contact]

If you have any questions or want to report a bug, please, write me here
Email: vladstorm00@gmail.com
Twitter: https://twitter.com/vladstorm_


[Update Log]

v1.4
+ removed MovieTexture support 'cause it's depricated

v1.3.2
+ fixed bug with the Timeline 

v1.3.1
+ minor fixes

v1.3
+switched from multi_compiles to shader_features (faster building, smaller build size, but you have to turn on multi_compile for the features you switching on/off in runtime)
+added feedback color
+added fisheye hard and soft cutoff
+added hyperspace cuttoff
+renamed movietex to bypass tex
+auto disable movie texture for ps4/xbox/mobile
+added bleeding on/off switch
+fixed 'for' loops for windows
+fixed 'lines float' feature
+added 'lines float' speed - with both ways
+fixed ps4 bug

v1.2

+ added signal noise 
+ added distortion to tape noise
+ added tails to tape noise
+ added color shift to tape noise
+ make better background noise (fixed cross pattern)
+ renamed background noise amount to alpha
+ renamed background noise to film grain
+ put noises into YIQ space
+ a lot of noise performance optimizations
+ renamed jitter vertical to jitter
+ renamed jitter horizontal to interlacing
+ renamed tape noise values (amount to alpha, threshhold to amount)
+ renamed/switched shift and adjust of signal tweak

v1.1

+ changed vertical jitter from RGB color space to YIQ color space
+ fixed gamma correction range. now you can go both directions
+ fixed scanlines - made them more expensive but better looking
+ added scanlines width parameter - allows you adjust scanline width

+ renamed crt mode to bleed mode
+ added custom bleeding curve feature - now you can create your own bleeding curves
+ bleed amount renamed to bleed stretch 

+ added phosphor trail section

+ added tools section
+ added unscaled time feature - now you can use unscaled time 

+ made better UI
