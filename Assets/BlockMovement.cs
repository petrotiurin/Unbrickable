using UnityEngine;
using System.Collections;

public class BlockMovement : MonoBehaviour {
	
	
	private System.Diagnostics.Stopwatch stopwatch;
	private bool moving = true;
	
	// Use this for initialization
	void Start () {
		stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();
	}
	
	
	// Update is called once per frame
	void Update () {
		if (moving && stopwatch.ElapsedMilliseconds > 10*60){
			this.transform.Translate(0, -1, 0);
			stopwatch.Reset();
			stopwatch.Start();
		}
	}
	
	void OnTriggerEnter(Collider other) {
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
        Stop();
		GameObject scene = GameObject.Find("Scene");
    	BlockCreation script = (BlockCreation) scene.GetComponent(typeof(BlockCreation));
		script.CreateCube();
    }
	
	void OnTriggerStay (Collider other)
    {  
		
    }
        
  	void OnTriggerExit (Collider other)
    {
     
    }
	
	void Stop(){
		moving = false;
		stopwatch.Stop();
		stopwatch.Reset();
	}
}
