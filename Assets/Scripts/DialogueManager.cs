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

	public string dialogue, loveExpression;
	private int lineNum;

	public int NPCNum;
	public int spyNum;
	public int loveNum;
	public int withdrawNum;
	private int chickenNum;

	string[] options;
	string[] checks;

	List <Button> buttons = new List <Button>();

	private Text spyText;
	private Text loveText;
	private Text NPCText;
	private Image characterArt;
	private Image dialogueBox;
	private Image textBG;
	private Image background;
	private Image loveInterestImg;
	private Image speechBubble;
	private Image returnMenuButton;
	private Image restartButton;

	private Text activeText;
	private string tempCode;
	private string tempEnding;

	public bool inCheck = false;
	public bool gameOver = false;
	public bool waiting = false;

    public GameManager.GameDirection nextDirection;
    public bool cluck;


    private int lineNum;

	void Awake()
	{
		spyText = GameObject.Find ("SpyText").GetComponent<Text> ();
		loveText = GameObject.Find ("LoveText").GetComponent<Text> ();
		NPCText = GameObject.Find ("NPCText").GetComponent<Text> ();
		dialogueBox = GameObject.Find ("DialogueBox").GetComponent<Image>();
		textBG = GameObject.Find ("TextBG").GetComponent<Image>();
		background = GameObject.Find ("BackgroundImg").GetComponent<Image> ();
		loveInterestImg = GameObject.Find ("LoveInterest").GetComponent<Image> ();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		speechBubble = GameObject.Find ("SpeechBubble").GetComponent<Image> ();
		returnMenuButton = GameObject.Find ("ReturnMenuButton").GetComponent<Image> ();
		restartButton = GameObject.Find ("RestartButton").GetComponent<Image> ();

	/*	bossSound = GameObject.Find ("Boss").GetComponent<AudioSource> ();
		customerSound = GameObject.Find ("Caller").GetComponent<AudioSource> ();
		catSound = GameObject.Find ("Cat").GetComponent<AudioSource> ();
*/
	}
	void Start () {

		dialogue = "";
		loveExpression = "";
		parser = GameObject.Find ("Dialogue").GetComponent<DialogueParser> ();
		NPCNum = 0;
		spyNum = 1;
		loveNum = 1;
		withdrawNum = 1;
		chickenNum = 0;
		tempCode = "";

	}
	IEnumerator wait_a_bit(){
		yield return new WaitForSeconds (1f);
	}

	void Update () {
		
/*		if (Input.GetKeyDown (KeyCode.Space))
			StartCoroutine("AdvanceDialogue", "NPC");*/

		if (gameManager.nextDirection == GameManager.GameDirection.spy){
			StartCoroutine("AdvanceDialogue", "spy");
			gameManager.nextDirection = GameManager.GameDirection.none;

		}

		if (gameManager.nextDirection == GameManager.GameDirection.romance)
		{
			StartCoroutine("AdvanceDialogue", "love");
			gameManager.nextDirection = GameManager.GameDirection.none;
		}

	}
		
	void ClearTalkingSounds()
	{
		/*	customerSound.Stop ();
		catSound.Stop ();
		bossSound.Stop ();*/
	}

	public IEnumerator AdvanceDialogue(string lines)
	{

		Debug.Log ("tempCode: " + tempCode);
		switch (lines)
		{
		case "NPC":
			parser.activeLines = parser.NPCLines;
			NPCNum = parser.SearchStory (tempCode);
			lineNum = NPCNum;

			ParseLine (false);
			UpdateUI(NPCText);
			break;

		case "spy":
			StartCoroutine ("FlashText", spyText);
			yield return new WaitForSeconds (0.6f);
			gameManager.endingCode = tempEnding;
			//Debug.Log ("after tempEnding");
			parser.activeLines = parser.spyLines;
			lineNum = spyNum;
			ParseLine (true);

			//Debug.Log ("after parsing line");

			if (spyNum == 0)
			{
				spyNum++;

				tempCode = parser.GetKey (spyNum - 1);
			}
			else
			{
				tempCode = parser.GetKey (spyNum - 1);

				spyNum++;
			}


			Debug.Log ("after getting key");
			if (gameManager.checks >= 4) {
				StartCoroutine ("DatingSimEndGame");
				yield break;
			}

			UpdateUI (spyText);
			Debug.Log ("after updating UI");

			StartCoroutine("AdvanceDialogue", "NPC");
			//gameManager.checks++;

			break;

		case "love":
			StartCoroutine ("FlashText", loveText);
			yield return new WaitForSeconds (0.6f);
			gameManager.endingCode = tempEnding;

			parser.activeLines = parser.loveLines;
			lineNum = loveNum;
			tempCode = parser.GetKey (loveNum-1);
			//Debug.Log ("AFTER LOVE, tempCode: " + tempCode);
			ParseLine (true);

			if (gameManager.checks >= 4)
			{
				StartCoroutine("DatingSimEndGame");
				yield break;
			}

			UpdateUI (loveText);
			loveNum++;

			StartCoroutine ("AdvanceDialogue", "withdraw");
			StartCoroutine ("AdvanceDialogue", "NPC");

			//gameManager.checks++;
			break;

		case "withdraw":
			parser.activeLines = parser.withdrawLines;
			lineNum = withdrawNum;
			ParseLine (false);

			UpdateUI (spyText);
			withdrawNum++;
			break;
		
		case "chicken":
			parser.activeLines = parser.chickenLines;
			lineNum = chickenNum;
			ParseLine (false);
			UpdateUI (NPCText);
			chickenNum++;
			break;

		default:
			Debug.Log ("Error: No such type of dialogue lines: " + lines);
			break;

		}

	}
	void ParseLine(bool watchEnding){
		
		//Debug.Log ("we are at line " + lineNum + " with this content: " + parser.GetContent (lineNum));

		loveExpression = parser.GetExpression (lineNum);

		var text = parser.GetContent (lineNum);


		if (text.Contains ("~")) {
			tempEnding = text.Split('~')[1];
			text = text.Split ('~') [0];
		}
		else if (watchEnding)
		{
			tempEnding = null;

		}


		dialogue = text;

	}

	void UpdateUI(Text activeText){

		if (dialogue != "over")
			activeText.text = dialogue;
		//else if (dialogue != "THE END")
		//dialogueBox.text = "The next day";
		//gameManager.animateStory (dialogue);
		//nameBox.text = loveExpression;
		//characterArt.sprite = Resources.Load<Sprite> ("Sprites/" + loveExpression + "_avatar");

		ClearTalkingSounds ();

		UpdateLoveExpression ();


	}//UpdateUI

	private void UpdateLoveExpression()
	{
		if (loveExpression != null)
		{
			//AudioSource talkingSound;
			//talkingSound = GameObject.Find (loveExpression).GetComponent<AudioSource> ();
			//talkingSound.Play ();

			loveInterestImg.sprite = Resources.Load<Sprite>("Sprites/LoveInterest/li_" + loveExpression);
		}
		else
		{
			loveInterestImg.sprite = Resources.Load<Sprite>("Sprites/LoveInterest/li_Neutral");

		}
	}

	IEnumerator BriefDisgust()
	{
		bool disgusted = false;

		disgusted = true;
		string tempText = NPCText.text;
		Sprite tempSprite = loveInterestImg.sprite;
		NPCText.text = "Want a cough drop?";
		loveInterestImg.sprite = Resources.Load<Sprite> ("Sprites/LoveInterest/li_Disgusted");
		yield return new WaitForSeconds (2f);

		NPCText.text = tempText;
		loveInterestImg.sprite = tempSprite;
	}

	IEnumerator DatingSimEndGame()
	{
		parser.activeLines = parser.NPCLines;

		//Debug.Log ("===HERE IS THE TEMP CODE: " + tempCode);
		int t = parser.SearchStory (tempCode);

		NPCText.text = parser.NPCLines [t].content;
		loveExpression = parser.NPCLines [t].expression;
		UpdateLoveExpression ();
		CloseDialogueBox ();
		yield return new WaitForSeconds (3f);

		t = parser.SearchStory (gameManager.endingCode);
		NPCText.text = parser.NPCLines[t].content;
		loveExpression = parser.NPCLines [t].expression;


	}

	private void CloseDialogueBox()
	{
		spyText.enabled = false;
		loveText.enabled = false;
		textBG.enabled = false;

	}

	public void ChangeToDatingColors()
	{
		textBG.enabled = true;
		dialogueBox.color = new Color32(255, 79, 122,255);
		UpdateStartingText ();
		NPCText.enabled = true;
		spyText.enabled = true;
		loveText.enabled = true;
		loveInterestImg.enabled = true;
		speechBubble.enabled = true;
		returnMenuButton.sprite = Resources.Load <Sprite>("Sprites/UI/spyguy_button_pink");
		restartButton.sprite = Resources.Load <Sprite>("Sprites/UI/spyguy_button_pink");


		//background = somenewbackground;

	}

	public void UpdateStartingText()
	{
		NPCText.text = parser.NPCLines [0].content;
		spyText.text = parser.spyLines [0].content;
		loveText.text = parser.loveLines [0].content;
	}

	public IEnumerator FlashText(Text text)
	{
		//Debug.Log ("flashing");
		Color tempColor = text.color;
		for (int i = 0; i < 3; i++)
		{
			text.color = new Color32 (255, 255, 255, 255); 
			yield return new WaitForSeconds (0.1f);
			text.color = tempColor;
			yield return new WaitForSeconds (0.1f);
		}
		//Debug.Log ("finished flashing");

	}
}
