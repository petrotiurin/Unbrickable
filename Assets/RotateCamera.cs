using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{	
	private GameObject target = null;
	private bool rotRight;
	private bool rotLeft;
	private float rotToGo = 0;
	private int rotSpeed = 200;
	
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
			
			if (rotToGo <= 0) {
				//Press "x" to rotate the board CW
				if( Input.GetKey("x") ){
					rotRight = true;
					rotLeft = false;
					rotToGo = 90;
				}
				//Press "z" to rotate the board CCW
				else if( Input.GetKey("z") ){
					rotRight = false;
					rotLeft = true;
					rotToGo = 90;
				}
			}
			
			//Make the position change
			if (rotToGo >= 0){
				float rot = Time.deltaTime * rotSpeed;
				if (rotToGo < rot) rot = rotToGo;
				if(rotLeft){
					transform.RotateAround( target.transform.position, Vector3.up, rot);
				}
				else if (rotRight){
					transform.RotateAround( target.transform.position, Vector3.down, rot);
				}
				rotToGo -= Time.deltaTime * rotSpeed;
			} else {
				rotRight = false;
				rotLeft = false;
			}
		}
	}
}

