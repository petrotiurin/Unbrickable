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

    private string uname = "Anon";

    // Use this for initialization
    void Start () {
        string name = Application.dataPath + "/lololololololololol.db";
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
            string sql = "CREATE TABLE highscores (name VARCHAR(20), score INT)"; //, level INT, rounds INT)";
            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
        }
        
        
        //Board brd = GameObject.Find("Scene").GetComponent<Board>();
        //AddScore("InsertNameHere", brd.score, brd.level, brd.rounds);
        //AddScore("BOO",58,1,2);
        //closeDb();
    }


    void Update(){
        //if(Input.GetKeyDown("return"))
    }


    // int AddScore(string name, int scr, int level, int rounds){
    //     string sql = "INSERT INTO highscores (name, score, level, rounds) VALUES ('"
    //         + name + "'," + scr + "," + level + "," + rounds + ")";
    //     //Debug.Log(sql);
    //     cmd = dbcon.CreateCommand();
    //     cmd.CommandText = sql;
    //     reader = cmd.ExecuteReader();

    //     return getPosition();
    // }

    int getPosition(string name, int scr){
        string sql = "SELECT COUNT(rowid) FROM highscores WHERE score > " + scr;

        Debug.Log("SQL = " + sql);

        cmd = dbcon.CreateCommand();
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();

        Debug.Log("READER.FIELDCOUNT = " + reader.FieldCount);

        int pos = 0;
        while(reader.Read()){
            pos = reader.GetInt32(0);// readArray[0];
            Debug.Log("pos-------------->" + (pos+1));
            return (pos+1);
        }
        return 0;//pos;
    }


    //for debugging & testing purposes.
    public int AddScore(string name, int scr) {
        string sql = "INSERT INTO highscores(name, score) VALUES ('" + name + "'," + scr + ")";
        cmd = dbcon.CreateCommand();
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();

        int pos = getPosition(name,scr);

        // Debug.Log("pos ======= " + pos);
        return pos;
    }

    // public ArrayList DisplayInLeaderboard(int pos, int scr){
    //     ArrayList readArray = new ArrayList();

    //     string sql = "SELECT rowid, * FROM highscores WHERE score >= " + scr + " ORDER BY score DESC";
    //     cmd = dbcon.CreateCommand();
    //     cmd.CommandText = sql;
    //     reader = cmd.ExecuteReader();

    //     while(reader.Read())
    //         for(int i = 0; i < reader.FieldCount; i++){
    //             readArray.Add(reader.GetValue(i));
    //             Debug.Log("urgh - " + reader.GetValue(i));
    //         }

    //     //Get the two rows with score higher than user's score
    //     sql = "SELECT * FROM highscores WHERE score < " + scr + " ORDER BY score DESC";
    //     cmd = dbcon.CreateCommand();
    //     cmd.CommandText = sql;
    //     reader = cmd.ExecuteReader();

    //     while(reader.Read())
    //         for(int i = 0; i < reader.FieldCount; i++){
    //             readArray.Add(reader.GetValue(i));
    //             Debug.Log("urgh - " + reader.GetValue(i));
    //         }

    //     return readArray;
    // }


    public ArrayList DisplayScores(){
        //Debug.Log("ARRAYLIST ----------> " + reader.FieldCount);

        string sql = "SELECT * FROM highscores ORDER BY score DESC";

        cmd = dbcon.CreateCommand();
        cmd.CommandText = sql;
        reader = cmd.ExecuteReader();

        // Debug.Log("ARRAYLIST ----------> " + reader.FieldCount);

        ArrayList readArray = new ArrayList();

         while(reader.Read())
            for (int i = 0; i < reader.FieldCount; i++)
                readArray.Add(reader.GetValue(i));

        return readArray;
    }

/*
    void closeDb(){
        reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;
        dbcon.Close();
        dbcon = null;
    }
    */


    void OnGUI(){
        
    }
}
