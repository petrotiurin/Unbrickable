using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	public int nx = 5;	/* width  */
	public int ny = 5;	/* height */
	public int nz = 5;	/* depth  */
	
	public int[] layer;
	
	private BlockControl blockCtrl;
	
	void Start () {
		layer = new int[ny];
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
		layer[j] ++;
		
		addBlocks(i, j, k);
		
		//destroy the layer if it is full
		if(layer[j] == nx*nz){
			clearLayer(j);
		}
		
		//release the next cube
		blockCtrl.CreateCube();
	}
	
	//clear the layer (i.e. reset the layer count to 0)
	public void clearLayer(int y) {
		layer[y] = 0;
		removeBlocks (y);
		
	}

	private void addBlocks(int i, int j, int k){
		
		GameObject cube = GameObject.Find("ActiveBlock");
		cube.name = "Block";
		
		if(blocksLayer[j] == null)
			blocksLayer[j] = cube;
		else
			cube.transform.parent = blocksLayer[j].transform;
		
		print("position: Y=" + j + "   X=" + i + "  Z=" + k);
	}

	public void removeBlocks(int y){
		foreach (Transform childTransform in blocksLayer[y].transform) {
		    Destroy(childTransform.gameObject);
		}
		blocksLayer[y] = null;
		
		for (int k = y+1; k<ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				blocksLayer[k] = null;
				Rigidbody layerRigidBody = blocksLayer[k-1].AddComponent<Rigidbody>();
				layerRigidBody.mass = 1;
				layerRigidBody.useGravity = true;
				blocksLayer[k].AddComponent("BlockCollision");
			}
		}
		return;
	}
	
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}	
/*
	public void removeBlocks(int y){		
		for (int k = y*nx*nz; k<(ny*nx*nz); k++){
			if (k<((y+1)*nx*nz)){
				if (blocks[k] != null){
					Destroy (blocks[k]);
					blocks[k] = null;
				}
			}else{
				if (blocks[k] != null){
					blocks[k-(nx*nz)] = blocks[k];
					blocks[k] = null;
					blocks[k-(nx*nz)].rigidbody.isKinematic = false;
					//Rigidbody cubeRigidBody = blocks[k-(nx*nz)].GetComponent<Rigidbody>();
					//cubeRigidBody.isKinematic = false;
				}
			}
		}
		return;
	}
*/
}

