using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private int i, j, pass;
	
	private GameObject FragmentCube;
	
	//for moving the pieces
	private int posX=2,posY=1,posZ=2;
	private Vector3 centreRotation = new Vector3 (2,1,2);

	// Use this for initialization
	void Start () {
		i = 0;
		j = 0;
		pass = 0;
		//CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
		
		GameObject block = GameObject.Find("ActiveBlock");
		
		//ROTATE right
		if (Input.GetKeyDown("v")){		
			//board has been moved (2,0,2) 2in x, 2in z-->> --^
			block.transform.RotateAround (centreRotation, Vector3.up, 90);
			print ("rotated around " + centreRotation);
		}		
		//ROTATE left
		if (Input.GetKeyDown("c")){
			block.transform.RotateAround (centreRotation, Vector3.up, -90);
			print ("rotated around " + centreRotation);

		}	
		
		//MOVE forward
			if (Input.GetKeyDown("up")){
					block.transform.position = new Vector3(block.transform.position.x,block.transform.position.y,block.transform.position.z +1);
					posZ +=1;
					centreRotation = new Vector3(posX,posY,posZ);
			}
			//move back
			if (Input.GetKeyDown("down")){
					block.transform.position = new Vector3(block.transform.position.x,block.transform.position.y,block.transform.position.z -1);
					posZ -=1;
					centreRotation = new Vector3(posX,posY,posZ);
			}
			//move right
			if (Input.GetKeyDown("right")){
				//board starts off at -0.5 in x-direction
				block.transform.position = new Vector3(block.transform.position.x + 1,block.transform.position.y,block.transform.position.z);
				posX +=1;
				centreRotation = new Vector3(posX,posY,posZ);
			}
			//move left
			if (Input.GetKeyDown("left")){
				//board starts off at -0.5 in x-direction
				block.transform.position = new Vector3(block.transform.position.x - 1,block.transform.position.y,block.transform.position.z);
				posX -=1;
				centreRotation = new Vector3(posX,posY,posZ);
				print("moved left position.x =  " + block.transform.position.x +"  centre rotation = "+ centreRotation);
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
