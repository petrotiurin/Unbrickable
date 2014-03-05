using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{	
	private GameObject target = null;
	private bool rotRight;
	private bool rotLeft;
	private int rotSpeed = 100;
	
	//Initialise starting values and find the base of the gameboard
	void Start ()
	{
		rotRight = false;
		rotLeft = false;
		target = GameObject.Find("base");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( target != null ){
			
			//Always focus on the centre of the gameboard base
			transform.LookAt(target.transform);
			
			//Press "x" to rotate the board CW
			if( Input.GetKey("x") ){
				rotRight = true;
				rotLeft = false;
			}
			//Press "z" to rotate the board CCW
			else if( Input.GetKey("z") ){
				rotRight = false;
				rotLeft = true;
			}
			//The board does not rotate when nothing is pressed
			else{
				rotRight = false;
				rotLeft = false;
			}
			
			//Make the position change
			if(rotLeft){
				transform.RotateAround( target.transform.position, Vector3.up, Time.deltaTime * rotSpeed );
			}
			else if (rotRight){
				transform.RotateAround( target.transform.position, Vector3.down, Time.deltaTime * rotSpeed );
			}
		}
	}
}

