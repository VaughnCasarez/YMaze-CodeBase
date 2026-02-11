using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // The number of trials per block/environment.
    public int trials = 6;
    // The list of environments, each representing a block in the experiment
    public List<string> environments = new List<string>() {"nature", "office", "stone", "ocean", "shop"};
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // This function is called from the UXF_Rig and generates the experiment.
    // Since this version was made for WebGL, we cannot load our settings from a JSON or .csv file,
    // so instead we manually code the blocks and trials.
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
        Session.instance.settings.SetValue("block_trial_number", 1);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    // This function is called when a new scene is loaded and adds the player to the list of tracked objects
    // This is required for the position and rotation of the player to be tracked between scenes since
    // the player is not a child of the rig.
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Session.instance.trackedObjects.Add(player.GetComponent<PositionRotationTracker>());
    }
}
