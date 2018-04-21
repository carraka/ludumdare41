using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmArrow : MonoBehaviour {

    public int reference;
    public float beat;
    public RhythmUI.arrowDirection direction;
    public RhythmUI.arrowType type;

    private RhythmUI parentUI;
    private RhythmUI.arrowState state;
    private float stateOffset;
    
	// Use this for initialization
	void Start () {
        parentUI = transform.parent.gameObject.GetComponent<RhythmUI>();

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 destination = this.gameObject.transform.localPosition;
        float currentBeat = parentUI.timeToBeat(Time.time);

        destination.y = 208f - 64f * (beat - currentBeat);
        this.gameObject.transform.localPosition = destination;

        if (destination.y > 272)
        {
            // miss
            Destroy(this.gameObject);
            return;
        }

        //transform.Translate(destination-transform.position);
        //transform.localPosition = destination;

        if (state == RhythmUI.arrowState.normal)
        {
            if (parentUI.arrowActions.Count == 0)
                return;
            if (parentUI.arrowActions.Peek().reference == reference)
                {
                    arrowAction command = parentUI.arrowActions.Dequeue();
                    state = command.state;
                    //do stuff
                //destroy(this);
            }
        }
	}
}
