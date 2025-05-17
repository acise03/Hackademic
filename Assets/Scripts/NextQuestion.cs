using UnityEngine;
using UnityEngine.UI;

public class NextQuestion : MonoBehaviour
{
    public Button NextButton;
    public aiCode aiCodeScript;

    void Start()
    {
        NextButton.onClick.AddListener(OnNextPressed);
    }

    public void OnNextPressed()
    {
        int value = UnityEngine.Random.Range(0, 2);
        aiCodeScript.AskNextQuestion(value);
    }
}
