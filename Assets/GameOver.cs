using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

    private Leaderboard lboard;
    private string uname = "Voldemort";
    private bool nameEntered = false;

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

    public void endGame(int scr){
        int score = scr;
    }

    // This displays the GameOver screen and
    //   displays buttons to 'view scores' & 'restart game.'
    // 'View Scores' loads Leaderboard scene, and restart brings up MainMenu
    /*void OnGUI(){
        Debug.Log("GUIGUIGUI");

        if(!nameEntered)
            uname = GUI.TextField (new Rect (50,50,250,50), uname);

        if(GUI.Button(new Rect(100, 75, 150, 50), "Click to enter"))
            nameEntered = true;


        if(GUI.Button(new Rect(Screen.width/2-150,Screen.height/2-100,300,200), "Click to restart game"))
            Application.LoadLevel("MainMenu");
    }*/
}