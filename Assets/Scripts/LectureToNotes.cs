using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class RecordedNotes
{
    public static string recordedNotesS;
}

public class LectureToNotes : MonoBehaviour
{
    public TextMeshProUGUI outputText;
    public Button StartGame;

    private string apiKey;
    private const string endpoint = "https://api.groq.com/openai/v1/chat/completions";

    void Start()
    {
        Debug.Log("notes: " + RecordedNotes.recordedNotesS);
        StartGame.onClick.AddListener(gogo);
        StartCoroutine(PrepareAndSend());
    }

    void gogo()
    {
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator PrepareAndSend()
    {
        yield return StartCoroutine(ReadApiKey());

        while (string.IsNullOrEmpty(RecordedNotes.recordedNotesS))
        {
            yield return null;
        }

        string prompt = "Generate notes based on the recording of a lecture converted to text: " + RecordedNotes.recordedNotesS;
        yield return StartCoroutine(SendRequest(prompt));
    }

    IEnumerator ReadApiKey()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
    string path = Path.Combine(Application.dataPath, "Scripts/apiKey.txt");
    if (File.Exists(path))
    {
        apiKey = File.ReadAllText(path).Trim();
    }
    else
    {
        Debug.LogError("API key file not found at: " + path);
        yield break;
    }
#elif UNITY_ANDROID
    UnityWebRequest reader = UnityWebRequest.Get(Application.streamingAssetsPath + "/Scripts/apiKey.txt");
    yield return reader.SendWebRequest();

    if (reader.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Failed to read API key: " + reader.error);
        yield break;
    }

    apiKey = reader.downloadHandler.text.Trim();
#endif

        yield return null;
    }


    IEnumerator SendRequest(string prompt)
    {
        yield return SendChatRequest(
            new Message[] {
                new Message { role = "user", content = prompt }
            },
            response =>
            {
                string converted = ConvertMarkdownToRichText(response);
                outputText.text = converted;
                TestInfo.providedNotes = converted;
                TestInfo.ifNotesProvided = true;
            });
    }

    IEnumerator SendChatRequest(Message[] messages, Action<string> onSuccess)
    {
        RequestBody body = new RequestBody { model = "llama-3.1-8b-instant", messages = messages };
        byte[] data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));

        using UnityWebRequest request = new UnityWebRequest(endpoint, "POST")
        {
            uploadHandler = new UploadHandlerRaw(data),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + request.error + "\n" + request.downloadHandler.text);
            yield break;
        }

        string responseText = request.downloadHandler.text;
        ResponseBody resBody = JsonUtility.FromJson<ResponseBody>(responseText);

        if (resBody != null && resBody.choices != null && resBody.choices.Length > 0)
        {
            onSuccess?.Invoke(resBody.choices[0].message.content);
        }
        else
        {
            Debug.LogError("Unexpected API response: " + responseText);
        }
    }

    private string ConvertMarkdownToRichText(string input)
    {
        input = System.Text.RegularExpressions.Regex.Replace(input, @"\*\*(.+?)\*\*", "<b>$1</b>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"(?<!\*)\*(?!\*)(.+?)\*(?!\*)", "<i>$1</i>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"__(.+?)__", "<u>$1</u>");
        return input;
    }
}