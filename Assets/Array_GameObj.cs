﻿using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	
	public GameObject[] legoPieces;
	
	public int piecesNum = 5;
	
	//pre-defined position to show pieces on
	private Vector3 position1, position2, position3;
	
	// Use this for initialization
	void Start () {
		
		legoPieces = new GameObject[piecesNum];
		
		position1 = new Vector3(0,0,0);
		position2 = new Vector3(2,0,0);
		position3 = new Vector3(2,0,2);
		SuggestLegoPiece();
	}
	
	//Makes given cube a child of the current shape
	private void addToShape(Transform shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = shape;
	}
	
	void InvokePiece(int piece, Vector3 pos){
		string prefab = "Assets/Blocks/";
		switch(piece){
		case 0: prefab+="L2x1"; break;
		case 1: prefab+="L2x2"; break;
		case 2: prefab+="L3x2"; break;
		case 3: prefab+="L4x2"; break;
		case 4: prefab+="L6x1"; break;
		default: throw new System.ArgumentException("Unrecognised piece number.");
		}
		prefab += ".prefab";
		Object obj = Resources.LoadAssetAtPath(prefab, typeof(GameObject));
		GameObject pieceObject = GameObject.Instantiate(obj) as GameObject;
		addToShape(transform, pieceObject);
		pieceObject.transform.LookAt(transform.forward);
		pieceObject.transform.localPosition = pos;
		pieceObject.transform.localScale = new Vector3(20,20,20);
		legoPieces[piece] = pieceObject;
	}
	
	/*void Update(){
		SuggestLegoPiece();
	}*/
	
	// Update is called once per frame
	public void SuggestLegoPiece () {

		int piece1, piece2, piece3; //stores the index of the piece to display
		int numberOfPieces; //stores if to generate 2 or 3 pieces

		foreach (GameObject piece in legoPieces){
			Destroy(piece);
		}
		
        numberOfPieces = Random.Range(2,4);

        piece1 = Random.Range(0, legoPieces.Length);
		piece2 = Random.Range(0, legoPieces.Length);
		
		InvokePiece(piece1,position1);
		
		while(piece1 == piece2)
			piece2 = Random.Range(0, legoPieces.Length);
		
		InvokePiece(piece2,position2);
		
		if(numberOfPieces == 3) {
			piece3 = Random.Range(0, legoPieces.Length);
		
			while(piece3 == piece1 || piece3 == piece2)
				piece3 = Random.Range(0, legoPieces.Length);

			InvokePiece(piece3,position3);
		}
		
		//Debug.Log("Random nos: " + piece1 + " & " + piece2);
	}

	// Combine pieces and make it fall on the surface. Used primarily
	//		for testing during development.
	void combinePieces () {
		/* Get index of the 2-3 pieces so you know which pieces to combine.
		*/
	}
}
