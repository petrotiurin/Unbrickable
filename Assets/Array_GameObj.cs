using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	
	public int[] suggestedPieces;
	
	//total number of pieces to choose from.
	public int piecesNum = 4;
	//number of pieces that are suggested to player.
	public int noOfSuggestedPieces = 3;
	
	// Use this for initialization
	void Start () {
		piecesNum = 4;
		suggestedPieces = new int[noOfSuggestedPieces];

		SuggestLegoPiece();
	}

	// Creates an array that holds the indices of lego pieces generated
	// 	pseudo-randomly. This is accessed from Board.cs and displayed.
	public void SuggestLegoPiece () {
		//flashPieces looks ugly.
		//flashPieces();

		for (int i = 0; i < noOfSuggestedPieces; i++)
	        suggestedPieces[i] = Random.Range(0, piecesNum);
	}

	//Effect where different images flash in the suggestion boxes till it
	//   stops and the real pieces are shown
	/*
	void flashPieces(){
		//generate arrays
		//timegap of 0.5s or so
		//loop few times
		
		for(int i = 0; i < 50; i++){
			for (int j = 0; j < noOfSuggestedPieces; j++)
			{
	        	suggestedPieces[j] = Random.Range(0, piecesNum);
			}
			StartCoroutine(timeDelay());
		}
	}

	IEnumerator timeDelay() {
        yield return new WaitForSeconds(5f);
        for (int j = 0; j < noOfSuggestedPieces; j++)
        	suggestedPieces[j] = Random.Range(0, piecesNum);
        yield return new WaitForSeconds(5f);
        for (int j = 0; j < noOfSuggestedPieces; j++)
        	suggestedPieces[j] = Random.Range(0, piecesNum);
        yield return new WaitForSeconds(5f);
        for (int j = 0; j < noOfSuggestedPieces; j++)
        	suggestedPieces[j] = Random.Range(0, piecesNum);
        yield return new WaitForSeconds(5f);
        for (int j = 0; j < noOfSuggestedPieces; j++)
        	suggestedPieces[j] = Random.Range(0, piecesNum);
    }

    */
}
