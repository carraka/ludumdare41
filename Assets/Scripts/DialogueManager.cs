using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {
	//public string name = "dia man!!";
	private DialogueParser parser;
	private storyManager storyManager;
	public gameManager gameManager;
	private Phone phone;
	private RatingSystemManager rm;

	private AudioSource customerSound;
	private AudioSource bossSound;
	private AudioSource catSound;

	//public mapTracker mapTracker;
	public roomScript roomScript;
	public Button[] button;

	public string dialogue, characterName;
	public int lineNum;
	string[] options;
	string[] checks;
	//public bool inChoice;
	List <Button> buttons = new List <Button>();

	public Text dialogueBox;
	public Text nameBox;
	public GameObject choiceBox;
	public Image characterArt;

	bool firstScreen = true;
	public bool inCheck = false;
	bool debugging = false;
	public bool gameOver = false;
	public bool waiting = false;
	//Image illustration;

	private string[] visitors = new string[] {"penguin", "mouse", "police","giraffe", "weregiraffe", "emptyroom", "nurse"};

	// Use this for initialization

	void Awake()
	{
		storyManager = GameObject.Find ("gameManager").GetComponent<storyManager> ();
		phone = GameObject.Find ("Phone").GetComponent<Phone> ();
		rm = GameObject.Find ("Rating System").GetComponent<RatingSystemManager> ();

		bossSound = GameObject.Find ("Boss").GetComponent<AudioSource> ();
		customerSound = GameObject.Find ("Caller").GetComponent<AudioSource> ();
		catSound = GameObject.Find ("Cat").GetComponent<AudioSource> ();

	}
	void Start () {
		//Debug.Log ("DiaMgr STARTING");
		//illustration =GameObject.Find ("illustration").GetComponent<Image> ();
//		characterArt = GameObject.Find ("characterArt").GetComponent<Image> ();
		dialogue = "";
		characterName = "";
		//inChoice = false;
		parser = GameObject.Find ("Dialogue Parser").GetComponent<DialogueParser> ();
		Debug.Log ("Setting linenum back to zero" + lineNum);
		lineNum = 0;


	}
	IEnumerator wait_a_bit(){
		yield return new WaitForSeconds (1f);
	}
	/*public void run_all_lines() {
		debugging = true;
		while (!gameOver) {
			Debug.Log ("running all lines");
			if (gameManager.delayedLineCommand) {
				Debug.Log ("releasing delayed line command");
				gameManager.runLineCommand (gameManager.commandModifier);
				gameManager.delayedLineCommand = false;

			} else {
				Debug.Log ("updating dialogue after click");
				UpdateDialogue (true);
			}	
			if (lineNum > 140) {
				Debug.Log ("at line 200");
				break;
			}
			Debug.Log ("waiting");
			StartCoroutine (wait_a_bit());
			Debug.Log ("done waiting");
		}
		Debug.Log ("compile success");
	}*/
	
	// Update is called once per frame
	void Update () {
		if (debugging)
			return;
		if (Input.GetKeyDown("space") && !gameManager.inChoice && gameManager.inStory && !inCheck && !waiting)
		{
			if (gameManager.delayedLineCommand)
			{
				Debug.Log ("releasing delayed line command");
				gameManager.runLineCommand (gameManager.commandModifier);
				gameManager.delayedLineCommand = false;

			}
			else
			{
				Debug.Log ("updating dialogue after click");
				UpdateDialogue (true);
			}
		}

	}


	public void ShowDialogue(){
		//ResetImages ();
		ParseLine ();

		// check flags
		//check_for_visitor();
	}

	void ResetImages(){
		if (characterName != ""){
			GameObject character = GameObject.Find (characterName);
			SpriteRenderer currSprite = character.GetComponent<SpriteRenderer> ();
			currSprite.sprite = null;
		}
	}//Reset Images

	void ParseLine(){
		Debug.Log ("Parsing line: " + (lineNum + 1));
		//Determine which list

		if (gameManager.gainingSkill)
		{

			parser.activeList = parser.skillLines;
			if (parser.GetKey(lineNum) != "" && !firstScreen)
			{
				gameOver = true;
				gameManager.ExitAdventure ();
				firstScreen = true;
				return;
			}
			firstScreen = false;
		}
		else
		{
			parser.activeList = parser.lines;
		}

		Debug.Log ("we are at line " + lineNum + " with this content: " + parser.GetContent (lineNum));
		if (parser.GetKey(lineNum).Contains("Check"))
		{
			Debug.Log ("In check");
			inCheck = true;
			checks = parser.GetOptions (lineNum);
			Debug.Log ("LINE NUM: " + lineNum);
			Debug.Log ("CHECKS");
			Debug.Log ("========================");
			foreach (var check in checks) {
				Debug.Log ("check:  [" + check + "]");
			}
			Debug.Log ("========================");
			for (int i = 0; i< checks.Length; i++)
			{
				Debug.Log ("LINE NUM: " + lineNum + "and i=" + i);
				string entireCheck = checks [i];
				Debug.Log ("Entire check: " + entireCheck);

				if (entireCheck == null) {
					Debug.Log ("entireCheck == null so breaking");
					break;
				}
				if (entireCheck.Contains("~"))//if there is a check
				{
					string allChecks = entireCheck.Split ('~') [0];
					string lineCommand = entireCheck.Split ('~') [1];

					if (entireCheck.Contains("*"))					//if there are multiple checks
					{
						bool pickThisLine = true;
						string[] statcheck = (allChecks.Split ('*'));
						for (int j = 0; j < statcheck.Length; j++)
						{
							if (!storyManager.checkStat (statcheck [j]))
							{
								pickThisLine = false;

							}

						}

						if (pickThisLine)
						{
							gameManager.runLineCommand (lineCommand);
							inCheck = false;
							Debug.Log ("1. decrement linenum: " + lineNum);
							lineNum--;
							return;
						}
					}//if multiple checks
					else
					{
						Debug.Log (allChecks + " is: " + storyManager.checkStat (allChecks));
						if (storyManager.checkStat(allChecks))
						{
							gameManager.runLineCommand(lineCommand);
							Debug.Log ("2. decrement linenum: " + lineNum);
							lineNum--;
							inCheck = false;
							return;
						}

					}

				}//if there is a single check

				else 
				{
					gameManager.runLineCommand (entireCheck);
					Debug.Log ("3. decrement linenum: " + lineNum);
					lineNum--;
					inCheck = false;
					return;
				}
			}
		}
		else if (parser.GetKey (lineNum) != "Choice" && parser.GetKey (lineNum) != "endChoice"){
			gameManager.inChoice = false;
			characterName = parser.GetSpeaker (lineNum);

			var text = parser.GetContent (lineNum);
			//Debug.Log ("In not choice");
			//if dialogue contains commands
			if (text == "over" && !gameManager.inChoice)
			{
				Debug.Log ("over called in DM");
				//Debug.Log ("adding satisfaction of " + phone.lines [phone.activeLine].call.satisfaction);
				rm.AddRating (phone.lines [phone.activeLine].call.satisfaction);
				phone.CloseLine(phone.activeLine);
				CloseStoryBox ();
				return;
			}
			else if (text == "...")
			{
				waiting = true;
				return;
			}
			else if (text=="resume")
			{
				lineNum = phone.lines [phone.activeLine].call.currentLine;
				ShowDialogue ();
				UpdateUI ();
				//lineNum++;
			}

				dialogue = text;


			if (dialogue.Contains("~"))
			{
				Debug.Log ("Commands: " + dialogue.Split ('~') [1]);

				gameManager.parseOption (dialogue.Split ('~') [1]);
				dialogue = dialogue.Split ('~') [0];

			}


		//	DisplayImages ();
		}
		else {
			Debug.Log ("In else so assume choice: " + lineNum);
			gameManager.inChoice = true;
			characterName = "";
			dialogue = "";
			options = parser.GetOptions (lineNum);
			Debug.Log ("========================");
			foreach (var option in options) {
				Debug.Log ("option:  [" + option + "]");
			}
			Debug.Log ("========================");
			CreateButtons ();
		}
	}

	void DisplayImages(){
		if (characterName != "")
		{
			//GameObject character = GameObject.Find (characterName);
			//SetSpritePositions(character);
//			SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
			//currSprite.sprite = character.GetComponent<Character>().charactersPoses[pose];
		}
	}

	void CreateButtons(){
		Debug.Log ("Choice------------------------");
		if (options.Length > 4)
			Debug.Log ("ERROR: MORE THAN FOUR OPTIONS");
		int b = 0;
		for (int i = 0; i < options.Length; i++)
		{
			ChoiceButton cb = button[b].GetComponent<ChoiceButton> ();
			string buttonText = options [i].Split ('~') [0];
			Debug.Log (buttonText);


			if (buttonText.Contains("*"))
			{
				bool showButton = true;
				string allChecks = buttonText.Split ('~') [0];


				//string statCheck = allChecks.Split ('*') [0];

				string[] statCheck = allChecks.Split('*');

				for (int j = 0; j < statCheck.Length; j++)
				{
					if (storyManager.checkStat(statCheck[j]))
					{
						Debug.Log (statCheck[j] + ":true");
					} else 
					{
						
						Debug.Log(statCheck[j] + ":false");	
						showButton = false;
					}

				}

				if (showButton)
				{
					roomScript.choiceText [b].text = options[i].Split ('~') [1];
					Debug.Log ("roomScript.choiceText [b].text=" + roomScript.choiceText [b].text);
					roomScript.choiceText [b].enabled = true;
					//cb.option = options [i].Split ('~') [2];
					cb.SetOption(options [i].Split ('~') [2]);
					b++;
				}//if button passes all statchecks

					
			}
			else
			{
				if (options [i].Length > 0) {
					roomScript.choiceText [b].text = options [i].Split ('~') [0]; 
					roomScript.choiceText [b].enabled = true;
					Debug.Log ("options [i]=" + options [i]);
					cb.SetOption (options [i].Split ('~') [1]);
					b++;
				} else {
					Debug.Log ("WARNING:  The option is empty");
				}

			}

			//Debug.Log (cb.option);
			//Debug.Log ("Button created");
		}

	}//CreateButtons

	void UpdateUI(){
		//Debug.Log ("updating UI: " + dialogue);
		if (!gameManager.inChoice){
			ClearButtons();
			//Debug.Log ("Buttons cleared");
		}
		if (dialogue != "over" && dialogue != "resume")
			dialogueBox.text = dialogue;
		//else if (dialogue != "THE END")
			//dialogueBox.text = "The next day";
		//gameManager.animateStory (dialogue);
		nameBox.text = characterName;
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
		customerSound.Stop ();
		catSound.Stop ();
		bossSound.Stop ();
	}

	void ClearButtons(){
		for (int i = 0; i < buttons.Count; i++){
			Button b = buttons[i];
			buttons.Remove(b);
			Destroy(b.gameObject);
		}
	}
	public void UpdateDialogue(bool lineIncrease)
	{
		if (!storyManager.choiceValid)
		{
			Debug.Log ("error: Choice invalid");
			return;
		}
		ShowDialogue ();
		if (lineIncrease)
		{
			lineNum++;

		}	

		if (gameManager.inStory)
		{
			UpdateUI();
		}
	}

	public void CloseStoryBox()
	{
		roomScript.hideChoices ();
		dialogueBox.enabled = false;
		nameBox.enabled = false;
		//characterArt.enabled = false;
		gameManager.inStory = false;
		gameManager.inChoice = false;
		inCheck = false;
		waiting = false;

	}
}
