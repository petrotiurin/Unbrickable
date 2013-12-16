using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private Board gameBoard = new Board();
	
	private int i, j, posX=2,posY=1,posZ=2, rotation=0;
	
	private Vector3 centreRotation = new Vector3 (2,1,2);
	
	private GameObject FragmentCube;

	// Use this for initialization
	void Start () {

		blocks = new GameObject[gameBoard.nx*gameBoard.ny*gameBoard.nz];
		i = 0;
		j = 0;
		CreateCube();
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
		int k;
		
		//wrap-around
		if (i == 5){
			i = 0;
			j = (j+1)%5;
		}
		
		GameObject cube = GameObject.Instantiate(Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject))) as GameObject;
		cube.name = "block";	
		
		//drop a brick on each space on the board in order
											//x,y,z
		cube.transform.position = new Vector3(i, 6, j); //TODO: randomize?
		
		
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		cube.AddComponent("BlockCollision");
		addToScene(cube);
		
		//add brick to array representation of board.
		k = i;
		addBlocks(k, j, cube);
		i++;
	}
	
	public Board getBoard(){
		return gameBoard;
	}
	
	//Makes given object a child of the "Scene"
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
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
	
	
}
