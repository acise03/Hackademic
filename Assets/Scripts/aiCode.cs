using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;
using TMPro;


[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class RequestBody
{
    public string model;
    public Message[] messages;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class ResponseBody
{
    public Choice[] choices;
}

public class aiCode : MonoBehaviour
{
    private string apiKey;
    private const string endpoint = "https://api.groq.com/openai/v1/chat/completions";

    public TextMeshProUGUI outputText;

    void Start()
    {
        StartCoroutine(ReadApiKeyAndSendRequest());
    }

    IEnumerator ReadApiKeyAndSendRequest()
    {
        string filePath = "Assets/Scripts/apiKey.txt";

#if UNITY_EDITOR || UNITY_STANDALONE
        apiKey = File.ReadAllText(filePath).Trim();
#elif UNITY_ANDROID
        UnityWebRequest reader = UnityWebRequest.Get(filePath);
        yield return reader.SendWebRequest();
        apiKey = reader.downloadHandler.text.Trim();
#endif

        yield return StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        RequestBody requestBody = new RequestBody
        {
            model = "llama-3.1-8b-instant",
            messages = new Message[]
            {
                new Message { role = "user", content = "You are an ai assistant tasked with generating questions for the user's upcoming test. You are to refer to the user as " + TestInfo.nickname + ", who is in grade " + TestInfo.grade + ". They are studying for their " + TestInfo.course + " course, specifically the topic is " + TestInfo.topic + ". Generate one multiple choice question with 4 answer choices. Then, write the answer and a short explanation." }
            }
        };

        string jsonData = JsonUtility.ToJson(requestBody);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        ResponseBody response = JsonUtility.FromJson<ResponseBody>(responseText);
        string finishedResponse = response.choices[0].message.content;
        outputText.text = finishedResponse;



    }
}
