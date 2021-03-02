using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MySceneManager : MonoBehaviour
{
    private static MySceneManager instance;
    public static MySceneManager Instance
    {
        get { return instance; }
    }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScene("Menu");
        }
        else
            Destroy(gameObject);
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(_LoadScene(sceneName));
    }
    private IEnumerator _LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        var asyncTask = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!asyncTask.isDone)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        asyncTask = SceneManager.UnloadSceneAsync(currentScene);
        while (!asyncTask.isDone)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        float startTime = Time.time;
        asyncTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (Time.time - startTime < 1 || !asyncTask.isDone)
            yield return null;
        FindObjectOfType<LoadSceneController>().FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.UnloadSceneAsync("Loading");
    }
}
