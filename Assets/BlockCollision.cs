using UnityEngine;
using System;
using System.Collections;

public class BlockCollision : MonoBehaviour {
	
	// Avoid script being triggered twice by multiple blocks.
	bool isTriggered = false;
	
	// Size of each individual pin.
	private float pinSize;
	// Size of shape in pins.
	private int shapeSize;
	Board gameBoard;
	
	// Initialization.
	void Start () {
		GameObject scene = GameObject.Find("Scene");
		gameBoard = scene.GetComponent<Board>();
	}
	
	public void setPinSize(float size){
		pinSize = size;
	}
	
	public void setShapeSize(int size){
		shapeSize = size;
	}
		
	// Update is called once per frame.
	void Update () {
	}
	
	// Calculate a layer number for particular pin in the given shape.
	private int layerPos(float contact, float pin){
		float pos = contact + 0.12F;
		return (int) Math.Round(pos/pin);
	}
	
	private void nextShape(){
		GameObject scene = GameObject.Find("Scene");
		BlockControl b = scene.GetComponent<BlockControl>();
		b.createShape();
	}
	
	/* On collision - send a request to the
	 * BlockCreation script to create a new block */
	void OnCollisionEnter(Collision collision) {
		rigidbody.isKinematic = true;
		
		if(collision.contacts[0].normal.y > 0 &&
			rigidbody.name == "ActiveBlock" && !isTriggered){
			
			isTriggered = true;
			float contactPoint = collision.contacts[0].point.y;
			
			// Loop through the shape pin by pin.
			for (int i = 0; i < Math.Pow(shapeSize, 3); i++){
				Transform childTransform = transform.FindChild("Current pin" + i.ToString());
				if (childTransform != null){
					GameObject pin = childTransform.gameObject;
					int layer = layerPos(contactPoint, pinSize);
					/* Following code assumes the max height is 3!
					 * TODO: make more robust
					 * assumes pin size 0<pin<1. */
					if (childTransform.localPosition.y == 0) layer += 1;
					else if(childTransform.localPosition.y > 0)	layer += 2;
					
					gameBoard.FillPosition(layer, pin);
				}
			}
			nextShape();
			Destroy(this.gameObject);
		}
    }
}
