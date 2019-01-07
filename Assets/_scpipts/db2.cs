using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

public class db2 : MonoBehaviour {
    private const string SQL_DB_NAME = "Pics1Word.db";

    private static string _sqlDBLocation = "";
    /// <summary>
    /// DB objects
    /// </summary>
    private IDbConnection _connection = null;
    private IDbCommand _command = null;
    private IDataReader _reader = null;
    private string _sqlString;
    public bool DebugMode = true;


    public static string StreamingAssetURLForPath(string path)
    {
#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_EDITOR_WIN)
        return Path.Combine("file://" + Application.streamingAssetsPath, path);
#elif UNITY_ANDROID
        return Path.Combine("file:///android_asset/", path);
        //return Application.streamingAssetsPath+"/"+path;
#else
        UniWebViewLogger.Instance.Critical("The current build target is not supported.");
        return string.Empty;
#endif
    }


    private void CopyFileIfNonexistent()
    {
        GameObject txtTex2 = GameObject.Find("txtTex2");
        Text txtText2 = txtTex2.GetComponent<Text>();
        string dataPath = Application.persistentDataPath + "/" + SQL_DB_NAME;
        string assetPath = Application.streamingAssetsPath + "/" + SQL_DB_NAME;

        if (!File.Exists(dataPath))
        {
     
            WWW www1 = new WWW(assetPath);
            while (!www1.isDone) { }
            Debug.Log("yield done");
            File.WriteAllBytes(dataPath, www1.bytes);
            Debug.Log("file copy done");
            www1.Dispose();
            www1 = null;
            txtText2.text = dataPath + " not existed, and copy done! assetPath="+ assetPath;
        }
        else
        {
            txtText2.text = dataPath+ " existed";
        }
    }
    /// <summary>
    /// Basic initialization of SQLite
    /// </summary>
    /// 
    private void SQLiteInit()
    {
        CopyFileIfNonexistent();
        GameObject txtTex2 = GameObject.Find("txtTex2");
        Text txtText2 = txtTex2.GetComponent<Text>();

      //  _sqlDBLocation = "URI="+ StreamingAssetURLForPath("4Pics1Word.db");
        //  _sqlDBLocation = "URI=" + Path.Combine("file://" + Application.streamingAssetsPath, "4Pics1Word.db");
        //_sqlDBLocation = "URI=file:" + Application.dataPath + "/StreamingAssets/4Pics1Word.db";

        _sqlDBLocation = "URI=file:" +Application.persistentDataPath + "/" + SQL_DB_NAME;
        //  txtText2.text = _sqlDBLocation;

        Debug.Log("SQLiter - Opening SQLite Connection at " + _sqlDBLocation);
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
            Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
        _reader.Close();

        // more speed increases
        _command.CommandText = "PRAGMA synchronous = OFF";
        _command.ExecuteNonQuery();

        // and some more
        _command.CommandText = "PRAGMA synchronous";
        _reader = _command.ExecuteReader();
        if (DebugMode && _reader.Read())
            Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
        _reader.Close();

       
        _connection.Close();
    }
    /// <summary>
    /// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
    /// </summary>
    public void GetAllWords()
    {
        GameObject text = GameObject.Find("txtText");
        Text txtText = text.GetComponent<Text>();
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
        txtText.text = sb.ToString();
        _reader.Close();
        _connection.Close();
    }


    // Use this for initialization
    void Start () {
   
        SQLiteInit();
        GetAllWords();


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
