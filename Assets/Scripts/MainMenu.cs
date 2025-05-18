using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button TopicButton;
    public Button UploadButton;
    public Button RecordButton;

    void Start()
    {
        TopicButton.onClick.AddListener(OnTopicPressed);
        UploadButton.onClick.AddListener(OnUploadPressed);
        RecordButton.onClick.AddListener(OnRecordPressed);
    }

    public void OnTopicPressed()
    {
        SceneManager.LoadScene("TestInfo");
    }
    public void OnRecordPressed()
    {
        SceneManager.LoadScene("Record");
    }
    public void OnUploadPressed()
    {
        SceneManager.LoadScene("UploadNotes");
    }
    

}
