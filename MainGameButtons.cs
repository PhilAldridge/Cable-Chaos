using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainGameButtons : MonoBehaviour
{
    private float startTime;
    private float timeElapsed;
    private int gameLength;
    private bool ranked;
    private int minutes;
    private int seconds;
    public TMP_Text Timer = null;
    // Start is called before the first frame update
    void Start()
    {
        ZPlayerPrefs.Initialize("what'sYourName", "salt12issalt");
        if (!ZPlayerPrefs.HasKey("startTime"))
        {
            ZPlayerPrefs.SetFloat("startTime", Time.time);
        }
        startTime = ZPlayerPrefs.GetFloat("startTime");

        if (ZPlayerPrefs.GetString("gameMode") == "Ranked")
        {
            ranked = true;
            gameLength = ZPlayerPrefs.GetInt("timeRemaining");
            
        }
        else
        {
            ranked = false;
            Timer.text = "";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ranked)
        {
            timeElapsed = Time.time - startTime;
            if (timeElapsed > gameLength)
            {
                SceneManager.LoadScene("RankedGameEnd");
            }
            minutes = (int)(Mathf.FloorToInt(timeElapsed / 60));
            seconds = (int)(Mathf.FloorToInt(timeElapsed % 60));
            Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void ResetLevel()
    {
        foreach(List<GameObject> path in Create3x3cube.HumanPaths)
        {
            while (path.Count > 1)
            {
                int pathIndex = Create3x3cube.HumanPaths.IndexOf(path);
                Destroy(Create3x3cube.Pipes[pathIndex][1]);
                Create3x3cube.Pipes[pathIndex].RemoveAt(1);
                path.RemoveAt(1);
            }
        }
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
