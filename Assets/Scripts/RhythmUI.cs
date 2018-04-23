using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmUI : MonoBehaviour {

    public enum arrowDirection { left, down, up, right};
    public enum arrowType { none, spy, romance};
    public enum arrowState { normal, ClearGood, ClearOK, ClearPoor, ClearMiss };


    public float offsetFirstBeat;
    public float BPM;
    //public soundobject song;

    public float goodThreshold;
    public float OKThreshold;
    public float poorThreshold;
    public float missThreshold;

    public arrow [] songArrows;

    public float[] reportTimes;
    private int nextReport;

    private int reportSpyArrows;
    private int reportSpyHits;
    private int reportLoveArrows;
    private int reportLoveHits;

    private Sprite heart;
    private Sprite spy;
    private Sprite redArrow;
    private Sprite blueArrow;
    private Sprite guideArrow;
    private Sprite guideArrowBlink;

    private Slider loveSlider;
    private Slider spySlider;

    private bool playing;
    private float songStartTime;
    private float beatZero;
    private float BPS; // beats per second

    private int nextReveal;
    private int lastResolved;

    private GameObject RhythmArrowPrefab;

    private GameObject GuideArrowLeft;
    private GameObject GuideArrowDown;
    private GameObject GuideArrowUp;
    private GameObject GuideArrowRight;

    private float leftLastPressed;
    private float downLastPressed;
    private float upLastPressed;
    private float rightLastPressed;

    private GameManager gameManager;

    private AudioSource audio;

    private AudioClip clucks1;
    private AudioClip clucks2;
    private AudioClip squawk1;
    private AudioClip squawk2;

    private void Awake()
    {
        RhythmArrowPrefab = (GameObject)Resources.Load("prefabs/RhythmArrow", typeof(GameObject));

        GameObject tempGO;

        tempGO = (GameObject)Resources.Load("prefabs/Heart", typeof(GameObject));
        heart = tempGO.GetComponent<SpriteRenderer>().sprite;

        tempGO = (GameObject)Resources.Load("prefabs/Spy", typeof(GameObject));
        spy = tempGO.GetComponent<SpriteRenderer>().sprite;

        tempGO = (GameObject)Resources.Load("prefabs/RedArrow", typeof(GameObject));
        redArrow = tempGO.GetComponent<SpriteRenderer>().sprite;

        tempGO = (GameObject)Resources.Load("prefabs/BlueArrow", typeof(GameObject));
        blueArrow = tempGO.GetComponent<SpriteRenderer>().sprite;

        tempGO = (GameObject)Resources.Load("prefabs/GuideArrow", typeof(GameObject));
        guideArrow = tempGO.GetComponent<SpriteRenderer>().sprite;

        tempGO = (GameObject)Resources.Load("prefabs/GuideArrowBlink", typeof(GameObject));
        guideArrowBlink = tempGO.GetComponent<SpriteRenderer>().sprite;

        GuideArrowLeft = GameObject.Find("GuideArrowLeft");
        GuideArrowDown = GameObject.Find("GuideArrowDown");
        GuideArrowUp = GameObject.Find("GuideArrowUp");
        GuideArrowRight = GameObject.Find("GuideArrowRight");

        spySlider = GameObject.Find("SpySlider").GetComponent<Slider>();
        loveSlider = GameObject.Find("LoveSlider").GetComponent<Slider>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        audio = this.gameObject.GetComponent<AudioSource>();

        clucks1 = (AudioClip)Resources.Load("Audio/SFX/clucks1");
        clucks2 = (AudioClip)Resources.Load("Audio/SFX/clucks2");
        squawk1 = (AudioClip)Resources.Load("Audio/SFX/squawk1");
        squawk2 = (AudioClip)Resources.Load("Audio/SFX/squawk2");


    }

    // Use this for initialization
    void Start()
    {
        leftLastPressed = -1;
        downLastPressed = -1;
        upLastPressed = -1;
        rightLastPressed = -1;

        nextReport = 0;
        prepareReport(0);
    }

    void prepareReport (int x)
    {
        float firstArrow;

        if (reportTimes.Length == 0)
            return;
        if (x == 0)
            firstArrow = -1;
        else firstArrow = reportTimes[x - 1];

        float lastArrow;

        if (x >= reportTimes.Length)
            lastArrow = songArrows[songArrows.Length - 1].beat;
        else lastArrow = reportTimes[x];

        reportSpyArrows = 0;
        reportSpyHits = 0;
        reportLoveArrows = 0;
        reportLoveHits = 0;

        foreach (arrow check in songArrows)
            if (check.beat > firstArrow && check.beat <= lastArrow)
            {
                if (check.type == arrowType.spy)
                    reportSpyArrows++;
                if (check.type == arrowType.romance)
                    reportLoveArrows++;
            }

        // Debug.Log("report " + x + " prepared, " + reportSpyArrows + " Spy Arrows, " + reportLoveArrows + " Romance Arrows");
    }

    void makeReport ()
    {
        float spyPercent = (float)reportSpyHits / reportSpyArrows;
        float lovePercent = (float)reportLoveHits / reportLoveArrows;

        if (spyPercent > Mathf.Max(.5f, lovePercent))
            gameManager.nextDirection = GameManager.GameDirection.spy;
        else if (lovePercent > Mathf.Max(.5f, spyPercent))
            gameManager.nextDirection = GameManager.GameDirection.romance;
        else
        {
            gameManager.nextDirection = GameManager.GameDirection.chicken;
            audio.volume = 2f;
            if (Random.value < .5)
                audio.PlayOneShot(clucks1);
            else
                audio.PlayOneShot(clucks2);
        }

            // Debug.Log("Report " + nextReport + " result: " + gameManager.nextDirection);

            nextReport++;
        prepareReport(nextReport);

    }

    public void StartSong()
    {
        //song.play ();
        songStartTime = Time.time;
        playing = true;
        BPS = BPM / 60f;

        beatZero = songStartTime + offsetFirstBeat - (1f / BPS);

        nextReveal = 0;
        lastResolved = -1;
    }

    public float timeToBeat(float time)
    {
        return (time - beatZero) * BPS;
    }

    // Update is called once per frame

    void RenderArrows(float currentBeat)
    {
        bool renderNextArrow;

        do
        {
            if (nextReveal < songArrows.Length && (songArrows[nextReveal].beat - currentBeat < 8.5))
            {
                renderNextArrow = true;

                GameObject newArrowGO = Instantiate(RhythmArrowPrefab);
                RhythmArrow newArrow = newArrowGO.GetComponent<RhythmArrow>();
                newArrow.reference = nextReveal;
                newArrow.beat = songArrows[nextReveal].beat;
                newArrow.direction = songArrows[nextReveal].direction;
                newArrow.type = songArrows[nextReveal].type;

                if (newArrow.type == arrowType.spy)
                    newArrow.GetComponent<Image>().overrideSprite = blueArrow;
                if (newArrow.type == arrowType.romance)
                    newArrow.GetComponent<Image>().overrideSprite = redArrow;


                newArrowGO.transform.SetParent(transform);
                newArrowGO.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector3 pos = new Vector3(0, 208f - 64f * (newArrow.beat - currentBeat), 0);
                Vector3 rotation = new Vector3(0, 0, 0);
                switch (newArrow.direction)
                {
                    case arrowDirection.left: pos.x = -96f; rotation.z = 90; break;
                    case arrowDirection.down: pos.x = -32f; rotation.z = 180; break;
                    case arrowDirection.up: pos.x = 32f; rotation.z = 0; break;
                    case arrowDirection.right: pos.x = 96f; rotation.z = 270; break;
                }

                Quaternion rotationQuat = Quaternion.identity;
                rotationQuat.eulerAngles = rotation;

                newArrowGO.transform.SetPositionAndRotation(pos, rotationQuat);
                newArrowGO.transform.localPosition = pos;

                nextReveal++;
            }
            else
                renderNextArrow = false;

        } while (renderNextArrow == true);
    }

    void AnimateGuideArrows(float currentBeat)
    {
        bool pressedLeft = false;
        bool pressedUp = false;
        bool pressedDown = false;
        bool pressedRight = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            pressedLeft = true;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            pressedUp = true;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            pressedDown = true;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            pressedRight = true;

        if (pressedLeft)
        {
            GuideArrowLeft.transform.localScale = new Vector3(.9f, .9f, 1f);
        }
        else
        {
            Vector3 scale = GuideArrowLeft.transform.localScale;
            if (scale.x < 1)
                scale.x = Mathf.Min(scale.x + .02f, 1f);
            scale.y = scale.x;
            scale.z = 1f;
            GuideArrowLeft.transform.localScale = scale;
        }

        if (pressedDown)
        {
            GuideArrowDown.transform.localScale = new Vector3(.9f, .9f, 1f);
        }
        else
        {
            Vector3 scale = GuideArrowDown.transform.localScale;
            if (scale.x < 1)
                scale.x = Mathf.Min(scale.x + .02f, 1f);
            scale.y = scale.x;
            scale.z = 1f;
            GuideArrowDown.transform.localScale = scale;
        }

        if (pressedUp)
        {
            GuideArrowUp.transform.localScale = new Vector3(.9f, .9f, 1f);
        }
        else
        {
            Vector3 scale = GuideArrowUp.transform.localScale;
            if (scale.x < 1)
                scale.x = Mathf.Min(scale.x + .02f, 1f);
            scale.y = scale.x;
            scale.z = 1f;
            GuideArrowUp.transform.localScale = scale;
        }

        if (pressedRight)
        {
            GuideArrowRight.transform.localScale = new Vector3(.9f, .9f, 1f);
        }
        else
        {
            Vector3 scale = GuideArrowRight.transform.localScale;
            if (scale.x < 1)
                scale.x = Mathf.Min(scale.x + .02f, 1f);
            scale.y = scale.x;
            scale.z = 1f;
            GuideArrowRight.transform.localScale = scale;
        }


        if (Mathf.Abs(Mathf.Round(currentBeat)-currentBeat) < goodThreshold)
        {
            GuideArrowLeft.GetComponent<Image>().overrideSprite = guideArrowBlink;
            GuideArrowDown.GetComponent<Image>().overrideSprite = guideArrowBlink;
            GuideArrowUp.GetComponent<Image>().overrideSprite = guideArrowBlink;
            GuideArrowRight.GetComponent<Image>().overrideSprite = guideArrowBlink;
        } else
        {
            if (GuideArrowLeft.GetComponent<Image>().overrideSprite == guideArrowBlink)
            {
                GuideArrowLeft.GetComponent<Image>().overrideSprite = guideArrow;
                GuideArrowDown.GetComponent<Image>().overrideSprite = guideArrow;
                GuideArrowUp.GetComponent<Image>().overrideSprite = guideArrow;
                GuideArrowRight.GetComponent<Image>().overrideSprite = guideArrow;
            }
        }
    }

    void Update ()
    {
        if (playing)
        {
            float currentBeat = timeToBeat(Time.time);

            RenderArrows(currentBeat);

            AnimateGuideArrows(currentBeat);

            bool pressedLeft = false;
            bool pressedUp = false;
            bool pressedDown = false;
            bool pressedRight = false;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                pressedLeft = true;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                pressedUp = true;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                pressedDown = true;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                pressedRight = true;

            bool leftHeld = pressedLeft;
            bool downHeld = pressedDown;
            bool upHeld = pressedUp;
            bool rightHeld = pressedRight;

            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && (currentBeat - leftLastPressed) <= goodThreshold)
                leftHeld = true;
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && (currentBeat - downLastPressed) <= goodThreshold)
                downHeld = true;
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && (currentBeat - upLastPressed) <= goodThreshold)
                upHeld = true;
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && (currentBeat - rightLastPressed) <= goodThreshold)
                rightHeld = true;

            int x = lastResolved + 1;
            bool checkMoreArrows = true;

//            arrowState command = arrowState.normal;

            do
            {
                if (x >= songArrows.Length)
                {
                    checkMoreArrows = false;
                }
                else if (currentBeat > songArrows[x].beat + missThreshold)   //clear if past missThreshold
                {
                    foreach (Transform eachChild in transform)
                    {
                        if (eachChild.tag == "Arrow")
                            if (eachChild.gameObject.GetComponent<RhythmArrow>().reference == x)
                                eachChild.gameObject.GetComponent<RhythmArrow>().DoStuff(arrowState.ClearMiss, arrowType.none);
                    }

                    switch (songArrows[x].type)
                    {
                        case (arrowType.spy):
                            spySlider.value = Mathf.Max(spySlider.value - 3, 0);
                            break;
                        case (arrowType.romance):
                            loveSlider.value = Mathf.Max(loveSlider.value - 3, 0);
                            break;
                    }

                    lastResolved = x;
                    x++;
                }
                else if (currentBeat < songArrows[x].beat - missThreshold) //
                {
                    checkMoreArrows = false;
                }
                else
                {

                    float beat = songArrows[x].beat;
                    List<int> arrowsThisBeat = new List<int>();

                    while (songArrows[x].beat == beat)
                    {
                        arrowsThisBeat.Add(x);
                        x++;

                        if (x >= songArrows.Length)
                            break;
                    }

                    bool fail = false;
                    bool success = false;
                    int successCount = 0;

                    arrowType clearType = arrowType.none;

                    if (arrowsThisBeat.Count == 1)
                    {
                        int y = arrowsThisBeat[0];

                        if (songArrows[y].direction == arrowDirection.left && pressedLeft
                            || songArrows[y].direction == arrowDirection.up && pressedUp
                            || songArrows[y].direction == arrowDirection.right && pressedRight
                            || songArrows[y].direction == arrowDirection.down && pressedDown)
                        {
                            successCount = 1;
                            success = true;
                            fail = false;
                            clearType = songArrows[y].type;
                        }

                    }

                    if (arrowsThisBeat.Count > 1)
                    {
                        foreach (int y in arrowsThisBeat)
                        {
                            if (songArrows[y].direction == arrowDirection.left && pressedLeft
                                || songArrows[y].direction == arrowDirection.up && pressedUp
                                || songArrows[y].direction == arrowDirection.right && pressedRight
                                || songArrows[y].direction == arrowDirection.down && pressedDown)

                                successCount++;

                        }

                        if (successCount > 0)
                        {
                            int spiesFound = 0;
                            int spiesHit = 0;
                            int loveFound = 0;
                            int loveHit = 0;

                            foreach (int y in arrowsThisBeat)
                            {
                                bool hit;
                                if ((songArrows[y].direction == arrowDirection.left && leftHeld)
                                || (songArrows[y].direction == arrowDirection.up && upHeld)
                                || (songArrows[y].direction == arrowDirection.down && downHeld)
                                || (songArrows[y].direction == arrowDirection.right && rightHeld))
                                    hit = true;
                                else hit = false;

                                if (songArrows[y].type == arrowType.spy)
                                {
                                    spiesFound++;
                                    if (hit)
                                        spiesHit++;
                                }
                                if (songArrows[y].type == arrowType.romance)
                                {
                                    loveFound++;
                                    if (hit)
                                        loveHit++;
                                }
                            }

                            if (spiesHit > 0 && loveHit > 0)
                            {
                                fail = true;                    // if you hit both spies and hearts, you fail
                                success = false;
                            }
                            else if (spiesHit == spiesFound && spiesHit > 0)
                            {
                                success = true;
                                fail = false;
                                successCount = spiesHit;
                                clearType = arrowType.spy;
                            }
                            else if (loveHit == loveFound && loveHit > 0)
                            {
                                success = true;
                                fail = false;
                                successCount = loveHit;
                                clearType = arrowType.romance;
                            }
                        }
                    }

                    arrowState precision = arrowState.normal;

                    if (success == true || fail == true)
                    {
                        if (fail == true)
                        {
                            clearType = arrowType.none;
                            precision = arrowState.ClearMiss;

                            spySlider.value = Mathf.Max(spySlider.value - 3, 0);
                            loveSlider.value = Mathf.Max(loveSlider.value - 3, 0);

                            gameManager.cluck = true;
                            audio.volume = 1f;
                            if (Random.value < .5)
                                audio.PlayOneShot(squawk1);
                            else
                                audio.PlayOneShot(squawk2);

                        }
                        else if (success == true)
                        {
                            int points = 0;
                            if (Mathf.Abs(beat - currentBeat) <= goodThreshold)
                            {
                                precision = arrowState.ClearGood;
                                points = 3;
                            }
                            else if (Mathf.Abs(beat - currentBeat) <= OKThreshold)
                            {
                                precision = arrowState.ClearOK;
                                points = 1;
                            }
                            else if (Mathf.Abs(beat - currentBeat) <= poorThreshold)
                            {
                                precision = arrowState.ClearPoor;
                                points = 0;
                            }
                            else
                            {
                                precision = arrowState.ClearMiss;
                                points = -3;
                            }

                            switch (clearType)
                            {
                                case (arrowType.spy):
                                    spySlider.value = Mathf.Min(spySlider.value + points * successCount, 100);
                                    loveSlider.value = Mathf.Max(loveSlider.value - 3 * successCount, 0);
                                    reportSpyHits += successCount;
                                    break;
                                case (arrowType.romance):
                                    loveSlider.value = Mathf.Min(loveSlider.value + points * successCount, 100);
                                    spySlider.value = Mathf.Max(spySlider.value - 3 * successCount, 0);
                                    reportLoveHits += successCount;
                                    break;
                            }
                        }

                        foreach (Transform eachChild in transform)
                        {
                            if (eachChild.tag == "Arrow")
                                foreach (int y in arrowsThisBeat)
                                {
                                    if (eachChild.gameObject.GetComponent<RhythmArrow>().reference == y)
                                    {
                                        switch (clearType)
                                        {
                                            case (arrowType.spy):
                                                eachChild.gameObject.GetComponent<RhythmArrow>().DoStuff(precision, clearType, spy); break;
                                            case (arrowType.romance):
                                                eachChild.gameObject.GetComponent<RhythmArrow>().DoStuff(precision, clearType, heart); break;
                                            default:
                                                eachChild.gameObject.GetComponent<RhythmArrow>().DoStuff(precision, clearType); break;
                                        }
                                    }
                                }
                        }

                        lastResolved = x - 1;
                    }
                } 
            } while (checkMoreArrows);

            if (pressedLeft) leftLastPressed = currentBeat;
            if (pressedDown) downLastPressed = currentBeat;
            if (pressedUp) upLastPressed = currentBeat;
            if (pressedRight) rightLastPressed = currentBeat;

            if (nextReport < reportTimes.Length && currentBeat > reportTimes[nextReport] + missThreshold)
                makeReport();
        }
    }
}

public struct arrowAction
{
    public int reference;
    public RhythmUI.arrowState state;
}

[System.Serializable]
public struct arrow
{
    public float beat;
    public RhythmUI.arrowDirection direction;
    public RhythmUI.arrowType type;

}