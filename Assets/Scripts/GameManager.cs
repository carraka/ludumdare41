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

	private Slider spySlider;
	private Slider loveSlider;
	private GameObject player;

    public enum GameDirection { none, spy, chicken, romance};
    public GameDirection nextDirection;
    public bool cluck;


	void Awake (){
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
		SwitchToDatingSimMode ();
		checks++;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SwitchToDatingSimMode(){
		datingSimMode = true;
		dm.ChangeToDatingColors ();
	}

	public void EndGame(){
		
	}
}
