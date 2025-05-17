using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class TestInfo
{
    public static string nickname;
    public static string grade;
    public static string course;
    public static string topic;
}
public class DataCollection : MonoBehaviour
{
    public Button StartButton;
    public TMP_InputField Nickname;
    public TMP_InputField Grade;
    public TMP_InputField Course;
    public TMP_InputField Topic;
    void Start()
    {
        StartButton.onClick.AddListener(OnSubmitPressed);

    }

    public void OnSubmitPressed()
    {
        TestInfo.nickname = Nickname.text;
        TestInfo.grade = Grade.text;
        TestInfo.course = Course.text;
        TestInfo.topic = Topic.text;

        SceneManager.LoadScene("Questions");

    }

}
