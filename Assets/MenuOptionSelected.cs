using UnityEngine;
using System.Collections;

public class MenuOptionSelected : MonoBehaviour
{	
	public GameObject option;
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			
			if(tag == "Easy")
				Application.LoadLevel("space");
			else if(tag == "Medium")
				Application.LoadLevel("pirate");
			else if(tag == "Hard")
				Application.LoadLevel("space");
			else if(tag == "Finish")
				Application.Quit();
		}
	}
	
	void OnMouseExit(){
		this.gameObject.SetActive(false);
		option.gameObject.SetActive(true);
	}
}