using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;

public class Leaderboard : MonoBehaviour {

    //Database connection
    private SqliteConnection dbcon;
    private string filename;
    private SqliteCommand cmd;

	// Use this for initialization
	void Start () {

        filename = "URI=file:" + Application.dataPath + "/scores.db"
        //if database does not exist already.
        dbcon = new SqliteConnection(filename);

        dbcon.Open();

        try{
            cmd = new SqliteCommand("CREATE TABLE highscores (name VARCHAR(20), score INT");
            SqliteDataReader reader = cmd.ExecuteReader();
        }catch{
            Debug.Log("Failed to create database.");
        }

        //for file system
        /*
	    //initialise top 10 high scores table.
        for(int i = 1; i <= 10; i++){
            PlayerPrefs.SetInt(i+"HScore", 0);
            PlayerPrefs.SetString(i+"HScore", "-");
        }
        */
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void AddScore(string name, int score) {
        //check leaderboard to see if score is higher than those.
        //Linear search is fine because it's just 10 scores -
        //but consider binary search for larger values. 

    }

    void DisplayScores(){

    }
}
