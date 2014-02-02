using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	// Dimensions of the board in "shapes".
	public int nx = 5;	// width
	public int ny = 5;	// height
	public int nz = 5;	// depth 
	
	// Pins in one dimansion.
	private int pinsPerShape;
	
	private BlockControl blockCtrl;
	
	// Initialization.
	void Start () {
		
		blockCtrl = GetComponent<BlockControl>();
		pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		
		for (int i=0; i<ny; i++){
			blocksLayer[i] = new GameObject();
			String layerName = "Layer" + i;
			blocksLayer[i].name = layerName;
			addToScene(blocksLayer[i]);
		}
		
		DrawBoard();
		
		blockCtrl.createShape();
	}
	
	// Update is called once per frame.
	void Update ()
	{
	}
	
	// Create the base of the game board.
	private void DrawBoard(){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "base";
		
		// Blocks fall from (0,0) into bottom corner.
		cube.transform.position = new Vector3(2, -0.1F, 2);
		cube.transform.localScale = new Vector3(nx, 0.2F, nz);
		
		// Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = cube.transform;
		t.parent = scene.GetComponent<Transform>();
	}
	
	// Add a pin object to its respective layer.
	public void FillPosition(int layer, GameObject pin) {
		addBlocks(layer, pin);
		
		// Destroy the layer if it is full.
		if(blocksLayer[layer].transform.childCount == nx*nz*pinsPerShape*pinsPerShape){
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
		
		float blockSize = blockCtrl.pinSize;
		
		String layerName;
		for (int k = y + 1; k < ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				// Translate down only if blocks present.
				if (blocksLayer[k-1].transform.childCount > 0){
					// Easy to make smooth fall with "lerp".
					blocksLayer[k-1].transform.Translate(new Vector3(0, -blockSize, 0)); 
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
	}
	
	// Make an object a child of the Scene.
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = scene.GetComponent<Transform>();
	}
}

