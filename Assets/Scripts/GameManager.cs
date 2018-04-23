﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    public enum GameDirection { none, spy, chicken, romance };
    public GameDirection nextDirection;
    public bool cluck;

    private AudioSource romanceMusic;
    private AudioSource spyMusic;

    private static GameManager thisGM = null;

    public struct replayFrame
    {
        public float beat;
        public bool cluck;
        public GameDirection nextDirection;
    }
    public bool replayMode;
    private int replayFrameNumber;
    private List<replayFrame> replayQueue;
    
	void OnLevelWasLoaded()
	{
		Debug.Log ("loaded");

		if (SceneManager.GetActiveScene().name == "Level 1")
		{
			dm = GameObject.Find("Dialogue").GetComponent<DialogueManager>();
			spySlider = GameObject.Find("SpySlider").GetComponent<Slider>();
			loveSlider = GameObject.Find("LoveSlider").GetComponent<Slider>();
			player = GameObject.Find("Player");
			playerController = player.GetComponent<PlayerController>();

			endingCode = "failNeutral";

			rhythmUI = GameObject.Find("RhythmUI").GetComponent<RhythmUI>();
			questionMark = GameObject.Find("QuestionMark").GetComponent<QuestionMark>();
			romanceMusic = GameObject.Find("RomanceMusic").GetComponent<AudioSource>();
			spyMusic = GameObject.Find("SpyMusic").GetComponent<AudioSource>();

			thisGM.Start ();
		}

	}
    void Awake()
    {
        if (thisGM != null)
        {
            Destroy(this.gameObject);
           // thisGM.Awake();
            Debug.Log("destroyed duplicate gameManager");
			return;
        }
        else
        {
            thisGM = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("created and protected gameManager");
        }



        replayMode = false;
    }

    // Use this for initialization
    void Start()
    {
        romanceMusic.Play();
        spyMusic.Play();

        spyMusic.volume = .75f;
        romanceMusic.volume = .75f;

        rhythmUI.StartSong(replayMode);

        nextDirection = GameDirection.none;
        cluck = false;

        checks = 0;

        replayFrameNumber = 0;
        if (replayMode)
            replayQueue = new List<replayFrame>();

        endingCode = "succeedSpy";
	
//		SwitchToDatingSimMode ();
//		checks++;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (checks >= 5)
		{
			dm.EnablePressSpaceText ();
		}

        if (replayMode)
        {
            if (replayFrameNumber < replayQueue.Count && rhythmUI.timeToBeat(Time.time) > replayQueue[replayFrameNumber].beat)
            {
                cluck = replayQueue[replayFrameNumber].cluck;
                nextDirection = replayQueue[replayFrameNumber].nextDirection;

                replayFrameNumber++;

            }
        }else
        {
            if (cluck || nextDirection != GameDirection.none)
            {
                replayFrame frameData = new replayFrame();
                frameData.beat = rhythmUI.timeToBeat(Time.time);
                frameData.cluck = cluck;
                frameData.nextDirection = nextDirection;

				replayQueue.Add (frameData);
            }
        }

        if (datingSimMode == true)
        {
            dm.cluck = cluck;
            cluck = false;

            dm.nextDirection = nextDirection;
            nextDirection = GameDirection.none;
            return;
        }


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
                        playerController.pushAction(PlayerController.Movement.action.move, 1.5f, new Vector2(0, 1));
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
			endingCode = "failNeutral";

            spyMusic.volume = 0f;
            romanceMusic.volume = 1f;

            datingSimMode = true;
            dm.ChangeToDatingColors();

            Destroy(player);
        }
    }

    public void EndGame()
    {

    }
}
