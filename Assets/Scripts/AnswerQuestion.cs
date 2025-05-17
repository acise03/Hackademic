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
        Time.timeScale = 0f; 
        SceneManager.LoadScene("Questions", LoadSceneMode.Additive);
    }
}
