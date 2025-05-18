using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class TestInfo
{
    public static string nickname = "Jammy";
    public static string grade = "3";
    public static string course = "Math";
    public static string topic = "Arithmetic";
    public static int correctAnswers = 0;
    public static int totalQuestions;
    public static string providedNotes;
    public static bool ifNotesProvided = false;
    public static List<string> notes = new List<string>();
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

        SceneManager.LoadScene("GameScene");

    }



}