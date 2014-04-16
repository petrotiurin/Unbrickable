using UnityEngine;
using System.Collections;

public class MenuOptionSelected : MonoBehaviour{
	
	public GameObject option;
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			
			if(tag == "NewGame")
				Application.LoadLevel("Scene1");
			else if(tag == "Finish")
				Application.Quit();
		}
	}
	
	void OnMouseExit(){
		this.gameObject.SetActive(false);
		option.renderer.enabled = true;
	}
}