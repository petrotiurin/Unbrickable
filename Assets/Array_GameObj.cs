using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	
	public GameObject[] legoPieces;
	public int[] suggestedPieces;
	
	//total number of pieces to choose from.
	public int piecesNum = 4;
	public int noOfSuggestedPieces = 3;
	
	//pre-defined position to show pieces on
	private Vector3[] position;//1, position2, position3;
	
	// Use this for initialization
	void Start () {
		piecesNum = 4;
		legoPieces = new GameObject[noOfSuggestedPieces];
		suggestedPieces = new int[noOfSuggestedPieces];

		position = new Vector3[noOfSuggestedPieces];
		position[0] = new Vector3(-8.7f,3,3f);
		position[1] = new Vector3(-6.2f,3,3f);
		position[2] = new Vector3(-3.5f,3,3f);
		SuggestLegoPiece();
	}
	
	//Makes given cube a child of the current shape
	private void addToShape(Transform shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = shape;
	}
	
	void InvokePiece(int pieceno, int piece, Vector3 pos){
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
		legoPieces[pieceno] = pieceObject;
		suggestedPieces[pieceno] = piece;
	}
	
	/*void Update(){
		SuggestLegoPiece();
	}*/
	
	// Update is called once per frame
	public void SuggestLegoPiece () {

		int suggestedPiece;

		//destroy gameobjects (suggested lego pieces) from the previous run.

		foreach (GameObject piece in legoPieces){
			Destroy(piece);
		}
		
		for (int i = 0; i < noOfSuggestedPieces; i++)
		{
			//flashPieces(i);
	        suggestedPiece = Random.Range(0, piecesNum);
	        //Debug.Log("Piece no = " + suggestedPiece);
	        suggestedPieces[i] = suggestedPiece;
			//InvokePiece(i, suggestedPiece, position[i]);
		}
	}

	// Combine pieces and make it fall on the surface. Used primarily
	//		for testing during development.
	void combinePieces () {
		/* Get index of the 2-3 pieces so you know which pieces to combine.
		*/
	}
}
