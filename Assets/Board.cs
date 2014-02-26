using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	// Dimensions of the board in "shapes".
	public int nx = 15;	// width
	public int ny = 15;	// height
	public int nz = 15;	// depth 
	
	private bool[,,] boardArray;
	
	//for rotating the board
	//ricky moved the board initialy so the bottom left == (0,0) so centre is this
	private Vector3 centreRotation = new Vector3 (2,1,2);
	
	private BlockControl blockCtrl;
	
	// Initialization.
	void Start () {
		
		boardArray = new bool[nx,ny,nz];
		Array.Clear(boardArray, 0, boardArray.Length);
		blockCtrl = GetComponent<BlockControl>();
		//pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		
		for (int i=0; i<ny; i++){
			blocksLayer[i] = new GameObject();
			String layerName = "Layer" + i;
			blocksLayer[i].name = layerName;
			addToScene(blocksLayer[i]);
		}
		
		//shadow layer
		GameObject slayer = new GameObject();
		String lName = "ShadowLayer";
		slayer.name = lName;
		addToScene(slayer);
		
		DrawBoard();
		
		blockCtrl.createShape();
	}
	
	public void PivotTo(GameObject o, Vector3 position){
	    Vector3 offset = o.transform.position - position;
	 
	    foreach (Transform child in o.transform)
	        child.transform.position += offset;
	 
	    o.transform.position = position;
	}
	
	// Update is called once per frame.
	void Update ()
	{
		//ROTATE right
		if (Input.GetKeyDown("x")){		
			//board has been moved (2,0,2) 2in x, 2in z-->> --^
			transform.RotateAround(centreRotation, Vector3.up, 90);
		}		
		//ROTATE left
		if (Input.GetKeyDown("z")){
			transform.RotateAround(centreRotation, Vector3.up, -90);
		}
	}
	
	// Create the base of the game board.
	private void DrawBoard(){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "base";
		
		// Blocks fall from (0,0) into bottom corner.
		cube.transform.localScale = new Vector3(nx, 0.2F, nz);
		cube.transform.position = new Vector3(6.0F, -0.1F, 6.0F);
	
		//set the center to be the pivot
		centreRotation = new Vector3 ((float)cube.transform.position.x,cube.transform.position.y,(float)cube.transform.position.z);
		PivotTo(cube,centreRotation);
		
		// Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = cube.transform;
		t.parent = scene.GetComponent<Transform>();
		cube.transform.Translate(0,-0.4f,0);
		//cube.transform.localPosition = new Vector3(cube.transform.position.x, -0.1f, cube.transform.position.z);
	}
	
	// Add a pin object to its respective layer.
	public void FillPosition(int layer, GameObject pin) {
		addBlocks(layer, pin);
		// Destroy the layer if it is full.
		if(blocksLayer[layer].transform.childCount == nx*nz){
			clearLayer(layer);
		}
	}
	
	// Clear the layer (i.e. reset the layer count to 0).
	public void clearLayer(int y) {
		foreach (Transform childTransform in blocksLayer[y].transform) {
		    Destroy(childTransform.gameObject);
		}
		Destroy(blocksLayer[y]);
		blocksLayer[y] = null; //probably redundant
		
		//float blockSize = blockCtrl.pinSize;
		
		String layerName;
		for (int k = y + 1; k < ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				// Translate down only if blocks present.
				if (blocksLayer[k-1].transform.childCount > 0){
					// Easy to make smooth fall with "lerp".
					blocksLayer[k-1].transform.Translate(new Vector3(0, -1, 0)); //assuming size 1
				}
				layerName = "Layer" + (k - 1);
				blocksLayer[k-1].name = layerName;
			}
		}
		// Add an empty(removed) layer on top.
		blocksLayer[ny-1] = new GameObject();
		layerName = "Layer" + (ny-1);
		blocksLayer[ny-1].name = layerName;
		addToScene(blocksLayer[ny-1]);
		return;
	}
	
	// Add blocks to the layer.
	private void addBlocks(int layer, GameObject cube){
		cube.name = "Block";
	    cube.transform.parent = blocksLayer[layer].transform;
		int x = (int)Math.Round(cube.transform.position.x) + 1;
		int z = (int)Math.Round(cube.transform.position.z) + 1;
		boardArray[x,layer,z] = true;
		//Debug.Log(x+" "+layer+" "+z);
	}
	
	public bool checkPosition(int x, int y, int z){
		if (y < 15)	return boardArray[x,y,z];
		return false;
	}
	
	public void printArray(){
		Debug.Log(boardArray[8,1,9]);
		Debug.Log(boardArray[8,1,10]);
		Debug.Log(boardArray[8,1,7]);
	}
	
	// Make an object a child of the Scene.
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = scene.GetComponent<Transform>();
	}
}

