using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockCreation : MonoBehaviour {
	
	private GameObject currentObject; //can be used in future
    
	private List<GameObject> objects; //to keep track of objects on the scene

	// Use this for initialization
	void Start () {
		objects = new List<GameObject>();
		CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	/* Creates a new cube at the tob of our scene.
	 * Works only if the last object stopped moving. */
	public void CreateCube() {
		//just to avoid the infinite loop.
		if (objects.Count > 3) return;
		//to avoid calls when object is still moving
		if (currentObject != null &&
			currentObject.rigidbody.isKinematic == false) return;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		addToScene(cube);
		cube.transform.position = new Vector3(0, 0, -5); //TODO: randomize?
		//cube.transform.localScale = new Vector3(1, 1, 1);
		cube.AddComponent("BlockMovement");
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 2;
		cubeRigidBody.useGravity = true;
		currentObject = cube;
		objects.Add(cube);
	}
	
	//for future use
	public GameObject getCurrentObject(){
		return currentObject;
	}
	
	//Makes given object a child of the "Scene"
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
}
