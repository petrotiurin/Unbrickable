using UnityEngine;
using System.Collections;

public class MenuOptionSelected : MonoBehaviour
{
	public GameObject option;
	
	Ray ray;
   	RaycastHit hit;
	
	void Update(){

		ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if(Physics.Raycast(ray, out hit)) {
            if(hit.transform.tag != this.transform.tag) {
                this.gameObject.SetActive(false);
				option.gameObject.SetActive(true);
            }
		}
		
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