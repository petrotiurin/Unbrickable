using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	
	//Stores indices of the suggested pieces.
	public int[] suggestedPieces;
	//total number of pieces to choose from.
	public int piecesNum = 4;
	//number of pieces that are suggested to player.
	public int noOfSuggestedPieces = 2;
	
	// Use this for initialization
	void Start () {
		piecesNum = 4;
		suggestedPieces = new int[noOfSuggestedPieces];

		SuggestLegoPiece();
	}

	// Creates an array that holds the indices of lego pieces generated
	// 	pseudo-randomly. This is accessed from Board.cs and displayed.
	public void SuggestLegoPiece () {
		suggestedPieces = new int[noOfSuggestedPieces];

		for (int i = 0; i < noOfSuggestedPieces; i++){
	        suggestedPieces[i] = Random.Range(0, piecesNum);
		}
		if(noOfSuggestedPieces == 2){
			while (suggestedPieces[1] == suggestedPieces[0]){
				suggestedPieces[1] = Random.Range(0, piecesNum);
			}
		}

	}
}
