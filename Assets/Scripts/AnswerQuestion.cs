using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class UIReference
{
    public static GameObject oldUI;
}

public class AnswerQuestion : MonoBehaviour
{

    public GameObject oldUI; public Button AnswerQuestionB;

    void Start()
    {
        AnswerQuestionB.onClick.AddListener(OnAnswerQuestionPressed);
    }

    public void OnAnswerQuestionPressed()
    {
        Time.timeScale = 0f; UIReference.oldUI = oldUI;

        oldUI.SetActive(false);

        SceneManager.LoadScene("Questions", LoadSceneMode.Additive);
    }
}
