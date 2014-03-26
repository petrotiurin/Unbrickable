using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{	
	private GameObject target = null;
	private bool rotRight;
	private float XrotToGo = 0;
	private float YrotToGo = 0;
	private int rotSpeed = 200;
	public int rotationDir=0;
	
	//Initialise starting values and find the base of the gameboard
	void Start ()
	{
		rotRight = false;
		target = GameObject.Find("base");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( target != null ){
			
			//Always focus on the centre of the gameboard base
			transform.LookAt(target.transform);
			
			if (XrotToGo <= 0) {
				//Press "w" to rotate the board CW
				/*if( Input.GetKey("w") ){
					
				} else if ( Input.GetKey("s") ) {
					
				}*/
				if( Input.GetKey("x") ){
					rotRight = true;
					XrotToGo = 90;
					if(rotationDir==3){
						rotationDir = 0;
					}else{
						rotationDir++;
					}
				}
				//Press "z" to rotate the board CCW
				else if( Input.GetKey("z") ){
					rotRight = false;
					XrotToGo = 90;
					if(rotationDir==0){
						rotationDir = 3;
					}else{
						rotationDir--;
					}
				}
			}
			
			//Make the position change
			if (XrotToGo >= 0){
				float rot = Time.deltaTime * rotSpeed;
				if (XrotToGo < rot) rot = XrotToGo;
				if(rotRight){
					transform.RotateAround( target.transform.position, Vector3.down, rot);
				} else {
					transform.RotateAround( target.transform.position, Vector3.up, rot);
				}
				XrotToGo -= rot;
			}
		}
	}
}

