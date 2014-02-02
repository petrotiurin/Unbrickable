using UnityEngine;
using System;
using System.Collections;

public class BlockCollision : MonoBehaviour {
	
	// avoid script being triggered twice by multiple blocks
	bool isTriggered = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	private int layerPos(float contact, float pin){
		float pos = contact + 0.12F;
		return (int) Math.Round(pos/pin);
	}
	
	/* On collision - send a request to the
	 * BlockCreation script to create a new block */
	void OnCollisionEnter(Collision collision) {
		int x,y,z;
		
		rigidbody.isKinematic = true;
		if(collision.contacts[0].normal.y > 0){
			
			if(rigidbody.name == "ActiveBlock" && !isTriggered){
				isTriggered = true;
				//get pin size
				Transform pin = transform.FindChild("Current pin0");
				float pinSize = pin.renderer.bounds.size.x;
				Debug.Log(pinSize);
				float contactPoint = collision.contacts[0].point.y;
				
				GameObject scene = GameObject.Find("Scene");
		    	Board gameBoard = (Board) scene.GetComponent(typeof(Board));
				int shapeSize = gameBoard.nx;
				for (int i = 0; i<shapeSize*shapeSize*shapeSize; i++){
					Transform childTransform = transform.FindChild("Current pin"+i.ToString());
					if (childTransform != null){
						GameObject cube = childTransform.gameObject;
						x = (int) Math.Round(rigidbody.position.x + childTransform.position.x);
						y = layerPos(contactPoint, pinSize);//determines the layer
						//following code assumes the max height is 3! TODO: make more robust
						//assumes pin size 0<pin<1
						if (childTransform.localPosition.y == 0){
							y += 1;
						} else if(childTransform.localPosition.y > 0){
							y += 2;
						}
						z = (int) Math.Round(rigidbody.position.z + childTransform.position.z);
						gameBoard.FillPosition(x,y,z,cube);
					}
				}
				gameBoard.createShape();
				Destroy(this.gameObject);
			}
		}
    }
}
