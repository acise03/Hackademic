using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Recording : MonoBehaviour
{
    DictationRecognizer rec; public Button nextButton;

    bool isRecording = false;

    [SerializeField] public TMP_Text display;
    [SerializeField] public TMP_Text statusLabel;

    void Start()
    {
        nextButton.onClick.AddListener(OnSubmitPressed);

        rec = new DictationRecognizer();

        rec.DictationResult += (text, confidence) =>
        {
            Debug.Log("Recognized: " + text);
            display.text += text + " ";

        };

        rec.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogError("Dictation stopped unexpectedly: " + completionCause);
            isRecording = false;
            UpdateStatusLabel();
        };

        rec.DictationError += (error, hresult) =>
        {
            Debug.LogError("Dictation error: " + error);
            isRecording = false;
            UpdateStatusLabel();
        };
    }

    void OnSubmitPressed()
    {
        RecordedNotes.recordedNotesS = display.text;
        Debug.Log("display.text");
        SceneManager.LoadScene("LectureToNotes");

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRecording)
                StopRecording();
            else
                StartRecording();
        }
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            rec.Start();
            isRecording = true;
            Debug.Log("Recording started.");
            UpdateStatusLabel();
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            rec.Stop();
            isRecording = false;
            Debug.Log("Recording stopped.");
            UpdateStatusLabel();
        }
    }

    private void UpdateStatusLabel()
    {
        if (statusLabel != null)
            statusLabel.text = isRecording ? "...Listening..." : "Not Recording";
    }

    void OnApplicationQuit()
    {
        if (rec != null)
        {
            rec.Dispose();
        }
    }
}
