using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float tileWidth;
	private float tileHeight;

	public Vector2[] tileTargetList;
	public Vector2 tileTarget;

	public Vector2[][] spyMoves;

	public float speed;
	public int nextTargetIndex = 0;

	private Animator animator;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();

		tileWidth = 64;
		tileHeight = 64;

		MovePlayer ("move", "6,1");
		MovePlayer ("crouch", "left");
	}
	
	// Update is called once per frame
	void Update () {
	//	MoveToTarget ();
		checkDistance();

	}

	public void MovePlayer(string command, string value){
		if (command == "move"){
			string[] coordinates = value.Split(',');
			MoveToTarget (int.Parse (coordinates [0]), int.Parse (coordinates [1]));

		}
		else if (command == "crouch"){
			if (value == "left")
				animator.SetBool ("crouchLeft", true);
			else
				animator.SetBool ("crouchRight", true);
		}
		else {
			Debug.Log ("Error: Invalid command: " + command);
		}


	}//Move Player

	public Vector3 TiletoWorld(Vector2 tileInput)
	{
		Vector3 worldPos = new Vector3(tileInput.x * tileWidth, Camera.main.pixelHeight - tileInput.y * tileHeight - tileHeight);

		return worldPos;
	}

	public void MoveToTarget(int targetx, int targety)
	{

		bool doneMoving = false;

		tileTarget.x = targetx;
		tileTarget.y = targety;

		//Travel toward target
		if (!doneMoving) {
			if (tileTarget != null) {

				rb.velocity = Vector3.Normalize (TiletoWorld(tileTarget) - transform.position) * speed;

				Debug.Log (rb.velocity.x + ", " + rb.velocity.y); 
				//Debug.Log (this.transform.localRotation);
				Vector3 targ = TiletoWorld(tileTarget);
				targ.z = 0f;
				//Debug.Log (transform.position);
				//Debug.Log (targ);

				Vector3 objectPos = transform.position;
				targ.x = targ.x - objectPos.x;
				targ.y = targ.y - objectPos.y;

				//float angle = Mathf.Atan2 (targ.y, targ.x) * Mathf.Rad2Deg;
				//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle - 90));

				animateWalking (rb.velocity.x, rb.velocity.y);



			}//if (target =! null)
		}//while (!doneMoving)

		doneMoving = false;

	}//MovetoTarget

	void checkDistance ()
	{
		float distance = Vector3.Distance (transform.position, TiletoWorld(tileTarget));

		if (distance < 1f) {
			rb.velocity = Vector3.Normalize (TiletoWorld(tileTarget) - transform.position) * 0f;
		}
	}
	void animateWalking (float vel_x, float vel_y)
	{
		if (vel_x > 99){
			animator.SetBool ("walkingUp", false); 
			animator.SetBool ("walkingRight", true); 
			animator.SetBool ("walkingLeft", false); 
			animator.SetBool ("walkingDown", false); 
		}
		else if (vel_x < -99){
			animator.SetBool ("walkingUp", false); 
			animator.SetBool ("walkingRight", false); 
			animator.SetBool ("walkingLeft", true); 
			animator.SetBool ("walkingDown", false); 
		}
		else if (vel_y > 99){
			animator.SetBool ("walkingUp", true); 
			animator.SetBool ("walkingRight", false); 
			animator.SetBool ("walkingLeft", false);
			animator.SetBool ("walkingDown", false); 
		}
		else if (vel_y < -99){
			animator.SetBool ("walkingUp", false); 
			animator.SetBool ("walkingRight", false); 
			animator.SetBool ("walkingLeft", false);
			animator.SetBool ("walkingDown", true); 
		}
		else {
			animator.SetBool ("walkingUp", false); 
			animator.SetBool ("walkingRight", false); 
			animator.SetBool ("walkingLeft", false);
			animator.SetBool ("walkingDown", false); 
		}
	}
}//PlayerController
