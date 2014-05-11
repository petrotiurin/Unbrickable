using UnityEngine;
using System.Collections;

public class MenuOption : MonoBehaviour
	
{
	
	public GameObject selected;
	void Awake(){
        //Time was paused when game ends.
        Time.timeScale = 1;
    }

	void OnMouseEnter(){
		renderer.enabled = false;
		selected.gameObject.SetActive(true);
	}	
}

