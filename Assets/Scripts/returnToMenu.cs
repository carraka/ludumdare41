using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class returnToMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void click(){
		//Debug.Log ("credit button clicked");
		SceneManager.LoadScene ("title");

	}
}
