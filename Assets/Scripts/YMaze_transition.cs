 using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UXF;

public class YMaze_transition : MonoBehaviour {
    public List<GameObject> environments = new List<GameObject>();

    //get the FPS controller
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

    //UI
    public Image blackscreen;
    public Text text;

    //sound file
    public AudioSource correct;
    public AudioSource wrong;

    //starting locations and orientations
    private Vector3 start_loc1 = new Vector3(36.52f, 0.0f, 22f);
    private Vector3 start_loc2 = new Vector3(0f, 0.0f, -34f);
    private Vector3 start_loc3 = new Vector3(-35.82f, 0.0f, 22f);
    private Vector3 Ori1 = new Vector3(0.0f, -120.0f, 0.0f);
    private Vector3 Ori2 = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Ori3 = new Vector3(0.0f, 120.0f, 0.0f);

    private float waitseconds = 3.0f;
    private float wait_transitions = 5.0f;

    // Keeps track of the amount of times a trial was repeated per block
    private int repeated_trials = 0;
    void Start () {
        //This starts the first trial of the experiment when the scene is first loaded
        Session.instance.BeginNextTrial();
        Session.instance.onSessionEnd.AddListener(EndSession);

        transform.position = start_loc1;
        transform.eulerAngles = Ori1;
        controller.Mousereset();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // When a player enters one of the zones, it can either be correct, wrong, or start
    // If the player enters the correct zone, the trial ends and the correct sound is played, otherwises
    // the trial ends and the wrong sound is played. Additionally, the player is teleported to the start location and
    // must redo the trial.
    void OnTriggerEnter(Collider col)
    {
        if ((col.gameObject.name == "Correct" || col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
            && Session.instance.CurrentTrial.numberInBlock == Session.instance.CurrentBlock.lastTrial.numberInBlock)
        {
            Debug.Log("Change Environment!");
            DisableEnvironment();
            Session.instance.EndCurrentTrial();
            repeated_trials = 0;
            GetComponent<PositionRotationTracker>().objectName = "participant";
            StartCoroutine(WaitSeconds_ChangeEnvir(wait_transitions));
        }
        else
        { 
            if (col.gameObject.name == "Correct")
            {
                Session.instance.EndCurrentTrial();
                Debug.Log("Correct!");
                correct.Play();
                StartCoroutine(WaitSeconds_Teleport(waitseconds));

            }
            else if (col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
            {
                Session.instance.EndCurrentTrial();
                Session.instance.currentTrialNum = Session.instance.settings.GetInt("block_trial_number") - 1;
                repeated_trials++;
                GetComponent<PositionRotationTracker>().objectName = $"{GetComponent<PositionRotationTracker>().objectName}_{repeated_trials}";

                Debug.Log("Wrong!");
                wrong.Play();
                StartCoroutine(WaitSeconds_Teleport(waitseconds));
            }   
        }
    }

    public void DisableEnvironment()
    {
        environments[Session.instance.currentBlockNum-1].SetActive(false);
    }
    public void LoadNewEnvironment()
    {
        environments[Session.instance.currentBlockNum-1].SetActive(true);
    }

    void Teleport(string loc)
    {
        string environment = Session.instance.CurrentTrial.settings.GetString("environment");
        if (loc == "start") // teleport subs to starting location
        {
            if (environment == "nature")
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (environment == "office")
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (environment == "stone")
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (environment == "ocean")
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
        }

        else // when subs completed 5 trials in a row correctly, teleport them to wrong location
        {
            if (environment == "nature")
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (environment == "office")
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (environment == "stone")
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (environment == "ocean")
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
        }
    }

    IEnumerator WaitSeconds_Teleport(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        yield return new WaitForSeconds(seconds);
        Session.instance.BeginNextTrial();
        Teleport(Session.instance.CurrentTrial.settings.GetString("spawn"));
        controller.m_WalkSpeed = 10.0f;
    }
    IEnumerator WaitSeconds_ChangeEnvir(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        blackscreen.enabled = true;
        text.enabled = true;
        yield return new WaitForSeconds(seconds - 2f);
        Session.instance.BeginNextTrial();
        Session.instance.settings.SetValue("block_trial_number", Session.instance.currentTrialNum);
        Teleport(Session.instance.CurrentTrial.settings.GetString("spawn"));
        yield return new WaitForSeconds(2f);
        LoadNewEnvironment();
        blackscreen.enabled = false;
        text.enabled = false;
        controller.m_WalkSpeed = 10.0f;
    }
    public void EndSession(Session session)
    {
        blackscreen.enabled = true;
        text.text = "The end of the experiment! \nThanks for your participation! \nPlease press the 'Esc' key to quit.";
        text.enabled = true;
    }
}
