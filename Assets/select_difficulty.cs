using UnityEngine;
using System.Collections;

public class select_difficulty : MonoBehaviour
{
	public GameObject play;
	public GameObject scores;
	public GameObject quit;
	public GameObject easy;
	public GameObject medium;
	public GameObject hard;
	public GameObject back;

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			play.gameObject.SetActive(false);
			scores.gameObject.SetActive(false);
			quit.gameObject.SetActive(false);
			easy.gameObject.SetActive(true);
			medium.gameObject.SetActive(true);
			hard.gameObject.SetActive(true);
			back.gameObject.SetActive(true);
		}
	}
}