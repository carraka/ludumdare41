using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private DialogueManager dm;

	public bool datingSimMode = false;

	public string endingCode;

	public int love = 0;
	public int spy = 0;

	public int checks = 0;

    private bool annoyedNPC = false;

	private Slider spySlider;
	private Slider loveSlider;
	private GameObject player;
    private PlayerController playerController;
    private RhythmUI rhythmUI;
    private QuestionMark questionMark;

    public enum GameDirection { none, spy, chicken, romance};
    public GameDirection nextDirection;
    public bool cluck;

    private AudioSource romanceMusic;
    private AudioSource spyMusic;

    public static bool GMCreated = false;

    void Awake ()
    {
        if (GMCreated)
        {
            Destroy(this.gameObject);
            Debug.Log("destroyed duplicate gameManager");
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            GMCreated = true;
            Debug.Log("created and protected gameManager");
        }

        dm = GameObject.Find ("Dialogue").GetComponent<DialogueManager> ();
		spySlider = GameObject.Find ("SpySlider").GetComponent<Slider> ();
		loveSlider = GameObject.Find ("LoveSlider").GetComponent<Slider> ();
		player = GameObject.Find ("Player");
        playerController = player.GetComponent<PlayerController>();

        endingCode = "failNeutral";

        rhythmUI = GameObject.Find("RhythmUI").GetComponent<RhythmUI>();
        questionMark = GameObject.Find("QuestionMark").GetComponent<QuestionMark>();

        romanceMusic = GameObject.Find("RomanceMusic").GetComponent<AudioSource>();
        spyMusic = GameObject.Find("SpyMusic").GetComponent<AudioSource>();

    }

	// Use this for initialization
	void Start () {
        romanceMusic.Play();
        spyMusic.Play();

        spyMusic.volume = .75f;
        romanceMusic.volume = .75f;

        rhythmUI.StartSong();

        nextDirection = GameDirection.none;
        cluck = false;

        checks = 0;

    }

    // Update is called once per frame
    void Update ()
    {
        if (datingSimMode == true)
            return;

        if (cluck)
        {
            if (annoyedNPC == true)
                SwitchToDatingSimMode();
            else
            {
                annoyedNPC = true;
                questionMark.Appear();
            }
            cluck = false;
        }

		if (nextDirection != GameDirection.none)
        {
            checks++;

            if (nextDirection == GameDirection.chicken)
            {
                if (annoyedNPC == true)
                    SwitchToDatingSimMode();
                else
                {
                    annoyedNPC = true;
                    questionMark.Appear();
                    nextDirection = GameDirection.spy;
                }
            }

            if (nextDirection == GameDirection.romance)
                SwitchToDatingSimMode();

            if (nextDirection == GameDirection.spy)
            {
                switch (checks)
                {
                    case 1: //move to cubicle
                        spyMusic.volume = 1f;
                        romanceMusic.volume = 0f;

                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(1, 3));
                        playerController.pushAction(PlayerController.Movement.action.crouchRight, 5);
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(1, 4));
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(2, 4));
                        playerController.pushAction(PlayerController.Movement.action.crouchRight, 5);
                        break;
                    case 2:                         // duck behind file cabinet
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(3, 4));
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(3, 3));
                        playerController.pushAction(PlayerController.Movement.action.move, 4, new Vector2(5, 3));
                        playerController.pushAction(PlayerController.Movement.action.crouchLeft, 5);
                        break;
                    case 3:                        // move to other cubicle
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(4, 3));
                        playerController.pushAction(PlayerController.Movement.action.move, 4, new Vector2(4, 1));
                        playerController.pushAction(PlayerController.Movement.action.move, 4, new Vector2(5, 1));
                        playerController.pushAction(PlayerController.Movement.action.crouchLeft, 5);
                        break;
                    case 4:                        //duck behind potted plant
                        playerController.pushAction(PlayerController.Movement.action.move, 6, new Vector2(2, 1));
                        playerController.pushAction(PlayerController.Movement.action.wait, .5f);
                        playerController.pushAction(PlayerController.Movement.action.move, 1, new Vector2(4, 1));
                        playerController.pushAction(PlayerController.Movement.action.move, .5f, new Vector2(4, 0));
                        playerController.pushAction(PlayerController.Movement.action.crouchLeft, 1);
                        break;
                    case 5:                        //move to file cabinet
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(4, 1));
                        playerController.pushAction(PlayerController.Movement.action.move, 2, new Vector2(0, 1));
                        playerController.pushAction(PlayerController.Movement.action.crouchRight, 0);
                        break;
                }
            }

            nextDirection = GameDirection.none;
        }
	}

	void SwitchToDatingSimMode()
    {
        if (datingSimMode == false)
        {
            spyMusic.volume = 0f;
            romanceMusic.volume = 1f;

            datingSimMode = true;
            dm.ChangeToDatingColors();
        }
    }

	public void EndGame(){
		
	}
}
