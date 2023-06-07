/*
 * Name: Liam Kikin-Gil
 * Date: October 4, 2022
 * Desc: Add this to use the function in events in other components such as the menu buttons
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string sceneName = "First Level";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
