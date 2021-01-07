using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;
using UnityEngine.Networking;

public class SQLCon : MonoBehaviour
{
   
    public class Config
    {
        public string name;
        public string record;
        public string stage;
    }
    string DB_Name = "config.db";
    
    string Table_Name = "GameState";

    bool isDataExists = true;

    public List<Config> configs;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        configs = new List<Config>();
       
          yield return StartCoroutine(DBCrate());

        if (!isDataExists)
        {
            DatabaseInsert();
            isDataExists = true;
        }
        // DBConnectionCheck();
        DataBaseRead("Select * From "+ Table_Name);
        yield break;
    }

    IEnumerator DBCrate()
    {
        string filePath = string.Format("{0}/{1}", Application.persistentDataPath, DB_Name);
       
        if (Application.platform == RuntimePlatform.Android)
        {

            if (!File.Exists(filePath))
            {
                //  infoText.text += "jar:file://" + Application.dataPath + "!/assets/device_db.db";
                UnityWebRequest unityWebRequest = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/"+ DB_Name);
                unityWebRequest.downloadedBytes.ToString();
                yield return unityWebRequest.SendWebRequest().isDone;
                File.WriteAllBytes(filePath, unityWebRequest.downloadHandler.data);
                // Row 추가 해야함
                isDataExists = false;
               // Debug.Log("DB생성 완료(파일 없을때)");
            }
             
           // Debug.Log("DB생성 완료(파일 있을때)");
           

        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path =  Application.dataPath + "/Raw/" + DB_Name;

            if (!File.Exists(filePath))
            {
                //UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);
                //unityWebRequest.downloadedBytes.ToString();
                //yield return unityWebRequest.SendWebRequest().isDone;
                //File.WriteAllBytes(filePath, unityWebRequest.downloadHandler.data);

                File.Copy(path, filePath);
                isDataExists = false;
            //    Debug.Log("|파일복사|");
                // Row 추가 해야함

            }
            else
            {
                // "|파일은 이미 생성|";
            }

  
        }
        else
        {

            filePath = Application.dataPath + "/" + DB_Name;
            if (!File.Exists(filePath))
            {
                File.Copy(Application.streamingAssetsPath + "/" + DB_Name, filePath);
                // Row 추가 해야함
                isDataExists = false;

                Debug.Log("DB생성 완료(파일 없을때)");
            }
            else
            {
                Debug.Log("DB생성 완료(파일 있을때)");
            }
        }

        yield break;
    }

    
    public string GetDBfilePath()
    {
        string str = string.Empty;

        if (Application.platform == RuntimePlatform.Android)
        {
            str = "URI=file:" + Application.persistentDataPath + "/"+ DB_Name;
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer )
        {
            str = "URI=file:" + Application.persistentDataPath + "/" + DB_Name;
        }
        else
        {
            str = "URI=file:" + Application.dataPath + "/" + DB_Name;
        }

        return str;
    }

    public void DBConnectionCheck()
    {
        try
        {

            IDbConnection dbConnection = new SqliteConnection(GetDBfilePath());
            dbConnection.Open();

            if (dbConnection.State == ConnectionState.Open)
            {
                //infoText2.text = "DB 연결 성공";
            }
            else
            {
                //infoText2.text = "연결실패(에러)";
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void DatabaseInsert()
    {
        string query = "Insert Into " + Table_Name+ "(\"name\" ,\"best_record\", \"stage\") VALUES(\"Player1\", \"0\", \"0\" )";
        IDbConnection dbConnection = new SqliteConnection(GetDBfilePath());
        dbConnection.Open();

        if (dbConnection.State == ConnectionState.Open)
        {
            // infoText2.text = "DB연결성공";
        }
        else
        {
            return;
        }

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = query;
        dbCommand.ExecuteReader();

     
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

    }
    public void DataBaseUpdate(int point)
    {
        string query = "Update " + Table_Name + " Set best_record=\"" + point.ToString() + "\" Where name=\"Player1\" ";
        IDbConnection dbConnection = new SqliteConnection(GetDBfilePath());
        dbConnection.Open();

        if (dbConnection.State == ConnectionState.Open)
        {
            // infoText2.text = "DB연결성공";
        }
        else
        {
            return;
        }

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = query;
        dbCommand.ExecuteReader();


        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    public void DataBaseRead(string query)
    {
        try
        {
            IDbConnection dbConnection = new SqliteConnection(GetDBfilePath());

            dbConnection.Open();

            if (dbConnection.State == ConnectionState.Open)
            {
               // infoText2.text = "DB연결성공";
            }
            else
            {
               // infoText2.text = "연결실패(에러)";
            }


            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = query;
            IDataReader dataReader = dbCommand.ExecuteReader();

            string viewLog = string.Empty;
            while (dataReader.Read())
            {
                Config config = new Config();

                config.name   = dataReader.GetString(0);
                config.record = dataReader.GetString(1);
                config.stage  = dataReader.GetString(2);
                Debug.Log(dataReader.GetString(0) + ", " + dataReader.GetString(1) + ", " + dataReader.GetString(2));

                configs.Add(config);
            }

           
            dataReader.Dispose();
            dataReader = null;
            dbCommand.Dispose();
            dbCommand = null;
            dbConnection.Close();
            dbConnection = null;
        }
        catch (Exception e)
        {
            Debug.Log("오류: " + e.ToString());
        }
    }



}
