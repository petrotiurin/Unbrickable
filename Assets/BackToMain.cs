using UnityEngine;
using System.Collections;

public class BackToMain : MonoBehaviour{
	public GameObject play;
	public GameObject scores;
	public GameObject quit;
	public GameObject MOB_title;
	
	public GameObject highscores_title;
	
	public GameObject easy;
	public GameObject medium;
	public GameObject hard;
	
	public GameObject back;
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			back.gameObject.SetActive(false);
			highscores_title.gameObject.SetActive(false);
			easy.gameObject.SetActive(false);
			medium.gameObject.SetActive(false);
			hard.gameObject.SetActive(false);
			
			MOB_title.renderer.enabled = true;
			play.gameObject.SetActive(true);
			scores.gameObject.SetActive(true);
			quit.gameObject.SetActive(true);
		}
	}
}