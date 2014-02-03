using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
<<<<<<< HEAD
	private int i, j, pass;
=======
	private Board gameBoard = new Board();
	
	private int i, j, posX=2,posY=1,posZ=2, rotation=0;
	
	private Vector3 centreRotation = new Vector3 (2,1,2);
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
	
	private GameObject FragmentCube;
	
	//for moving the pieces
	private int posX=2,posY=1,posZ=2;
	private Vector3 centreRotation = new Vector3 (2,1,2);

	// Use this for initialization
	void Start () {
<<<<<<< HEAD
=======

		blocks = new GameObject[gameBoard.nx*gameBoard.ny*gameBoard.nz];
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
		i = 0;
		j = 0;
		pass = 0;
		//CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
		
<<<<<<< HEAD
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
=======
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
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
			print ("rotated around " + centreRotation);

		}	
		
<<<<<<< HEAD
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
=======
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
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
			}
			//move right
			if (Input.GetKeyDown("right")){
				//board starts off at -0.5 in x-direction
<<<<<<< HEAD
				block.transform.position = new Vector3(block.transform.position.x + 1,block.transform.position.y,block.transform.position.z);
=======
				if(transform.position.x != (gameBoard.nx+0.5))
				transform.position = new Vector3(transform.position.x + 1,transform.position.y, transform.position.z);
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
				posX +=1;
				centreRotation = new Vector3(posX,posY,posZ);
			}
			//move left
			if (Input.GetKeyDown("left")){
				//board starts off at -0.5 in x-direction
<<<<<<< HEAD
				block.transform.position = new Vector3(block.transform.position.x - 1,block.transform.position.y,block.transform.position.z);
				posX -=1;
				centreRotation = new Vector3(posX,posY,posZ);
				print("moved left position.x =  " + block.transform.position.x +"  centre rotation = "+ centreRotation);
=======
				if(transform.position.x != (-(gameBoard.nx-0.5)))
				transform.position = new Vector3(transform.position.x - 1,transform.position.y, transform.position.z);
				posX -=1;
				centreRotation = new Vector3(posX,posY,posZ);
				print("moved left position.x =  " + transform.position.x +"  centre rotation = "+ centreRotation);
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
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
											//x,y,z
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
<<<<<<< HEAD
	}	
=======
	}
	
	private void addBlocks(int x, int z, GameObject cube){
		if (blocks[x + z*gameBoard.nz] == null){
			//print ("brick added to blocks["+(x+z*gameBoard.nz)+"]");
			blocks[x + z*gameBoard.nz] = cube;
		}
		else{
			x += gameBoard.nx*gameBoard.nz;
			addBlocks(x, z, cube);
		}
		return;
	}
	
	public void removeBlocks(int y){
		int x = gameBoard.nx;
		int z = gameBoard.nz;
		for (int k = y*x*z; k<((y+1)*x*z); k++){
			if (blocks[k] != null){
				Destroy (blocks[k]);
				blocks[k] = null;
			}
		}
		return;
	}
	
	
>>>>>>> e25cf66b14664b6c513f882809cc23b610fdbac3
}
