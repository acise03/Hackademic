using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnswerQuestion : MonoBehaviour
{
    public Button AnswerQuestionB;

    void Start()
    {
        AnswerQuestionB.onClick.AddListener(OnAnswerQuestionPressed);
    }

    public void OnAnswerQuestionPressed()
    {
        SceneManager.LoadScene("Questions");
    }
}
