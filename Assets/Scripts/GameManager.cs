using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // public void GenerateExperiment(Session session)
    // {
    //     int trialCount = session.settings.GetInt("trials");
    //     Block mainBlock = session.CreateBlock(trialCount);
    //     SceneManager.LoadScene(1);
    // }
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
