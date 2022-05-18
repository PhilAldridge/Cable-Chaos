using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            backButton();
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu"));
            //SceneManager.UnloadSceneAsync("MainGame");
        }
    }

    public void backButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
