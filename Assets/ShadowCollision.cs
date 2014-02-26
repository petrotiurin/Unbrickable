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
		transform.position = pos;
		transform.rotation = rotation;
		collided = false;
	}
	
	void OnTriggerStay(Collider other){
		Debug.Log("Any shadow collision!");
		if (other.gameObject.transform.parent.gameObject.name == "ActiveBlock"||
			other.gameObject.name == "ActiveBlock"||
			other.gameObject.name == "base"){
			Debug.Log("Active block collision!");
			return;
		}
		collided = true;
		Debug.Log("True shadow collision!");
	}
}

