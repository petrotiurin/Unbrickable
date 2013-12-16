using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour {
	
	public int nx = 5;	/* width  */
	public int ny = 5;	/* height */
	public int nz = 5;	/* depth  */
	
	public int[] layer = new int[5];
	
	
	void Start () {
		DrawBoard();
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
		cube.transform.position = new Vector3(2, 0, 2);
		cube.transform.localScale = new Vector3(nx, 0.2F, nz);
		
		//Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = cube.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
	
	//increase the layer count by one
	public void FillPosition(int y) {
		layer[y] ++;
	}
	
	//clear the layer (i.e. reset the layer count to 0)
	public void clearLayer(int y) {
		layer[y] = 0;
	}
	
}

