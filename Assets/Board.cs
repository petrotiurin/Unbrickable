using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	// Dimensions of the board in "shapes".
	public int nx = 17;	// width
	public int ny = 15;	// height
	public int nz = 17;	// depth 
	
	private bool[,,] boardArray;
	
	//for rotating the board
	//ricky moved the board initialy so the bottom left == (0,0) so centre is this
	private Vector3 centreRotation = new Vector3 (2,1,2);

	/*[DllImport ("make2")]
	private static extern int main();*/

	[DllImport ("make2")]
	private static extern IntPtr lego();

	[DllImport ("make2")]
	private static extern int main();


	private BlockControl blockCtrl;
	
	// Initialization.
	void Awake () {


		print(main());
		print(Marshal.PtrToStringAnsi(lego()));
		print ("HIERHIEHRIE");

		boardArray = new bool[nx,ny,nz];
		Array.Clear(boardArray, 0, boardArray.Length);
		blockCtrl = GetComponent<BlockControl>();
		//pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		
		//creating the bounding walls in the array
		for(int i=0; i< ny;i++){
			for(int j=0;j < nx;j++){
				boardArray[j,i,0] = true;
				boardArray[j,i,nz-1] = true;
				boardArray[0,i,j] = true;
				boardArray[nx-1,i,j] = true;
				//print (boardArray[j,i,0]);
			}
		}
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
	
	}

	void Start(){
		
		blockCtrl.createShape();

	}
	
	public void PivotTo(GameObject o, Vector3 position){
	    Vector3 offset = o.transform.position - position;
	 
	    foreach (Transform child in o.transform)
	        child.transform.position += offset;
	 
	    o.transform.position = position;
	}
	
	// Update is called once per frame.
	void Update (){
		
	}
	
	// Create the base of the game board.
	private void DrawBoard(){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "base";
		
		// Blocks fall from (0,0) into bottom corner.
		cube.transform.localScale = new Vector3(nx-2, 0.2F, nz-2);
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
		if(blocksLayer[layer].transform.childCount == (nx-2)*(nz-2)){
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
		
			
		//remove the layer from the board array
		//TODO: change so that it can delete multiple layers
		//nx=17    x,y,z
		for(int i=1; i< nx-1;i++){
			for(int j=1;j < nz-1;j++){
				boardArray[i,y,j] = false;
			}
		}
		
		//from where the layer has been deleted, move all of the layers down one IN THE ARRAY
		for(int k = y;k<ny-1;k++){
			for(int i=1; i< nx-1;i++){
				for(int j=1;j < nz-1;j++){
					boardArray[i,k,j] =boardArray[i,k+1,j] ;
				}
			}
		}
		//clearing the top layer
		for(int i=1; i< nx-1;i++){
			for(int j=1;j < nz-1;j++){
				boardArray[i,ny-1,j] =false ;
			}
		}
		
		
		
		//float blockSize = blockCtrl.pinSize;
		
		//for moving the rest of the board down
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
		int x = (int)Math.Round(cube.transform.position.x) + 2;
		int z = (int)Math.Round(cube.transform.position.z) + 2;
		boardArray[x,layer,z] = true;
		//Debug.Log(x+" "+layer+" "+z);
	}
	
	public bool checkPosition(int x, int y, int z){
		if (y < 15)	return boardArray[x,y,z];
		return false;
	}
	
	public void printArray(){
		print("******A collision has happened,,,,,,,the board array looks like this ------->>>>>>");
		for(int k=0;k<ny;k++){
			for(int i=1; i< nx-2;i++){
				for(int j=1;j < nz-2;j++){
					if(boardArray[i,k,j]==true) print("x="+i+" y="+k+" z="+j);
						//Debug.Log(boardArray[i,k,j]);
				}
			}
		}
	}
	
	// Make an object a child of the Scene.
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = scene.GetComponent<Transform>();
	}
}

