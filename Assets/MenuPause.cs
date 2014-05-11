using UnityEngine;
using System.Collections;

public class MenuPause : MonoBehaviour
{
	public GameObject play;
	public GameObject scores;
	public GameObject quit;

	bool startFlag = true;
	
	private float timeLeft;

	void Awake(){
		Time.timeScale = 1;
	}

	void Start(){
		timeLeft = 3.0f;
	}
	
	void Update(){
		if (startFlag){
			timeLeft -= Time.deltaTime;
	        if ( timeLeft < 0 )
			{
	            startFlag = false;
				play.gameObject.SetActive(true);
				scores.gameObject.SetActive(true);
				quit.gameObject.SetActive(true);
	        }
		}
		return;
	}
}

