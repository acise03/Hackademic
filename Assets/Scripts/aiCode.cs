using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
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

    private List<string> questionPrompts = new List<string>();
    private int currentQuestionIndex = 0;

    void Start()
    {
        questionPrompts.Add("Generate a multiple choice question with 4 answer choices. Then, write the answer and a short explanation.");
        questionPrompts.Add("Generate a fill in the blank question. Then write the answer and a short explanation.");

        StartCoroutine(ReadApiKey());
    }

   IEnumerator ReadApiKey()
{
    string filePath = "Assets/Scripts/apiKey.txt";

#if UNITY_EDITOR || UNITY_STANDALONE
    apiKey = File.ReadAllText(filePath).Trim();
    yield break;

#elif UNITY_ANDROID
    UnityWebRequest reader = UnityWebRequest.Get(filePath);
    yield return reader.SendWebRequest();
    apiKey = reader.downloadHandler.text.Trim();
    yield break;

#else
    yield break;
#endif
}


    public void AskNextQuestion(int questionIndex)
    {

        StartCoroutine(SendRequest(questionPrompts[questionIndex]));
        currentQuestionIndex++;

    }

    IEnumerator SendRequest(string userPrompt)
    {
        Message systemMsg = new Message
        {
            role = "user",
            content = "You are an AI assistant tasked with generating questions for the user's upcoming test. (Background information: You are to refer to the user as " + TestInfo.nickname + ", who is in grade " + TestInfo.grade + ".) They are studying for their " + TestInfo.course + " course, specifically the topic is " + TestInfo.topic + "."
        };

        Message questionMsg = new Message
        {
            role = "user",
            content = userPrompt
        };

        RequestBody requestBody = new RequestBody
        {
            model = "llama-3.1-8b-instant",
            messages = new Message[] { systemMsg, questionMsg }
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
        string finishedResponse = convert(response.choices[0].message.content);
        outputText.text = finishedResponse;
    }

    private string convert(string input)
    {
        input = System.Text.RegularExpressions.Regex.Replace(input, @"\*\*(.+?)\*\*", "<b>$1</b>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"__(.+?)__", "<u>$1</u>");
        return input;
    }
}
