using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockCreation : MonoBehaviour {
	
	private GameObject currentObject;
    
	private List<GameObject> objects;

	// Use this for initialization
	void Start () {
		objects = new List<GameObject>();
		currentObject = CreateCube();
		objects.Add(currentObject);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public GameObject CreateCube() {
		if (objects.Count > 3) return null;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0, 0, -2);
		cube.transform.localScale = new Vector3(1, 1, 1);
		cube.AddComponent("BlockMovement");
		BoxCollider c = (BoxCollider)cube.GetComponent("BoxCollider");
		c.isTrigger = true;
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 2;
		cubeRigidBody.useGravity = true;
		currentObject = cube;
		objects.Add(currentObject);
		return cube;
	}
}
