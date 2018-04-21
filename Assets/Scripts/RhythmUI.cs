using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmUI : MonoBehaviour {

    public enum arrowDirection { left, down, up, right};
    public enum arrowType { spy, romance};
    public enum arrowState { normal, ClearGood, ClearOK, ClearPoor, ClearMiss };


    public float offsetFirstBeat;
    public float BPM;
    //public soundobject song;
    public arrow [] songArrows;

    public Queue<arrowAction> arrowActions; //actions for arrows to read

    private bool playing;
    private float songStartTime;
    private float beatZero;
    private float BPS; // beats per second

    private int nextInput;
    private int nextReveal;

    private GameObject RhythmArrowPrefab;

    // Use this for initialization
    void Start () {
        RhythmArrowPrefab = (GameObject)Resources.Load("prefabs/RhythmArrow", typeof(GameObject));
        arrowActions = new Queue<arrowAction>();

        StartSong();
    }

    public void StartSong()
    {
        //song.play ();
        songStartTime = Time.time;
        playing = true;
        BPS = BPM / 60f;

        beatZero = songStartTime + offsetFirstBeat - (1f / BPS);

        nextInput = 0;
        nextReveal = 0;
    }

    public float timeToBeat(float time)
    {
        return (time - beatZero) * BPS;
    }

	// Update is called once per frame
	void Update ()
    {
        if (playing)
        {
            float currentBeat = timeToBeat(Time.time);

            bool renderNextArrow;

            if (nextReveal < songArrows.Length && (songArrows[nextReveal].beat - currentBeat < 8.5))
                renderNextArrow = true;
            else
                renderNextArrow = false;

            while (renderNextArrow == true)
                
            {
                GameObject newArrowGO = Instantiate(RhythmArrowPrefab);
                RhythmArrow newArrow = newArrowGO.GetComponent<RhythmArrow>();
                newArrow.reference = nextReveal;
                newArrow.beat = songArrows[nextReveal].beat;
                newArrow.direction = songArrows[nextReveal].direction;
                newArrow.type = songArrows[nextReveal].type;

                newArrowGO.transform.SetParent(transform);
                newArrowGO.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector3 pos = new Vector3(0, 208f - 64f * (newArrow.beat - currentBeat), 0);
                Vector3 rotation = new Vector3(0, 0, 0);
                switch (newArrow.direction)
                {
                    case arrowDirection.left:  pos.x = -96f;  rotation.z = 90;   break;
                    case arrowDirection.down:  pos.x = -32f;  rotation.z = 180;  break;
                    case arrowDirection.up:    pos.x = 32f;   rotation.z = 0;    break;
                    case arrowDirection.right: pos.x = 96f;   rotation.z = 270;  break;
                    // default: Debug.Log("What am I doing with my life?"); break;
                }

                Quaternion rotationQuat = Quaternion.identity;
                rotationQuat.eulerAngles = rotation;

                newArrowGO.transform.SetPositionAndRotation(pos, rotationQuat);
                newArrowGO.transform.localPosition = pos;

                nextReveal++;

                if (nextReveal < songArrows.Length && (songArrows[nextReveal].beat - currentBeat < 8.5))
                    renderNextArrow = true;
                else
                    renderNextArrow = false;

            }
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