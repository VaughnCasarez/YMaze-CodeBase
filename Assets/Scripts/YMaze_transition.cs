using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
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

    //Demographic information


    private int envir_num = 0;

    private float record_time_interval = 1.0f;//how often the distance and position is recorded.

    //starting locations and orientations
    private Vector3 start_loc1 = new Vector3(36.52f, 0.0f, 22f);
    private Vector3 start_loc2 = new Vector3(0f, 0.0f, -34f);
    private Vector3 start_loc3 = new Vector3(-35.82f, 0.0f, 22f);
    private Vector3 Ori1 = new Vector3(0.0f, -120.0f, 0.0f);
    private Vector3 Ori2 = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Ori3 = new Vector3(0.0f, 120.0f, 0.0f);

    private float waitseconds = 3.0f;
    private float wait_transitions = 5.0f;
    // Use this for initialization
    void Start () {
        Session.instance.BeginNextTrial();
        envir_num = Session.instance.settings.GetInt("environment_number");


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

    void OnTriggerEnter(Collider col)
    {
        if ((col.gameObject.name == "Correct" || col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
            && Session.instance.CurrentTrial.numberInBlock == Session.instance.CurrentBlock.lastTrial.numberInBlock)
        {
            Debug.Log("Change Environment!");
            DisableEnvironment();
            Session.instance.EndCurrentTrial();
            Session.instance.BeginNextTrial();
            Session.instance.settings.SetValue("block_trial_number", Session.instance.currentTrialNum);

            StartCoroutine(WaitSeconds_ChangeEnvir(wait_transitions));
        }
        else
        { 
            if (col.gameObject.name == "Correct")
            {
                Session.instance.EndCurrentTrial();

                Debug.Log("Correct!");
                correct.Play();

                Session.instance.BeginNextTrial();
                StartCoroutine(WaitSeconds_Teleport(waitseconds));

            }
            else if (col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
            {
                Session.instance.EndCurrentTrial();
                Session.instance.currentTrialNum = Session.instance.settings.GetInt("block_trial_number") - 1;
                Session.instance.BeginNextTrial();

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
        Teleport(Session.instance.CurrentTrial.settings.GetString("spawn"));
        controller.m_WalkSpeed = 10.0f;
    }
    IEnumerator WaitSeconds_ChangeEnvir(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        blackscreen.enabled = true;
        text.enabled = true;
        yield return new WaitForSeconds(seconds - 2f);
        Debug.Log("New ENV: " + Session.instance.CurrentTrial.settings.GetString("environment"));
        Teleport(Session.instance.CurrentTrial.settings.GetString("spawn"));
        yield return new WaitForSeconds(2f);
        LoadNewEnvironment();
        blackscreen.enabled = false;
        text.enabled = false;
        controller.m_WalkSpeed = 10.0f;
    }
}
