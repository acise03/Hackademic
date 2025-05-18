using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class home : MonoBehaviour
{
    public Button Home;
    void Start()
    {
        Home.onClick.AddListener(onHomePressed);
    }

    // Update is called once per frame
    void onHomePressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
