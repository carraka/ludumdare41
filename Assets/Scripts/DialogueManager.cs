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
	private Text NPCText;
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

	private int lineNum;
	//Image illustration;

	// Use this for initialization

	void Awake()
	{
		spyText = GameObject.Find ("SpyText").GetComponent<Text> ();
		loveText = GameObject.Find ("LoveText").GetComponent<Text> ();
		NPCText = GameObject.Find ("NPCText").GetComponent<Text> ();
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
		
		/*	if (Input.GetKeyDown (KeyCode.Space))
			AdvanceDialogue ("NPC");

		if (Input.GetKeyDown (KeyCode.LeftArrow))
			AdvanceDialogue ("spy");
		
		if (Input.GetKeyDown (KeyCode.RightArrow))
			AdvanceDialogue ("love");*/

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
			lineNum = NPCNum;
			parser.activeLines = parser.NPCLines;
			activeText = NPCText;
			ParseLine ();
			NPCNum++;
			break;

		case "spy":
			parser.activeLines = parser.spyLines;
			lineNum = spyNum;
			activeText = spyText;
			ParseLine ();
			spyNum++;
			break;

		case "love":
			lineNum = loveNum;
			parser.activeLines = parser.loveLines;
			activeText = loveText;
			ParseLine ();
			loveNum++;
			break;

		default:
			Debug.Log ("Error: No such type of dialogue lines: " + lines);
			break;

		}

		Debug.Log ("Parsing line: " + (lineNum));

		UpdateUI();

	}
	void ParseLine(){
		//Determine which list


		Debug.Log ("we are at line " + lineNum + " with this content: " + parser.GetContent (lineNum));

		if (parser.GetKey (lineNum) != "Choice" && parser.GetKey (lineNum) != "endChoice"){
			//gameManager.inChoice = false;
			characterName = parser.GetSpeaker (lineNum);

			var text = parser.GetContent (lineNum);
			//Debug.Log ("In not choice");
			//if dialogue contains commands
			if (text == "over")
			{
				Debug.Log ("over called in DM");
				//Debug.Log ("adding satisfaction of " + phone.lines [phone.activeLine].call.satisfaction);
				/*rm.AddRating (phone.lines [phone.activeLine].call.satisfaction);
				phone.CloseLine(phone.activeLine);
				CloseStoryBox ();*/
				return;
			}

			dialogue = text;

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

	public void ChangeToDatingColors()
	{
		textBG.enabled = true;
		dialogueBox.color = new Color32(255, 0, 195,255);
		spyText.enabled = true;
		loveText.enabled = true;
	}
}
