using UnityEngine;
using System.Collections;

public class MenuOption : MonoBehaviour
	
{
	
	public GameObject selected;
	
	void OnMouseEnter(){
		renderer.enabled = false;
		selected.gameObject.SetActive(true);
	}	
}

