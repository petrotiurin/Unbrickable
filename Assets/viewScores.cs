using UnityEngine;
using System.Collections;

public class viewScores : MonoBehaviour
{
    private Leaderboard lboard;
	private bool receivedLboard;
	ArrayList dispScores;
	private GUIStyle lboard_header;
	private GUIStyle lboard_entry;
	
	// Use this for initialization
	void Start ()
	{
		dispScores = new ArrayList();
        lboard = GetComponent<Leaderboard>();
		
		lboard_entry = new GUIStyle();
		lboard_entry.fontSize = 60;
		lboard_entry.fontStyle = FontStyle.Bold;
		lboard_entry.normal.textColor = Color.white;

	}

	
	void OnGUI(){
		//Leaderboard graphics
        if(!receivedLboard){
            dispScores = lboard.DisplayScores(5);
            receivedLboard = true; //!receivedLboard;
		}
		
		int n = (dispScores.Count < 5) ? dispScores.Count : 5;
		
        for(int i = 0; i < n; i++){
            // Debug.Log("TEST -------- " + dispScores[i]);
			GUI.Label(new Rect(Screen.width/2 - 450, 275 + i*75, 400, 25), (i+1) + "." , lboard_entry);
            GUI.Label(new Rect(Screen.width/2 - 350, 275 + i*75, 400, 25), "" + dispScores[i*2], lboard_entry);
            GUI.Label(new Rect(Screen.width/2 + 200, 275 + i*75, 400, 25), "" + dispScores[i*2+1], lboard_entry);
        }
	}
}

