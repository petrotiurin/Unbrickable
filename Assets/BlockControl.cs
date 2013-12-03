using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] bricks;
	
	private Board gameBoard = new Board();
	
	private int i, j;
	
	private GameObject FragmentCube;

	// Use this for initialization
	void Start () {
		bricks = new GameObject[gameBoard.nx*gameBoard.ny*gameBoard.nz];
		i = 0;
		j = 0;
		CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
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
		
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "block";
		addToScene(cube);
		
		//drop a brick on each space on the board in order
		cube.transform.position = new Vector3(i, 6, j); //TODO: randomize?
		
		cube.AddComponent("BlockCollision");
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		//add brick to array representation of board.
		k = i;
		addBrick(k, j, cube);
			
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
	
	private void addBrick(int x, int z, GameObject cube){
		if (bricks[x + z*gameBoard.nz] == null){
			print ("brick added to bricks["+(x+z*gameBoard.nz)+"]");
			bricks[x + z*gameBoard.nz] = cube;
		}
		else{
			x += gameBoard.nx*gameBoard.nz;
			addBrick(x, z, cube);
		}
		return;
	}
	
	public void removeBricks(int y){
		int x = gameBoard.nx;
		int z = gameBoard.nz;
		for (int k = y*x*z; k<((y+1)*x*z); k++){
			if (bricks[k] != null){
				Destroy (bricks[k]);
				bricks[k] = null;
			}
		}
		return;
	}
	
	
}
