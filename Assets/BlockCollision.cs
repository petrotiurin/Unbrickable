using UnityEngine;
using System;
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
		int x,y,z;
		
		if(collision.contacts[0].normal.y > 0){
			rigidbody.isKinematic = true;
			
			if(rigidbody.name == "ActiveBlock"){
				GameObject scene = GameObject.Find("Scene");
		    	Board gameBoard = (Board) scene.GetComponent(typeof(Board));
				
				//get position and add to the layer count				
				x = (int) Math.Round(rigidbody.position.x);
				y = (int) Math.Round(rigidbody.position.y);
				z = (int) Math.Round(rigidbody.position.z);
				gameBoard.FillPosition(x,y,z);
			}
		}
		else
			rigidbody.isKinematic = true;
    }
}
