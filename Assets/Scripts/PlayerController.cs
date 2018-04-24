using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float tileWidth;
	private float tileHeight;
    private float xOffset;
    private float yOffset;

    private Vector2 moveStart;
    private Vector2 moveDest;
    private float moveStartTime;
    private float moveTime;

    private Vector2 spyLocation;

	[System.Serializable]
	public struct Movement{
        public enum action { wait = 0, move, crouchLeft, crouchRight };
        public action command;
		public float duration;
        public Vector2 moveDest;
	}

	public Movement[] introMoves;

    private Queue<Movement> actionQueue;
    private Movement currentAction;
    private float currentActionStart;

	public float speed;
	public int nextTargetIndex = 0;

	private Animator animator;

    //private bool newMovement;
	private bool moving;

    public void pushAction (Movement.action command, float duration)
    {
        pushAction(command, duration, spyLocation);
    }

    public void pushAction (Movement.action command, float duration, Vector2 moveDest)
    {
        Movement move = new Movement();
        move.command = command;
        move.duration = duration;
        move.moveDest = moveDest;

        actionQueue.Enqueue(move);
    }

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();

		tileWidth = 64;
		tileHeight = 64;

        xOffset = -160;
        yOffset = 128;

		Reset ();
    
//		MoveManager (spyMoves);

	}

	public void Reset()
	{        
		spyLocation = TiletoWorld(new Vector2(0, 4));

		moveDest = spyLocation;
		moveStart = spyLocation;
		transform.localPosition = spyLocation;

		//newMovement = false;
		moving = false;

		actionQueue = new Queue<Movement>();

		foreach(Movement action in introMoves)
		{
			actionQueue.Enqueue(action);
		}
	}
	
/*    void TempMoveManager()
    {
        if (Input.GetKeyDown(KeyCode.W) & tileTarget.y > 0)
        {
            tileTarget.y--;
            newMovement = true;
        }
        if (Input.GetKeyDown(KeyCode.A) & tileTarget.x > 0)
        {
            tileTarget.x--;
            newMovement = true;
        }
        if (Input.GetKeyDown(KeyCode.S) & tileTarget.y < 4)
        {
            tileTarget.y++;
            newMovement = true;
        }
        if (Input.GetKeyDown(KeyCode.D) && tileTarget.x < 5)
        {
            tileTarget.x++;
            newMovement = true;
        }


        if (newMovement)
        {
            moveStart = new Vector2 (transform.localPosition.x, transform.localPosition.y);
            moveDest = TiletoWorld(tileTarget);
            Debug.Log(moveDest);
            moveStartTime = Time.time;
            moveTime = 1f;

            newMovement = false;
            moving = true;

            animateWalking(moveDest - moveStart);
        }
    } */

    void MoveSpy()
    {
        if (moving)
        {
            float percentage = Mathf.Min((Time.time - moveStartTime) / moveTime, 1f);
            spyLocation = (percentage * moveDest) + ((1 - percentage) * moveStart);
            transform.localPosition = spyLocation;

            if (percentage == 1f)
                moving = false;
        }

        if (moving == false)
        {
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingDown", false);
        }
    }
    // Update is called once per frame

    void Update () {
        MoveManager();
	}

	public void MoveManager()
	{
        if (Time.time - currentActionStart > currentAction.duration) //if finished with current action
        {
            if (actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();              // pull next action if available
                currentActionStart = Time.time;
            }
            else
            {
                currentAction.command = Movement.action.wait;       // wait if no actions in queue
                currentAction.duration = 0f;
                currentActionStart = Time.time;
            }

            MovePlayer(currentAction);
        }

        MoveSpy();
	}

	public void MovePlayer(Movement currentAction)
    {
        animator.SetBool("crouchLeft", false);
        animator.SetBool("crouchRight", false);
        animator.SetBool("walkingUp", false);
        animator.SetBool("walkingRight", false);
        animator.SetBool("walkingLeft", false);
        animator.SetBool("walkingDown", false);

        switch (currentAction.command)
        {
            case (Movement.action.crouchLeft): animator.SetBool("crouchLeft", true); break;
            case (Movement.action.crouchRight): animator.SetBool("crouchRight", true); break;
            case (Movement.action.move):
                moveStart = spyLocation;
                moveDest = TiletoWorld(currentAction.moveDest);
                moveStartTime = Time.time;
                moveTime = currentAction.duration;

                moving = true;

                animateWalking(moveDest - moveStart);

                break;
            default: break;
        }
    }//Move Player

	public Vector3 TiletoWorld(Vector2 tileInput)
	{
		Vector3 worldPos = new Vector3(tileInput.x * tileWidth + xOffset, yOffset - tileInput.y * tileHeight);

		return worldPos;
	}

	void animateWalking (Vector2 direction)
	{
        if (direction.y > 0 && Mathf.Abs (direction.y) >= Mathf.Abs (direction.x))
            animator.SetBool ("walkingUp", true); 
		else if (direction.x < 0 && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            animator.SetBool("walkingLeft", true);
        else if(direction.x > 0 && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            animator.SetBool("walkingRight", true);
        else if (direction.y < 0 && Mathf.Abs(direction.y) >= Mathf.Abs(direction.x))
            animator.SetBool("walkingDown", true);
	}
}//PlayerController
