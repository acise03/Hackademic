using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class Points
{
    public static int rating = 5;
}
public class ContinueFromQuestion : MonoBehaviour
{
    public Button ContinueButton;

    void Start()
    {
        ContinueButton.onClick.AddListener(OnContinuePressed);
    }

    public void OnContinuePressed()
    {
        SceneManager.LoadScene("GameScene");
    }
}
