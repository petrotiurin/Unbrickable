using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	//stores indices of the pieces that are suggested
	public int[] suggestedPieces;
	
	//total number of pieces recognised by game
	public int piecesNum = 4;
	//number of pieces that are suggested
	public int noOfSuggestedPieces = 3;
	
	// Use this for initialization
	void Start () {
		piecesNum = 4;
		//legoPieces = new GameObject[noOfSuggestedPieces];
		suggestedPieces = new int[noOfSuggestedPieces];
		SuggestLegoPiece();
	}
	
	// This function stores the random indices of lego pieces in an array.
	//This array is read in Board.cs and displayed.
	public void SuggestLegoPiece () {

		int suggestedPiece;

		//destroy gameobjects (suggested lego pieces) from the previous run.

		/*foreach (GameObject piece in legoPieces){
			Destroy(piece);
		}*/

		flashPieces();

		for (int i = 0; i < noOfSuggestedPieces; i++)
		{
			//TODO: Implement better pseudorandom number generator?
	        suggestedPiece = Random.Range(0, piecesNum);
	        Debug.Log("Piece no = " + suggestedPiece);
	        suggestedPieces[i] = suggestedPiece;
		}
	}

	void flashPieces(){
		//store random images in suggestesPieces[]
		//add half a second time gap
	}
}
