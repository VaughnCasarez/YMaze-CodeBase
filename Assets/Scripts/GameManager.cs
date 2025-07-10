using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Session.instance.trackedObjects.Add(player.GetComponent<PositionRotationTracker>());
    }
}
