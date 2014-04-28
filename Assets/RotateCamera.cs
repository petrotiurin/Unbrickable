using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{	
	private GameObject target = null;
	private bool rotRight;
	private float XrotToGo = 0;
	private float YrotToGo = 0;
	private int XrotSpeed = 200;
	private int YrotSpeed = 100;
	public int rotationDir=0;
	//Maximum allowed angles on vertical camera movement. Subject to change.
	private int maxUpAngle = 40;
	private int maxDownAngle = -20;
	private Board brd;
	private GameObject topCam;
	
	//Initialise starting values and find the base of the gameboard
	void Start ()
	{
		topCam = GameObject.Find("Top Cam");
//		Debug.Log("hi");
		rotRight = false;
		target = GameObject.Find("base");
		brd = GameObject.Find("Scene").GetComponent<Board>();
	}
	
	void processVerticalRotation(){
		float rot = 0;
		if( Input.GetKey("w") ){
				rot = Time.deltaTime * YrotSpeed;
				if (YrotToGo + rot > maxUpAngle){ YrotToGo = maxUpAngle; rot = 0; }
		} else if ( Input.GetKey("s") ) {
				rot = - Time.deltaTime * YrotSpeed;
				if (YrotToGo + rot < maxDownAngle){ YrotToGo = maxDownAngle; rot = 0; }
		}
		YrotToGo += rot;
		transform.RotateAround( target.transform.position, transform.right, rot);
	}
	
	// Update is called once per frame
	void Update ()
	{
			if( target != null ){
				
				//Always focus on the centre of the gameboard base
				transform.LookAt(target.transform);
				
				processVerticalRotation();
				
				if (XrotToGo <= 0) {
					//Press "w" to rotate the board CW
					if( Input.GetKey("x") ){
						rotRight = true;
						XrotToGo = 90;
						rotationDir++;
						rotationDir = rotationDir % 4;
						brd.DrawBoundary(rotationDir);
					}
					//Press "z" to rotate the board CCW
					else if( Input.GetKey("z") ){
						rotRight = false;
						XrotToGo = 90;
						rotationDir--;
						if(rotationDir < 0) rotationDir = 3;
						brd.DrawBoundary(rotationDir);
					}
				}
				
				//Make the position change
				if (XrotToGo >= 0){
					float rot = Time.deltaTime * XrotSpeed;
					if (XrotToGo < rot) rot = XrotToGo;
					if(rotRight){
						transform.RotateAround( target.transform.position, Vector3.down, rot);
						topCam.transform.RotateAround( topCam.transform.position, Vector3.down, rot);
					} else {
						transform.RotateAround( target.transform.position, Vector3.up, rot);
						topCam.transform.RotateAround( topCam.transform.position, Vector3.up, rot);
					}
					XrotToGo -= rot;
				}
			}
		}

}

