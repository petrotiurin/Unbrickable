using UnityEngine;
using System.Collections;

public class highscores : MonoBehaviour
{
	public GameObject play;
	public GameObject scores;
	public GameObject quit;
	public GameObject MOB_title;
	public GameObject highscores_title;
	public GameObject back;
	
	//TODO: add leaderboard top 3

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			MOB_title.renderer.enabled = false;
			play.gameObject.SetActive(false);
			scores.gameObject.SetActive(false);
			quit.gameObject.SetActive(false);
			highscores_title.gameObject.SetActive(true);
			back.gameObject.SetActive(true);
		}
	}
}