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

    public enum GameDirection { none, spy, chicken, romance};
    public GameDirection nextDirection;
    public bool cluck;

    private static bool GMcreated = false;

	void Awake ()
    {
        if (GMcreated)
        {
            Destroy(this.gameObject);
            Debug.Log("destroyed duplicate game manager");
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("Game manager loaded and protected");
            GMcreated = true;
        }

        dm = GameObject.Find ("Dialogue").GetComponent<DialogueManager> ();
		spySlider = GameObject.Find ("SpySlider").GetComponent<Slider> ();
		loveSlider = GameObject.Find ("LoveSlider").GetComponent<Slider> ();
		player = GameObject.Find ("Player");

        nextDirection = GameDirection.none;
        cluck = false;

        endingCode = "failNeutral";
	}

	// Use this for initialization
	void Start () {
//		SwitchToDatingSimMode ();
//		checks++;
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
                // activate question mark
                cluck = false;
            }
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
                    // activate question mark

                }
            }

            if (nextDirection == GameDirection.romance)
                SwitchToDatingSimMode();

            if (nextDirection == GameDirection.spy)
            {
                switch (checks)
                {
                    case 1: //move to cubicle
                        break;
                    case 2:
                        // duck behind file cabinet
                        break;
                    case 3:
                        // move to other cubicle
                        break;
                    case 4:
                        //duck behind potted plant
                        break;
                    case 5:
                        //move to file cabinet
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
            datingSimMode = true;
            dm.ChangeToDatingColors();
        }
	}

	public void EndGame(){
		
	}
}
