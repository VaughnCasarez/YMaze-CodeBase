using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UXF;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit;

public class YMaze_transition : MonoBehaviour {
    public List<GameObject> environments = new List<GameObject>();

    //get the FPS controller
    public ContinuousMoveProvider controller;
    public GameObject cam;

    //UI
    public Image blackscreen;
    public Text text;

    //sound file
    public AudioSource correct;
    public AudioSource wrong;

    //starting locations and orientations
    private Vector3 start_loc1 = new Vector3(36.52f, -3.42f, 22f);
    private Vector3 start_loc2 = new Vector3(0f, -3.42f, -34f);
    private Vector3 start_loc3 = new Vector3(-35.82f, -3.42f, 22f);
    public Transform centerpoint;

    private float waitseconds = 3.0f;
    private float wait_transitions = 5.0f;

    private int repeated_trials = 0;
    void Start () {
        Session.instance.BeginNextTrial();
        Session.instance.onSessionEnd.AddListener(EndSession);
        transform.position = start_loc1;
        FaceCenterpoint();
        //controller.Mousereset();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if ((col.gameObject.name == "Correct" || col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
            && Session.instance.CurrentTrial.numberInBlock == Session.instance.CurrentBlock.lastTrial.numberInBlock)
        {
            Debug.Log("Change Environment!");
            DisableEnvironment();
            Session.instance.EndCurrentTrial();
            repeated_trials = 0;
            cam.GetComponent<PositionRotationTracker>().objectName = "participant";
            GetComponent<EyeTracking>().objectName = "participant";
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
                cam.GetComponent<PositionRotationTracker>().objectName = $"{cam.GetComponent<PositionRotationTracker>().objectName}_{repeated_trials}";
                GetComponent<EyeTracking>().objectName = $"{GetComponent<EyeTracking>().objectName}_{repeated_trials}";
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
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "office")
            {
                transform.position = start_loc1;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "stone")
            {
                transform.position = start_loc2;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "ocean")
            {
                transform.position = start_loc2;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else
            {
                transform.position = start_loc1;
                FaceCenterpoint();
                //controller.Mousereset();
            }
        }

        else // when subs completed 5 trials in a row correctly, teleport them to wrong location
        {
            if (environment == "nature")
            {
                transform.position = start_loc2;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "office")
            {
                transform.position = start_loc2;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "stone")
            {
                transform.position = start_loc1;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else if (environment == "ocean")
            {
                transform.position = start_loc1;
                FaceCenterpoint();
                //controller.Mousereset();
            }
            else
            {
                transform.position = start_loc2;
                FaceCenterpoint();
                //controller.Mousereset();
            }
        }
    }

    void FaceCenterpoint()
    {
        if (centerpoint == null) return;

        // Flat direction from player to centerpoint
        Vector3 targetDir = centerpoint.position - transform.position;
        targetDir.y = 0;
        if (targetDir == Vector3.zero) return;

        // Current camera (HMD) forward on the horizontal plane
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        if (camForward == Vector3.zero) return;

        // Rotate the rig by the delta so the HMD ends up facing the target
        float angleDelta = Vector3.SignedAngle(camForward, targetDir, Vector3.up);
        transform.Rotate(Vector3.up, angleDelta, Space.World);
    }

    IEnumerator WaitSeconds_Teleport(float seconds)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(seconds);
        Session.instance.BeginNextTrial();
        Teleport(Session.instance.CurrentTrial.settings.GetString("spawn"));
        controller.enabled = true;
    }
    IEnumerator WaitSeconds_ChangeEnvir(float seconds)
    {
        controller.enabled = false;
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
        controller.enabled = true;
    }
    public void EndSession(Session session)
    {
        blackscreen.enabled = true;
        text.text = "The end of the experiment! \nThanks for your participation! \nPlease press the 'Esc' key to quit.";
        text.enabled = true;
    }
}
