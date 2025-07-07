using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class GameManager : MonoBehaviour
{

    public void GenerateExperiment(Session session)
    {
        int trialCount = session.settings.GetStringList("trials").Count;
        Block mainBlock = session.CreateBlock(trialCount);
        // session.NextTrial.Begin();
    }
    
}
