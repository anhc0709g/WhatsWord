using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

public class DbHelper : MonoBehaviour {
    private static DbHelper sInstance;

    private const string SQL_DB_NAME = "Pics1Word.db";
    private  string DATABASE_PATH;
    private static string _sqlDBLocation = "";
    /// <summary>
    /// DB objects
    /// </summary>
    private IDbConnection _connection = null;
    private IDbCommand _command = null;
    private IDataReader _reader = null;
    private string _sqlString;
    public bool DebugMode = true;

    private DbHelper()
    {
        SQLiteInit();
    }

    private void CopyFileIfNonexistent()
    {

        DATABASE_PATH = Application.persistentDataPath + "/" + SQL_DB_NAME;
        string assetPath = Application.streamingAssetsPath + "/" + SQL_DB_NAME;

        if (!File.Exists(DATABASE_PATH))
        {
     
            WWW www1 = new WWW(assetPath);
            while (!www1.isDone) { }
            File.WriteAllBytes(DATABASE_PATH, www1.bytes);
            Debug.Log("file copy done");
            www1.Dispose();
            www1 = null;

        } 
    }
    /// <summary>
    /// Basic initialization of SQLite
    /// </summary>
    /// 
    private void SQLiteInit()
    {
        CopyFileIfNonexistent();
        _sqlDBLocation = "URI=file:" + DATABASE_PATH;

      //  Debug.Log("SQLiter - Opening SQLite Connection at " + _sqlDBLocation);
        _connection = new SqliteConnection(_sqlDBLocation);
        _command = _connection.CreateCommand();
        _connection.Open();

        // WAL = write ahead logging, very huge speed increase
        _command.CommandText = "PRAGMA journal_mode = WAL;";
        _command.ExecuteNonQuery();

        // journal mode = look it up on google, I don't remember
        _command.CommandText = "PRAGMA journal_mode";
        _reader = _command.ExecuteReader();
        if (DebugMode && _reader.Read())
         //   Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
        _reader.Close();

        // more speed increases
        _command.CommandText = "PRAGMA synchronous = OFF";
        _command.ExecuteNonQuery();

        // and some more
        _command.CommandText = "PRAGMA synchronous";
        _reader = _command.ExecuteReader();
        if (DebugMode && _reader.Read())
      //      Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
        _reader.Close();

       
        _connection.Close();
    }
    /// <summary>
    /// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
    /// </summary>
    public void GetTest()
    {
        StringBuilder sb = new StringBuilder();
        _connection.Open();

        // if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
        _command.CommandText = "SELECT * FROM DailyEvent";
        _reader = _command.ExecuteReader();
        //Debug.Log(_reader);
        while (_reader.Read())
        {
            // reuse same stringbuilder
            sb.Length = 0;
            sb.Append(_reader.GetString(0)).Append(" -");
            sb.Append(_reader.GetString(1)).Append(" ");
            sb.AppendLine();

            // view our output
            //if (DebugMode)
                Debug.Log(sb.ToString());
        }
        _reader.Close();
        _connection.Close();
    }

    public Puzzle getRandomPuzzle()
    {
        Puzzle puzzle = null;
        _connection.Open();
        // if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
        _command.CommandText = "select id, lower(solution) from Puzzle where id < 4000 order by   RANDOM() limit 4";
        _reader = _command.ExecuteReader();
        //Debug.Log(_reader);
        System.Random rnd = new System.Random();
        int selectPuzzle = rnd.Next(0,3);
        int i = 0;
        puzzle = new Puzzle();
        puzzle.randomSolution = new string[4];
        while (_reader.Read())
        {

            if (i==selectPuzzle)
            {
                puzzle.id = _reader.GetInt16(0);
                puzzle.solution = _reader.GetString(1);
            }
            puzzle.randomSolution[i] = _reader.GetString(1);
            i++;
        }
        _reader.Close();
        _connection.Close();
        return puzzle;
    }
    public Puzzle getNextPuzzle(int oldId)
    {
        Puzzle puzzle = null;
        _connection.Open();
        // if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
        _command.CommandText = "select id,solution from Puzzle where id > " + oldId + " and id < 4000 order by id asc limit 4";
        _reader = _command.ExecuteReader();
        //Debug.Log(_reader);
        bool first = true;
        int i = 0;
        puzzle = new Puzzle();
        puzzle.randomSolution = new string[4];
        while (_reader.Read())
        {

            if (first)
            {
                puzzle.id = _reader.GetInt16(0);
                puzzle.solution = _reader.GetString(1);
                first = false;
            }
            puzzle.randomSolution[i] = _reader.GetString(1);
            i++;
        }
        _reader.Close();
        _connection.Close();
        return puzzle;
    }
    public string[] getRandomPuzzle(String solution)
    {

        ArrayList arrayList = new ArrayList();
        _connection.Open();
        // if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
        _command.CommandText = "select  solution from Puzzle where id < 4000 and solution !='"+ solution + "' and length(solution) = length('" + solution + "')  order by RANDOM() limit 3";
        _reader = _command.ExecuteReader();
        System.Random ran = new System.Random();
        int ranIndex = ran.Next(0, 3);
        while (_reader.Read())
        {
            arrayList.Add(_reader.GetString(0));
        }
        arrayList.Insert(ranIndex, solution);
        _reader.Close();
        _connection.Close();
        return (string[])arrayList.ToArray(typeof(string));
    }

    public static DbHelper getInstance()
    {
        if(sInstance == null)
        {
            sInstance = new DbHelper();
        }
        return sInstance;
    }

    // Use this for initialization
    void Start () {
   
        SQLiteInit();
        GetTest();


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
