using UnityEngine;
using System.Collections;

public class BlockCreation : MonoBehaviour {
	
	private GameObject currentObject;
    

	// Use this for initialization
	void Start () {
		currentObject = CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public GameObject CreateCube() {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0, 0, -2);
		cube.transform.localScale = new Vector3(1, 1, 1);
		cube.AddComponent("BlockMovement");
		BoxCollider c = (BoxCollider)cube.GetComponent("BoxCollider");
		c.isTrigger = true;
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 2;
		cubeRigidBody.useGravity = true;
		return cube;
	}
}
