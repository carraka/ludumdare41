using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour {

	private GameManager gameManager;
	private Image illustration;
	private Text endingText;



	// Use this for initialization
	void Awake () {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		illustration = GameObject.Find ("endingIllustration").GetComponent<Image> ();
		endingText = GameObject.Find ("endingText").GetComponent<Text> ();
	}

	void Start()
	{
		AudioSource audio = gameObject.AddComponent < AudioSource > ();

		switch (gameManager.endingCode)
		{
		case "neutralFail":
		case "failLove":
		case "failSpy":
			audio.PlayOneShot ((AudioClip)Resources.Load ("Audio/Music/ld41_ending_chicken"));
			illustration.sprite = Resources.Load<Sprite> ("Sprites/Endings/chicken_ending");
			endingText.text = "Agent Tree, you have failed in your mission. Chickens have INVADED and IMPLODED our world. But we should not be surprised by your failure. Apparently you were a CHICKEN all along.";
			break;

		case "perfSpy":
		case "succeedSpy":
			illustration.sprite = Resources.Load<Sprite>("Sprites/Endings/lonely_ending");
			audio.PlayOneShot ((AudioClip)Resources.Load ("Audio/Music/ld41_savetheworld"));
			endingText.fontSize = 25;
			endingText.text = "Your mission was a SUCCESS. With the help of the DOSSIER you retrieved, we averted the enemy's plan to IMPLODE the world. You avoided  distractions with AGILITY. You have demonstrated that ROMANCE is INCOMPATIBLE with sneaking. You are to be COMMENDED for your LONELINESS.";
			break;

		case "succeedLove":
		case "perfLove":
			illustration.sprite = Resources.Load<Sprite>("Sprites/Ending/lover_ending");
			audio.PlayOneShot ((AudioClip)Resources.Load ("Audio/Music/ld41_ending_romantic"));
			endingText.text = "You ABANDONED your mission. In doing so, you abandoned the WORLD. Love, it seems, is INCOMPATIBLE with spying. Enjoy your remaining hours, for you guaranteed IMPLOSION for us all.";
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
