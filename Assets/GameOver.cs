using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

    private Leaderboard lboard;
	// Use this for initialization
	void Start () {
        //Ask for name
        //Insert score into database when game ends.
        lboard = GetComponent<Leaderboard>();
        Debug.Log("In GameOver scene");
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

    // This displays the GameOver screen and
    //   displays buttons to 'view scores' & 'restart game.'
    // 'View Scores' loads Leaderboard scene, and restart brings up MainMenu
    void OnGUI(){
        Debug.Log("GUIGUIGUI");

        if(GUI.Button(new Rect(Screen.width/2-150,Screen.height/2-100,300,200), "Click to restart game"))
            Application.LoadLevel("MainMenu");
    }
}
