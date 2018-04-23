using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

	private static bool created = false;
	private static DontDestroyOnLoad thisAudio = null;

	void Awake()
	{
		if (!created)
		{
			DontDestroyOnLoad (this.gameObject);
			created = true;
			thisAudio = this;
		}
		else{
			Destroy (this.gameObject);
			thisAudio.Start ();
		}
	}

	// Use this for initialization
	public void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
