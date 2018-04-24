using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LockedArtManager : MonoBehaviour {

    private static bool created;

    private Image art;

    public bool unlocked1, unlocked2, unlocked3;
    void Awake()
    {

        if (created)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            
        }


    }
	// Use this for initialization
	void Start () {
		
	}
	
    public void Ending1()
    {
		art = GameObject.Find("art").GetComponent<Image>();

        if (unlocked1)
        {
            art.enabled = true;
            art.sprite = Resources.Load<Sprite>("Sprites/UI/Endings/lonely_ending");
        }
        else
        {
            art.enabled = false;
        }

    }

    public void Ending2()
    {
		art = GameObject.Find("art").GetComponent<Image>();

        if (unlocked1)
        {
            art.enabled = true;
            art.sprite = Resources.Load<Sprite>("Sprites/UI/Endings/lover_ending");
        }
        else
        {
            art.enabled = false;
        }

    }

    public void Ending3()
    {
		art = GameObject.Find("art").GetComponent<Image>();

        if (unlocked1)
        {
            art.enabled = true;
            art.sprite = Resources.Load<Sprite>("Sprites/UI/Endings/chicken_ending");
        }
        else
        {
            art.enabled = false;
        }

    }

    public void LoadArtScene()
    {
        SceneManager.LoadScene("unlockedArt");
    }
    // Update is called once per frame
    void Update () {
		
	}
}
