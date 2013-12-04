using UnityEngine;
using System.Collections;


public class movement : MonoBehaviour {
	
	 public float speed = 50f;
	float yRotation = 90.0f;
		
	// Update is called once per frame
	void Update () {
		//rotate right
		if (Input.GetKey("x")){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y + yRotation, transform.eulerAngles.z);
		}	
		//rotate left
		if (Input.GetKey("z")){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y - yRotation, transform.eulerAngles.z);
		}
		//move forward
		if (Input.GetKey("up")){
			
			//void Translate(float x, float y, float z, Space relativeTo = Space.Self);
			transform.Translate(Vector3.forward* Time.deltaTime);	
		}
		//move back
		if (Input.GetKey("down")){
			transform.Translate(Vector3.back * Time.deltaTime);	
		}
		//move right
		if (Input.GetKey("right")){
			transform.Translate(Vector3.right * Time.deltaTime);	
		}
		//move left
		if (Input.GetKey("left")){
			transform.Translate(Vector3.left * Time.deltaTime);	
		}
		
	}
		
}

