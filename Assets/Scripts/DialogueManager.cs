using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {

	private DialogueParser parser;
	private GameManager gameManager;

	/*private AudioSource customerSound;
	private AudioSource bossSound;
	private AudioSource catSound;*/

	public Button[] button;

	public string dialogue, characterName;
	public int NPCNum;
	public int spyNum;
	public int loveNum;

	string[] options;
	string[] checks;

	List <Button> buttons = new List <Button>();

	private Text spyText;
	private Text loveText;
	private Image characterArt;
	private Image dialogueBox;
	private Image textBG;

	private Text activeText;

//	public Text nameBox;
//	public GameObject choiceBox;


	//bool firstScreen = true;
	public bool inCheck = false;
	bool debugging = false;
	public bool gameOver = false;
	public bool waiting = false;
	//Image illustration;

	// Use this for initialization

	void Awake()
	{
		spyText = GameObject.Find ("SpyText").GetComponent<Text> ();
		loveText = GameObject.Find ("LoveText").GetComponent<Text> ();
		dialogueBox = GameObject.Find ("DialogueBox").GetComponent<Image>();
		textBG = GameObject.Find ("TextBG").GetComponent<Image>();

		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();

	/*	bossSound = GameObject.Find ("Boss").GetComponent<AudioSource> ();
		customerSound = GameObject.Find ("Caller").GetComponent<AudioSource> ();
		catSound = GameObject.Find ("Cat").GetComponent<AudioSource> ();
*/
	}
	void Start () {
		//Debug.Log ("DiaMgr STARTING");
		//illustration =GameObject.Find ("illustration").GetComponent<Image> ();
//		characterArt = GameObject.Find ("characterArt").GetComponent<Image> ();
		dialogue = "";
		characterName = "";
		parser = GameObject.Find ("Dialogue").GetComponent<DialogueParser> ();
		NPCNum = 0;
		spyNum = 0;
		loveNum = 0;


	}
	IEnumerator wait_a_bit(){
		yield return new WaitForSeconds (1f);
	}

	void Update () {
		/*if (debugging)
			return;
		if (Input.GetKeyDown("space")) UpdateDialogue (true);*/

	}
		
	void ResetImages(){
		if (characterName != ""){
			GameObject character = GameObject.Find (characterName);
			SpriteRenderer currSprite = character.GetComponent<SpriteRenderer> ();
			currSprite.sprite = null;
		}
	}//Reset Images


	void DisplayImages(){
		if (characterName != "")
		{
			//GameObject character = GameObject.Find (characterName);
			//SetSpritePositions(character);
			//SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
			//currSprite.sprite = character.GetComponent<Character>().charactersPoses[pose];
		}
	}



	void UpdateUI(){

		if (dialogue != "over")
			activeText.text = dialogue;
		//else if (dialogue != "THE END")
			//dialogueBox.text = "The next day";
		//gameManager.animateStory (dialogue);
		//nameBox.text = characterName;
		//characterArt.sprite = Resources.Load<Sprite> ("Sprites/" + characterName + "_avatar");

		ClearTalkingSounds ();

		if (characterName != "You" && characterName != "" && characterName != "Alex" && characterName != "Winter")
		{
			AudioSource talkingSound;
			talkingSound = GameObject.Find (characterName).GetComponent<AudioSource> ();
			talkingSound.Play ();
		}


	}//UpdateUI

	void ClearTalkingSounds()
	{
		/*	customerSound.Stop ();
		catSound.Stop ();
		bossSound.Stop ();*/
	}

	public void AdvanceDialogue(string lines)
	{
		switch (lines)
		{
		case "NPC":
			NPCNum++;
			break;
		case "spy":
			spyNum++;
			break;
		case "love":
			loveNum++;
			break;
		default:
			Debug.Log ("Error: No such type of dialogue lines: " + lines);
			break;

		}

		UpdateUI();

	}

	public void ChangeToDatingColors()
	{
		textBG.enabled = true;
		dialogueBox.color = new Color32(255, 0, 195,255);
		spyText.enabled = true;
		loveText.enabled = true;
	}
}
