using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	   Debug.Log("GAME JUST ENDED!");
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

    public void debugging(){
        Debug.Log("GAME JUST ENDED!");
    }

    // Look in Game Interface Elements section of Unity Manual.
    // This displays the GameOver screen and
    //   displays buttons to 'view scores' & 'restart game.'
    void onGUI(){

    }
}
