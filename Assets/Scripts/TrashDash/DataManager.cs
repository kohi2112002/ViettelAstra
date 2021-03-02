using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
[System.Serializable]
public class UserData
{
    public string name;
    public int score;
}
[System.Serializable]
public class DataCollection
{
    public UserData[] datas;
    public DataCollection()
    {
        datas = new UserData[0];
    }
}
public class DataManager : MonoBehaviour
{
    private const int SCORE_LIMIT = 10;
    private static DataManager instance;
    public static DataManager Instance
    {
        get { return instance; }
    }
    public DataCollection dataCollection;
    private string dataPath;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            dataPath = Path.Combine(Application.persistentDataPath, "data.dat");
            LoadScore();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetString("Home", "Home");
    }
    public void LoadScore()
    {
        if (File.Exists(dataPath))
        {
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                try
                {
                    dataCollection = (DataCollection)binaryFormatter.Deserialize(fileStream);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    dataCollection = new DataCollection();
                }
            }
        }
        else
            dataCollection = new DataCollection();
    }
    public void SaveScore(string name, int score)
    {
        List<UserData> datas = new List<UserData>(dataCollection.datas);
        var user = new UserData();
        user.name = name;
        user.score = score;
        //sort the score and save the most ten score
        datas.Add(user);
        for (int i = datas.Count - 1; i > -1; i--)
        {
            for (int j = i - 1; j > -1; j--)
            {
                if (datas[i].score > datas[j].score)
                {
                    var tmp = datas[i];
                    datas[i] = datas[j];
                    datas[j] = tmp;
                }
            }
        }
        if (datas.Count > SCORE_LIMIT)
            datas.RemoveRange(SCORE_LIMIT, datas.Count - SCORE_LIMIT);
        dataCollection.datas = datas.ToArray();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, dataCollection);
        }
    }
    public bool IsHighScore(int score)
    {
        return (dataCollection.datas.Length < 10 || score > dataCollection.datas[dataCollection.datas.Length - 1].score) && score > 0;
    }
}
