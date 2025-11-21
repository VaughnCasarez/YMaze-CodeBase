using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int trials = 6;
    public List<string> environments = new List<string>() {"nature", "office", "stone", "ocean", "shop"};
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void GenerateExperiment()
    {
        foreach(string env in environments)
        {
            Block exp_block = Session.instance.CreateBlock(trials);
            for(int i = 0; i < trials; i++)
            {
                exp_block.GetRelativeTrial(i + 1).settings.SetValue("environment", env);
                if(i < trials - 1)
                    exp_block.GetRelativeTrial(i + 1).settings.SetValue("spawn", "start");
                else
                    exp_block.GetRelativeTrial(i + 1).settings.SetValue("spawn", "wrong");
            }
        }
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
