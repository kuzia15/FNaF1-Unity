using UnityEngine;
using System.Collections;

public class Typing : MonoBehaviour {

	[TextArea(3,10)]
	public string text;
	public float speed = 0.5f;
	public TextMesh tm1;
	public TextMesh tm2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		int l = (int)(Time.time * speed);
		if(l>text.Length) l = text.Length;
		string out_ = text.Substring(0,l);
	
		tm1.text = out_;
		tm2.text = out_;
	}
}
