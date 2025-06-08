using UnityEngine;
using System.Collections;

public class quadPlay : MonoBehaviour {

	// #if !(UNITY_PS4 || UNITY_IOS || UNITY_XBOXONE || UNITY_ANDROID)
	// 	public MovieTexture movTexture;
	// #endif

	// Use this for initialization
	void Start () {
		
		// #if !(UNITY_PS4 || UNITY_IOS || UNITY_XBOXONE || UNITY_ANDROID)

		// 	MovieTexture t = (MovieTexture)this.GetComponent<Renderer>().material.mainTexture;
		// 	t.loop = true;
		// 	t.Play();	

		// #endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
