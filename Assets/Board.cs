using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	public int nx = 5;	/* width  */
	public int ny = 5;	/* height */
	public int nz = 5;	/* depth  */
	//pins in one dimansion
	private int pinsPerShape;
	private BlockControl blockCtrl;
	
	void Start () {
		
		blockCtrl = new BlockControl();
		pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		
		for (int i=0; i<ny; i++){
			blocksLayer[i] = new GameObject();
			String layerName = "Layer" + i;
			blocksLayer[i].name = layerName;
			addToScene(blocksLayer[i]);
		}
		
		DrawBoard();
		
		
		//blockCtrl.CreateCube();
		blockCtrl.createShape();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
		if(blocksLayer[j].transform.childCount == nx*nz*pinsPerShape*pinsPerShape){
			clearLayer(j);
		}
		
		//release the next cube
		//blockCtrl.CreateCube();
		//blockCtrl.createShape();
	}
	public void FillPosition(int i, int j, int k, GameObject cube) {
		addBlocks(i, j, k, cube);
		
		//destroy the layer if it is full
		if(blocksLayer[j].transform.childCount == nx*nz*pinsPerShape*pinsPerShape){
			Debug.Log(nx*nz*pinsPerShape*pinsPerShape);
			clearLayer(j);
		}
	}
	
	//TODO: get rid of this shit
	public void createShape(){
		//release the next shape
		blockCtrl.createShape();
	}
	
	//clear the layer (i.e. reset the layer count to 0)
	public void clearLayer(int y) {
		foreach (Transform childTransform in blocksLayer[y].transform) {
		    Destroy(childTransform.gameObject);
		}
		Destroy(blocksLayer[y]);
		blocksLayer[y] = null; //probably redundant
		
		float blockSize = blockCtrl.pinSize;
		
		String layerName;
		for (int k = y+1; k<ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				//translate down only if blocks present
				if (blocksLayer[k-1].transform.childCount > 0){
					Debug.Log("Translation hppened!");
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
		//Debug.Log(j);
	    cube.transform.parent = blocksLayer[j].transform;
	}
	
	private void addBlocks(int i, int j, int k, GameObject cube){
		cube.name = "Block";
	    cube.transform.parent = blocksLayer[j].transform;
	}
	
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
}

