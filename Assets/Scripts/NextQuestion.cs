using UnityEngine;
using UnityEngine.UI;

public class NextQuestion : MonoBehaviour
{
    public Button SubmitButton;
    public aiCode aiCodeScript;

    void Start()
    {
        SubmitButton.onClick.AddListener(aiCodeScript.SubmitAnswer);
    }
}
