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
        string name = Application.dataPath + "/dbtest1.db";
        connectionString = "URI=file:" + name;
        bool flag;

        Debug.Log("Helloooo scoreboard");


        //check if the file/db exists already.
        flag = File.Exists(name);

        dbcon = (IDbConnection) new SqliteConnection(connectionString);
        dbcon.Open();
        cmd = dbcon.CreateCommand();

        Debug.Log("" + File.Exists(name));
        //We assume that the table was created when the file was created.
        //Therefore, if it exists, we assume the table does too.
        if(!flag){
            Debug.Log("Creating table");
            string sql = "CREATE TABLE highscores (name VARCHAR(20), score INT, level INT, rounds INT)";
            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
        }
        
        
        //Board brd = GameObject.Find("Scene").GetComponent<Board>();
        //AddScore("InsertNameHere", brd.score, brd.level, brd.rounds);
        AddScore("BOO",58,1,2);

        reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;
        dbcon.Close();
        dbcon = null;
	}


    void AddScore(string name, int scr, int level, int rounds){
        string sql = "INSERT INTO highscores (name, score, level, rounds) VALUES ('"
            + name + "'," + scr + "," + level + "," + rounds + ")";
        //Debug.Log(sql);
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();
    }


    //for debugging & testing purposes.
    void AddScore(string name, int scr) {
        string sql = "INSERT INTO highscores(name, score) VALUES (name, scr)";
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();
    }

    void DisplayScores(){
        string sql = "SELECT name, score FROM highscores";
    }
/*
    void OnGUI(){

    }
    */
}
