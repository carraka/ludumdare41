using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class load_game : MonoBehaviour {

	public bool instructionsOpen = false;
	public Image blackbg;
	public Image whiteborder;
	public Text actualinstructions;

	//private AudioSource buttonSound;

	private AudioSource menuMusic;

	// Use this for initialization
	void Awake(){
		//buttonSound = GameObject.Find ("ButtonSound").GetComponent<AudioSource> ();
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void load_level() {
		if (SceneManager.GetActiveScene().name == "title")
		{
			SceneManager.LoadScene ("tutorial");
		}
		else
		{
			//buttonSound.Play ();
			SceneManager.LoadScene("Level 1");
			GameObject.Find ("mainmenu_loop").GetComponent<AudioSource> ().Stop ();
		}

		
	}
	public void load_credits() {
		//buttonSound.Play ();
		SceneManager.LoadScene("credits");

		if (SceneManager.GetActiveScene().name == "ending" || SceneManager.GetActiveScene().name == "Level 1")
		{
			GameObject.Find ("mainmenu_loop").GetComponent<AudioSource> ().Play ();

		}
	}

	public void load_title() {
		
		//buttonSound.Play ();
		SceneManager.LoadScene("title");
		GameObject.Find ("RomanceMusic").GetComponent<AudioSource>().Stop();
		GameObject.Find ("SpyMusic").GetComponent<AudioSource>().Stop();


		if (SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "ending")
			GameObject.Find ("mainmenu_loop").GetComponent<AudioSource>().Play ();
		
	}

	public void Instructions(){
		//buttonSound.Play ();

		if (instructionsOpen)
		{
			instructionsOpen = false;
			blackbg.enabled = false;
			whiteborder.enabled = false;
			actualinstructions.enabled = false;
		}
		else
		{
			blackbg.enabled = true;
			whiteborder.enabled = true;
			actualinstructions.enabled = true;
			instructionsOpen = true;

		}
	}
}
