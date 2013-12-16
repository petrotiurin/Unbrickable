using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private int i, j, pass;
	
	private int i, j, posX=2,posY=1,posZ=2, rotation=0;
	
	private Vector3 centreRotation = new Vector3 (2,1,2);
	
	private GameObject FragmentCube;

	// Use this for initialization
	void Start () {
		i = 0;
		j = 0;
		pass = 0;
		//CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
		
		//rotate right
		if (Input.GetKeyDown("x")){	
			//board has been moved (2,0,2) 2in x, 2in z-->> --^
			transform.RotateAround (centreRotation, Vector3.up, 90);
			rotation += 90;		
			print ("rotated around " + centreRotation);
		}		
		//rotate left
		if (Input.GetKeyDown("z")){
			transform.RotateAround (centreRotation, Vector3.up, -90);
			rotation -= 90;
			print ("rotated around " + centreRotation);

		}	
		
			//move forward
			if (Input.GetKeyDown("up")){
				if(transform.position.z < (4258.936))
				transform.position = new Vector3(transform.position.x,transform.position.y, transform.position.z +1);
				posZ +=1;
				centreRotation = new Vector3(posX,posY,posZ);
			}
			//move back
			if (Input.GetKeyDown("down")){
				if(transform.position.z > 4249.936)
				transform.position = new Vector3(transform.position.x,transform.position.y, transform.position.z -1);
				posZ -=1;
				centreRotation = new Vector3(posX,posY,posZ);
			}
			//move right
			if (Input.GetKeyDown("right")){
				//board starts off at -0.5 in x-direction
				if(transform.position.x != (gameBoard.nx+0.5))
				transform.position = new Vector3(transform.position.x + 1,transform.position.y, transform.position.z);
				posX +=1;
				centreRotation = new Vector3(posX,posY,posZ);
			}
			//move left
			if (Input.GetKeyDown("left")){
				//board starts off at -0.5 in x-direction
				if(transform.position.x != (-(gameBoard.nx-0.5)))
				transform.position = new Vector3(transform.position.x - 1,transform.position.y, transform.position.z);
				posX -=1;
				centreRotation = new Vector3(posX,posY,posZ);
				print("moved left position.x =  " + transform.position.x +"  centre rotation = "+ centreRotation);
		}
	}	
	
	/* Creates a new cube at the top of our scene.
	 * Works only if the last object stopped moving. */
	public void CreateCube() {
		
		if (pass == 0 && (i + j*5) == 24){
			i++;
			pass=1;
		}
		
		//wrap-around
		if (i == 5){
			i = 0;
			j = (j+1)%5;
		}
		
		GameObject cube = GameObject.Instantiate(Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject))) as GameObject;
		cube.name = "ActiveBlock";	
		
		//drop a brick on each space on the board in order
		cube.transform.position = new Vector3(i, 6, j); //TODO: randomize?
		
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		cube.AddComponent("BlockCollision");
		addToScene(cube);
		
		i++;
	}
	
	//Makes given object a child of the "Scene"
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}	
}
