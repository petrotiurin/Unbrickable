using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	public int nx = 5;	/* width  */
	public int ny = 5;	/* height */
	public int nz = 5;	/* depth  */
	
	//for rotating the board
	private Vector3 centreRotation = new Vector3 (2,1,2);
	
	private BlockControl blockCtrl;
	
	
	
	void Start () {
		blocksLayer = new GameObject [ny];
		
		for (int i=0; i<ny; i++){
			blocksLayer[i] = new GameObject();
			String layerName = "Layer" + i;
			blocksLayer[i].name = layerName;
			addToScene(blocksLayer[i]);
		}
		
		DrawBoard();
		
		blockCtrl = new BlockControl();
		blockCtrl.CreateCube();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		//ROTATE right
		if (Input.GetKeyDown("x")){		
			//board has been moved (2,0,2) 2in x, 2in z-->> --^
			transform.RotateAround(centreRotation, Vector3.up, 90);
			print ("rotated around " + centreRotation);
		}		
		//ROTATE left
		if (Input.GetKeyDown("z")){
			transform.RotateAround(centreRotation, Vector3.up, -90);
			print ("rotated around " + centreRotation);

		}	
	}
	
	//create the base of the game board
	private void DrawBoard(){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "base";
		
		// blocks fall from (0,0) into bottom corner
		cube.transform.position = new Vector3(2, -0.1F, 2);
		cube.transform.localScale = new Vector3(nx, 0.2F, nz);
		
		//Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = cube.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
	
	//increase the layer count by one
	public void FillPosition(int i, int j, int k) {
		addBlocks(i, j, k);
		
		//destroy the layer if it is full
		if(blocksLayer[j].transform.childCount == nx*nz){
			clearLayer(j);
		}
		
		//release the next cube
		blockCtrl.CreateCube();
	}
	
	//clear the layer (i.e. reset the layer count to 0)
	public void clearLayer(int y) {
		foreach (Transform childTransform in blocksLayer[y].transform) {
		    Destroy(childTransform.gameObject);
		}
		Destroy(blocksLayer[y]);
		blocksLayer[y] = null; //probably redundant
		
		GameObject cube = GameObject.Find("Block");
		float blockSize = cube.transform.localScale.x;
		
		String layerName;
		for (int k = y+1; k<ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				//translate down only if blocks present
				if (blocksLayer[k-1].transform.childCount > 0){
					blocksLayer[k-1].transform.Translate(new Vector3(0,-blockSize,0)); //easy to make smooth fall
				}
				layerName = "Layer" + (k-1);
				blocksLayer[k-1].name = layerName;
			}
		}
		//add an empty(removed) layer on top
		blocksLayer[ny-1] = new GameObject();
		layerName = "Layer" + (ny-1);
		blocksLayer[ny-1].name = layerName;
		addToScene(blocksLayer[ny-1]);
		return;
	}

	private void addBlocks(int i, int j, int k){
		GameObject cube = GameObject.Find("ActiveBlock");
		cube.name = "Block";
	    cube.transform.parent = blocksLayer[j].transform;
		
		//print("position: Y=" + j + "   X=" + i + "  Z=" + k);
	}
	
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
}

