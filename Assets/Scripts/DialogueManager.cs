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

	public enum Lines {NPC, spy, love, withdraw, chicken};

	public string tempDialogue, loveExpression;
	private string tempCode, tempEnding, storedEnding;

	public int NPCNum, spyNum, loveNum, withdrawNum, chickenNum;

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

	public bool inCheck = false;
	public bool gameOver = false;
	public bool waiting = false;

    public GameManager.GameDirection nextDirection;
    public bool cluck;


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

		tempDialogue = "";
		loveExpression = "";
		parser = GameObject.Find ("Dialogue").GetComponent<DialogueParser> ();
		NPCNum = 0;
		spyNum = 0;
		loveNum = 1;
		withdrawNum = 1;
		chickenNum = 0;
		tempCode = "";

	}
	IEnumerator wait_a_bit(){
		yield return new WaitForSeconds (1f);
	}

	void Update () {
		
		if (Input.GetKeyDown (KeyCode.Space))
			StartCoroutine("AdvanceDialogue", Lines.chicken);

		if (gameManager.nextDirection == GameManager.GameDirection.spy || Input.GetKeyDown(KeyCode.LeftArrow)){
			StartCoroutine("AdvanceDialogue", Lines.spy);
			gameManager.nextDirection = GameManager.GameDirection.none;

		}

		if (gameManager.nextDirection == GameManager.GameDirection.romance|| Input.GetKeyDown(KeyCode.RightArrow))
		{
			StartCoroutine("AdvanceDialogue", Lines.love);
			gameManager.nextDirection = GameManager.GameDirection.none;
		}

	}
		
	void ClearTalkingSounds()
	{
		/*	customerSound.Stop ();
		catSound.Stop ();
		bossSound.Stop ();*/
	}

	public IEnumerator AdvanceDialogue(Lines lines)
	{

		Debug.Log ("tempCode: " + tempCode);
		switch (lines)
		{
		case Lines.NPC:
			parser.activeLines = parser.NPCLines;
			NPCNum = parser.SearchStory (tempCode);
			ParseLine (Lines.NPC);
			UpdateUI(NPCText);
			break;

		case Lines.spy:
			parser.activeLines = parser.spyLines;
			StartCoroutine ("FlashText", spyText);
			yield return new WaitForSeconds (0.6f);

			if (storedEnding == "spy")
				gameManager.endingCode = tempEnding;
			//Debug.Log ("after tempEnding");

			ParseLine (Lines.spy);
			//Debug.Log ("after parsing line");

			if (spyNum == 0)
				tempCode = "spy0";
			else
				tempCode = parser.GetKey (spyNum-1);

			spyNum++;


			//Debug.Log ("after getting key");
			if (gameManager.checks >= 4) {
				StartCoroutine ("DatingSimEndGame");
				yield break;
			}

			UpdateUI (spyText);
			//Debug.Log ("after updating UI");

			StartCoroutine("AdvanceDialogue", Lines.NPC);
			//gameManager.checks++;

			break;

		case Lines.love:
			parser.activeLines = parser.loveLines;
			StartCoroutine ("FlashText", loveText);
			yield return new WaitForSeconds (0.6f);

			if (storedEnding == "love")
				gameManager.endingCode = tempEnding;

			tempCode = parser.GetKey (loveNum - 1);
			//Debug.Log ("AFTER LOVE, tempCode: " + tempCode);
			ParseLine (Lines.love);

			if (gameManager.checks >= 4) {
				StartCoroutine ("DatingSimEndGame");
				yield break;
			}

			UpdateUI (loveText);
			loveNum++;

			StartCoroutine ("AdvanceDialogue", Lines.withdraw);
			StartCoroutine ("AdvanceDialogue", Lines.NPC);

			//gameManager.checks++;
			break;

		case Lines.withdraw:
			parser.activeLines = parser.withdrawLines;
			ParseLine (Lines.withdraw);
			UpdateUI (spyText);
			withdrawNum++;
			break;
		
		case Lines.chicken:
			parser.activeLines = parser.chickenLines;
			ParseLine (Lines.chicken);
			UpdateUI (NPCText);
			chickenNum++;
			break;

		default:
			Debug.Log ("Error: No such type of dialogue lines: " + lines);
			break;

		}

	}
	void ParseLine(Lines line){
		
		//Debug.Log ("we are at line " + lineNum + " with this content: " + parser.GetContent (lineNum));
		string text = "";
		bool tempEndingEnabled = false;
		bool instantEndingEnabled = false;

		switch (line)
		{
		case Lines.NPC:
			loveExpression = parser.GetExpression (NPCNum);
			text = parser.GetContent (NPCNum);
			break;

		case Lines.spy:
			text = parser.GetContent (spyNum);
			tempEndingEnabled = true;
			storedEnding = "spy";
			break;

		case Lines.love:
			text = parser.GetContent (loveNum);
			storedEnding = "love";
			break;

		case Lines.withdraw:
			text = parser.GetContent (withdrawNum);
			break;

		case Lines.chicken:
			loveExpression = parser.GetExpression (chickenNum);
			text = parser.GetContent (chickenNum);
			break;

		default: 
			Debug.Log ("====ERROR: NO SUCH ENUM LINES EXISTS");
			break;
		}

		//Split text by ~ if it contains ending instructions
		if (text.Contains ("~")) 
		{
			tempEnding = text.Split('~')[1];
			text = text.Split ('~') [0];

			//if ending should be changed as soon as dialgoue is accessed
			if (instantEndingEnabled)
			{
				gameManager.endingCode = tempEnding;
			}
		}
		else if (tempEndingEnabled) //if ending should only be saved for if the option is selected again
		{
			tempEnding = null;
		}


		tempDialogue = text;

	}

	void UpdateUI(Text activeText)
	{

		if (tempDialogue != "over")
			activeText.text = tempDialogue;
		
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

	public IEnumerator BriefDisgust()
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
