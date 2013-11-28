using UnityEngine;
using System.Collections;

public class BlockMovement : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	/* On collision - send a request to the
	 * BlockCreation script to create a new block */
	void OnCollisionEnter(Collision collision) {
		rigidbody.isKinematic = true;
		GameObject scene = GameObject.Find("Scene");
    	BlockCreation script = (BlockCreation) scene.GetComponent(typeof(BlockCreation));
		script.CreateCube();
    }
}
