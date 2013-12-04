using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private Board gameBoard = new Board();
	
	private int i, j;
	
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
			print ("brick added to blocks["+(x+z*gameBoard.nz)+"]");
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
