using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Upload : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button uploadButton;


    void Start()
    {

        uploadButton.onClick.AddListener(StoreTMPText);
    }

    void StoreTMPText()
    {
        TestInfo.providedNotes = inputField.text;
        TestInfo.ifNotesProvided = true;
        SceneManager.LoadScene("GameScene");



    }
}
