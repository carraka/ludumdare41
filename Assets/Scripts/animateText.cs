using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class animateText : MonoBehaviour {

	private GameManager gameManager;
	public Text storyText;
	private float fadeInTime = 0.025F;
	private string str;

	bool printingText = true;

	void Awake (){
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}
	void Update(){
		if (Input.GetButtonDown ("Fire1")) {
			//Debug.Log ("clicked");
			if (printingText) {
				printingText = false;


			} 
		}//if click
	}




	public IEnumerator animate(string strComplete){
		printingText = true;
		int i = 0;
		storyText.text = "";
		storyText.enabled = true;
		while( i < strComplete.Length && printingText){
			storyText.text += strComplete[i++];
			yield return new WaitForSeconds(fadeInTime);
		}
		storyText.text = strComplete;
		printingText = false;
	}
}
