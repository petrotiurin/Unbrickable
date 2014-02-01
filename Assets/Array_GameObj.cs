using UnityEngine;
using System.Collections;

public class Array_GameObj : MonoBehaviour {
	
	public GameObject[] legoPieces;
	
	// Use this for initialization
	void Start () {
		legoPieces = GameObject.FindGameObjectsWithTag("Lego");
		
		for(int i = 0; i < legoPieces.Length; i++)
        {
        	legoPieces[i].renderer.enabled = false;
            Debug.Log("This is number: "+i);
        }

        InvokeRepeating("SuggestLegoPiece", 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void SuggestLegoPiece () {

		int i; //for-loop
		int piece1, piece2, piece3; //stores the index of the piece to display
		int numberOfPieces; //stores if to generate 2 or 3 pieces

		for(i = 0; i < legoPieces.Length; i++)
        	legoPieces[i].renderer.enabled = false;

        numberOfPieces = Random.Range(2,4);

        piece1 = Random.Range(0, legoPieces.Length);
		piece2 = Random.Range(0, legoPieces.Length);

		while(piece1 == piece2)
			piece2 = Random.Range(0, legoPieces.Length);

		if(numberOfPieces == 3) {
			piece3 = Random.Range(0, legoPieces.Length);
		
			while(piece3 == piece1 || piece3 == piece2)
				piece3 = Random.Range(0, legoPieces.Length);

			legoPieces[piece3].renderer.enabled = true;
		}

		legoPieces[piece1].renderer.enabled = true;
		legoPieces[piece2].renderer.enabled = true;

		
		Debug.Log("Random nos: " + piece1 + " & " + piece2);
	}

	// Combine pieces and make it fall on the surface. Used primarily
	//		for testing during development.
	void combinePieces () {
		/* Get index of the 2-3 pieces so you know which pieces to combine.
		*/

	}
}
