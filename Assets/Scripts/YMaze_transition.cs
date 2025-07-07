using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class YMaze_transition : MonoBehaviour {
    //get the environments;
    public GameObject nature;
    public GameObject office;
    public GameObject stone;
    public GameObject ocean;
    public GameObject shop;

    private GameObject[] all_envirs = new GameObject[5];
    private List<GameObject> envirs = new List<GameObject>();

    //get the FPS controller
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;

    //UI
    public Image blackscreen;
    public Text text;

    //sound file
    public AudioSource correct;
    public AudioSource wrong;

    //Demographic information

    private string age;
    private string id;
    private string gender;
    private string name;
    private int num_of_envirs;

    //write file
    TextWriter data;
    TextWriter coordinates;

    //get Camera
    private Camera m_Camera;

    private int correct_trials_counter = 0;
    private int envir_num = 0;
    private bool trial_switch = true;
    private int consecutive_trial_num = 5;

    //for recording coordinates
    private float x_coor;
    private float z_coor;
    private float record_time;
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
        record_time = Time.realtimeSinceStartup;
        m_Camera = Camera.main;

        GameManager info = GameObject.Find("GameManager").GetComponent<GameManager>();
        id = info.id;
        age = info.age;
        gender = info.gender;
        name = info.name;
        num_of_envirs = int.Parse(info.trials);
        Debug.Log(num_of_envirs);

        all_envirs[0] = nature;
        all_envirs[1] = office;
        all_envirs[2] = stone;
        all_envirs[3] = ocean;
        all_envirs[4] = shop;

        for (int i = 0; i < num_of_envirs; i++)
        {
            envirs.Add(all_envirs[i]);
        }
        transform.position = start_loc1;
        transform.eulerAngles = Ori1;
        controller.Mousereset();

        //write the files
        data = new StreamWriter("YMazePreference_" + id + "_" + name + ".tsv", true);
        data.WriteLine("ID" + "\t" + "Gender" + "\t" + "Age" + "\t" + "MazeName" + "\t" + "Preference");
        data.Flush();
        coordinates = new StreamWriter("coordinates_" + id + "_" + name + ".tsv", true);
        coordinates.WriteLine("Environment" + "\t" + "Cummulative_Time" + "\t" + "X" + "\t" + "Z" + "\t" + "rotation");
        coordinates.Flush();
    }
	
	// Update is called once per frame
	void Update () {
		if (envir_num == num_of_envirs)
        {
            Debug.Log("Experiment finished!");
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1.0f;
        }

        if (Time.realtimeSinceStartup - record_time > record_time_interval)
        {
            SaveCoordinates();
            record_time = Time.realtimeSinceStartup;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (trial_switch == true)
        {
            if (correct_trials_counter < consecutive_trial_num) // before subs do the probe trial
            {
                if (col.gameObject.name == "Correct")
                {
                    trial_switch = false;
                    Debug.Log("Correct!");
                    correct.Play();
                    correct_trials_counter++;
                    if (correct_trials_counter == consecutive_trial_num)
                    {
                        StartCoroutine(WaitSeconds_TeleWrong(waitseconds));
                    }
                    else
                    {
                        StartCoroutine(WaitSeconds_TeleStart(waitseconds));
                    }
                }
                else if (col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
                {
                    trial_switch = false;
                    Debug.Log("Wrong!");
                    wrong.Play();
                    correct_trials_counter = 0;
                    StartCoroutine(WaitSeconds_TeleStart(waitseconds));
                }
            }
            else //after subs do the probe trial
            {
                if (col.gameObject.name == "Correct" || col.gameObject.name == "Wrong" || col.gameObject.name == "Start")
                {
                    trial_switch = false;
                    Debug.Log("Change Environment!");
                    //save response
                    if (col.gameObject.name == "Correct")
                    {
                        SaveResponse("Place");
                    }
                    else
                    {
                        SaveResponse("Response");
                    }

                    correct_trials_counter = 0;
                    envir_num++;
                    envirs[envir_num - 1].SetActive(false);
                    envirs[envir_num].SetActive(true);
                    StartCoroutine(WaitSeconds_ChangeEnvir(wait_transitions));
                }
            }
        }

    }

    void Teleport(string loc)
    {
        trial_switch = true;
        if (loc == "Start") // teleport subs to starting location
        {
            if (envirs[envir_num] == nature)
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == office)
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == stone)
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == ocean)
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
            if (envirs[envir_num] == nature)
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == office)
            {
                transform.position = start_loc2;
                transform.eulerAngles = Ori2;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == stone)
            {
                transform.position = start_loc1;
                transform.eulerAngles = Ori1;
                controller.Mousereset();
            }
            else if (envirs[envir_num] == ocean)
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

    IEnumerator WaitSeconds_TeleStart(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        yield return new WaitForSeconds(seconds);
        Teleport("Start");
        controller.m_WalkSpeed = 10.0f;
    }

    IEnumerator WaitSeconds_TeleWrong(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        yield return new WaitForSeconds(seconds);
        Teleport("Wrong");
        controller.m_WalkSpeed = 10.0f;
    }

    IEnumerator WaitSeconds_ChangeEnvir(float seconds)
    {
        controller.m_WalkSpeed = 0.0f;
        blackscreen.enabled = true;
        text.enabled = true;
        yield return new WaitForSeconds(seconds);
        Teleport("Start");
        blackscreen.enabled = false;
        text.enabled = false;
        controller.m_WalkSpeed = 10.0f;
    }

    /* save the performance trial by trial*/
    void SaveResponse(string preference)
    {
        string outResponse = id + "\t" + gender + "\t" + age + "\t" + envirs[envir_num].name.ToString() + "\t" + preference;
        data.WriteLine(outResponse);
        data.Flush();
    }

    void SaveCoordinates()
    {
        string outResponse = envirs[envir_num].name.ToString() + "\t" + Time.realtimeSinceStartup.ToString() + "\t" + 
            transform.position.x.ToString() + "\t" + transform.position.z.ToString() + "\t" + transform.eulerAngles.y.ToString();
        coordinates.WriteLine(outResponse);
        coordinates.Flush();
    }

}
