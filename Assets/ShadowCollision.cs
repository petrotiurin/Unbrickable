using UnityEngine;
using System;
using System.Collections;

public class ShadowCollision : MonoBehaviour {
	
	private bool collided = false;
	
	// Use this for initialization
	void Start () {
	}
	
	void Update(){
	}
	
	public bool isCollided(){
		return collided;
	}
	
	public void reset(Vector3 pos, Quaternion rotation){
		collided = false;
		transform.position = pos;
		transform.rotation = rotation;
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.transform.parent.name == "ActiveBlock") return;
		collided = true;
	}
}

