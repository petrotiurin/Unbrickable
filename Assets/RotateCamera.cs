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
	//private BlockControl blockCtrl;
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
		transform.RotateAround( target.transform.position, Vector3.up, -30);
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
		if (topCam == null) topCam = GameObject.Find("Top Cam");
		if( target != null && topCam != null){

			//Always focus on the centre of the gameboard base
			transform.LookAt(target.transform);
			
			processVerticalRotation();
			
			if (XrotToGo <= 0) {
				//Press "x" to rotate the board CW
				if( Input.GetKey("d") ){
					rotRight = true;
					XrotToGo = 90;
					rotationDir++;
					rotationDir = rotationDir % 4;
				}
				//Press "z" to rotate the board CCW
				else if( Input.GetKey("a") ){
					rotRight = false;
					XrotToGo = 90;
					rotationDir--;
					if(rotationDir < 0) rotationDir = 3;
				}
			}
			else{
				//Make the position change
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
