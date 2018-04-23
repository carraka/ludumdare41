using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmArrow : MonoBehaviour {

    public int reference;
    public float beat;
    public RhythmUI.arrowDirection direction;
    public RhythmUI.arrowType type;

    private RhythmUI parentUI;
    private RhythmUI.arrowState state;
    private float stateOffset;
    public float clearTime;
    
	// Use this for initialization
	void Start () {
        parentUI = transform.parent.gameObject.GetComponent<RhythmUI>();
        state = RhythmUI.arrowState.normal;

        Material newMaterial = new Material(this.gameObject.GetComponent<Image>().material);

        this.gameObject.GetComponent<Image>().material = newMaterial;
    }

    public void DoStuff(RhythmUI.arrowState command, RhythmUI.arrowType commandType, Sprite newSprite = null)
    {
        state = command;
        stateOffset = Time.time;
        type = commandType;

        if (newSprite != null)
        {
            this.GetComponent<Image>().overrideSprite = newSprite;

            Vector3 destination = this.gameObject.transform.localPosition;
            Vector3 rotation = new Vector3(0, 0, 0);

            Quaternion rotationQuat = Quaternion.identity;
            rotationQuat.eulerAngles = rotation;

            this.gameObject.transform.SetPositionAndRotation(destination, rotationQuat);
            this.gameObject.transform.localPosition = destination;
        }

//        Debug.Log("Arrow " + reference + " " + command);

    }

    // Update is called once per frame
    void Update()
    {
        if (state == RhythmUI.arrowState.normal)
        {
            Vector3 destination = this.gameObject.transform.localPosition;
            float currentBeat = parentUI.timeToBeat(Time.time);

            destination.y = 208f - 64f * (beat - currentBeat);
            this.gameObject.transform.localPosition = destination;

            if (destination.y > 272)
                Destroy(this.gameObject);
        } else
        {   if (state == RhythmUI.arrowState.ClearGood)
            {
                float scale = 1 + ((Time.time - stateOffset) / clearTime);
                this.gameObject.transform.localScale = new Vector3(scale, scale, 1f);
            }

            float transparancy = 1 - ((Time.time - stateOffset) / clearTime);
            byte alpha = (byte) Mathf.FloorToInt(255f * transparancy);

            this.gameObject.GetComponent<Image>().material.SetColor("_Color", new Color32(255, 255, 255, alpha));

            if (Time.time - stateOffset > clearTime)
                Destroy(this.gameObject);
        }
    }
}
