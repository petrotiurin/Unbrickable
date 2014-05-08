using UnityEngine;
using System.Collections;

public class MusicHandler : MonoBehaviour {

	private static MusicHandler instance = null;

	public static MusicHandler Instance {
		get { return instance; }
	}

	// Check if instance already exists and destroy if yes
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		GameObject cam = GameObject.Find("Main Camera");
		instance.transform.position = cam.transform.position;
	}
}
