using UnityEngine;
using System.Collections;

public class MenuOption : MonoBehaviour
{
	public GameObject selected;

	void OnMouseEnter(){
		this.gameObject.SetActive(false);
		selected.gameObject.SetActive(true);
	}	
}