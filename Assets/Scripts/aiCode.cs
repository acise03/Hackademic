using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
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

    public void OnSubmitButtonPressed()
    {
        SubmitAnswer();
    }
    private string apiKey;
    private const string endpoint = "https://api.groq.com/openai/v1/chat/completions";

    public TextMeshProUGUI outputText;
    public TMP_InputField userAnswerInput;
    public TextMeshProUGUI feedbackText;

    private List<string> questionPrompts = new List<string>();
    private int currentQuestionIndex = 0;
    private string lastQuestion = "";

    void Start()
    {
        questionPrompts.Add("Generate a multiple choice question with 4 answer choices");
        questionPrompts.Add("Generate a fill in the blank question");

        StartCoroutine(ReadApiKey());
        System.Random rand = new System.Random();
        int number = rand.Next(0, 2);
        AskNextQuestion(number);
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
        if (questionIndex >= questionPrompts.Count) return;
        StartCoroutine(SendRequest(questionPrompts[questionIndex]));
        currentQuestionIndex++;
    }

    IEnumerator SendRequest(string userPrompt)
    {
        Message systemMsg = new Message
        {
            role = "user",
            content = "You are an AI assistant tasked with generating questions for the user's upcoming test. (Background information: " + TestInfo.nickname + " is in grade " + TestInfo.grade + ".) They are studying for their " + TestInfo.course + " course, specifically the topic is " + TestInfo.topic + ". Remember to keep responses as short and clean as possible."
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
        lastQuestion = outputText.text;
    }

    public void SubmitAnswer()
    {
        string userAnswer = userAnswerInput.text;
        StartCoroutine(SendAnswerForFeedback(userAnswer));
    }

    IEnumerator SendAnswerForFeedback(string userAnswer)
    {
        string prompt = $"The following question was asked: \"{lastQuestion}\". " +
                        $"The user answered: \"{userAnswer}\". Please give constructive feedback and indicate whether the answer is correct.";

        Message systemMsg = new Message
        {
            role = "system",
            content = "You are a helpful tutor giving feedback on a student's test answers."
        };

        Message feedbackMsg = new Message
        {
            role = "user",
            content = prompt
        };

        RequestBody requestBody = new RequestBody
        {
            model = "llama-3.1-8b-instant",
            messages = new Message[] { systemMsg, feedbackMsg }
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
        string feedback = convert(response.choices[0].message.content);
        feedbackText.text = feedback;
    }

    private string convert(string input)
    {
        input = System.Text.RegularExpressions.Regex.Replace(input, @"\*\*(.+?)\*\*", "<b>$1</b>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"__(.+?)__", "<u>$1</u>");
        return input;
    }
}
