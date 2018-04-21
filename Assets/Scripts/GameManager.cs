using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public int love = 0;
	public int spy = 0;

	private Slider spySlider;
	private Slider loveSlider;

	void Awake (){

		spySlider = GameObject.Find ("SpySlider").GetComponent<Slider> ();
		loveSlider = GameObject.Find ("LoveSlider").GetComponent<Slider> ();

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
