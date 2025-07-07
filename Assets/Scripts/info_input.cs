using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;

public class info_input : MonoBehaviour {
    public InputField input_trials;
    public InputField input_name;
    public InputField input_age;
    public InputField input_gender;
    public InputField input_id;
    public Button submit;
    public GameObject info_object;
    public GameManager info;

    private static string trials;
    private static string id;
    private static string age;
    private static string gender;
    private static string name;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(info_object);
        input_trials.onEndEdit.AddListener(SubmitTrials);
        input_age.onEndEdit.AddListener(SubmitAge);
        input_id.onEndEdit.AddListener(SubmitID);
        input_gender.onEndEdit.AddListener(SubmitGender);
        input_name.onEndEdit.AddListener(SubmitName);
        Button button = submit.GetComponent<Button>();
        button.onClick.AddListener(SceneTransition);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SubmitTrials(string trials)
    {
        info.trials = trials;

    }

    private void SubmitID(string ID)
    {
        info.id = ID;

    }

    private void SubmitAge(string Age)
    {
        info.age = Age;
    }

    private void SubmitGender(string Gender)
    {
        info.gender = Gender;
    }

    private void SubmitName(string Name)
    {
        info.name = Name;
    }

    private void SceneTransition()
    {
        SceneManager.LoadScene("YMaze");
    }


}
