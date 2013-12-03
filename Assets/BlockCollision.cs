using UnityEngine;
using System.Collections;

public class BlockCollision : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	/* On collision - send a request to the
	 * BlockCreation script to create a new block */
	void OnCollisionEnter(Collision collision) {
		int y;
		
		if(collision.contacts[0].normal.y > 0){
			rigidbody.isKinematic = true;
			
			GameObject scene = GameObject.Find("Scene");
	    	BlockControl script = (BlockControl) scene.GetComponent(typeof(BlockControl));
			Board gameBoard = script.getBoard();
			
			//get height and add to the layer count
			y = (int) rigidbody.position.y;
			gameBoard.FillPosition(y);
			
			//destroy the layer if it is full
			if(gameBoard.layer[y] == gameBoard.nx*gameBoard.nz){
				gameBoard.clearLayer(y);
				script.removeBlocks(y);
			}
			
			script.CreateCube();
		}
    }
	
}
