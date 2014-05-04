using UnityEngine;
using System.Collections;
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class Leaderboard : MonoBehaviour {

    private IDbConnection dbcon;
    private IDbCommand cmd;
    private IDataReader reader;
    private string connectionString;

	// Use this for initialization
	void Start () {
        string name = Application.dataPath + "/lololol4.db";
        connectionString = "URI=file:" + name;
        bool flag;

        Debug.Log("Helloooo scoreboard");


        //check if the file/db exists already.
        flag = File.Exists(name);

        dbcon = (IDbConnection) new SqliteConnection(connectionString);
        dbcon.Open();
        cmd = dbcon.CreateCommand();

        Debug.Log("" + File.Exists(name));
        if(!flag){
            Debug.Log("Creating table");
            cmd.CommandText = "CREATE TABLE highscores (name VARCHAR(20), score INT)";
            reader = cmd.ExecuteReader();
        }
        
        
        //Board brd = GameObject.Find("Scene").GetComponent<Board>();
        AddScore("JohnSmith", 5);//brd.score);

        // clean up
        /*reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;*/
        dbcon.Close();
        dbcon = null;
	}

    void AddScore(string name, int scr) {
        string sql = "INSERT INTO highscores(name, score) VALUES (name, scr)";
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();
    }

    void DisplayScores(){

    }
/*
    void OnGUI(){

    }
    */
}
