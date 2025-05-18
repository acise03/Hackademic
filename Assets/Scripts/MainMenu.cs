using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button TopicButton;
    public Button UploadButton;

    void Start()
    {
        TopicButton.onClick.AddListener(OnTopicPressed);
        UploadButton.onClick.AddListener(OnUploadPressed);
    }

    public void OnTopicPressed()
    {
        SceneManager.LoadScene("TestInfo");
    }
    public void OnUploadPressed()
    {
        SceneManager.LoadScene("UploadNotes");
    }
    

}
