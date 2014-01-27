using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private int i, j, pass;
	
	private GameObject FragmentCube;

	// Use this for initialization
	void Start () {
		i = 0;
		j = 0;
		pass = 0;
		//CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	
	/* Creates a new cube at the top of our scene.
	 * Works only if the last object stopped moving. */
	public void CreateCube() {
		
		if (pass == 0 && (i + j*5) == 24){
			i++;
			pass=1;
		}
		
		//wrap-around
		if (i == 5){
			i = 0;
			j = (j+1)%5;
		}
		
		GameObject cube = GameObject.Instantiate(Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject))) as GameObject;
		cube.name = "ActiveBlock";	
		
		//drop a brick on each space on the board in order
		cube.transform.position = new Vector3(i, 6, j); //TODO: randomize?
		
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		cube.AddComponent("BlockCollision");
		addToScene(cube);
		
		i++;
	}
	
	//Makes given object a child of the "Scene"
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}	
}
