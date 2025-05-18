using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;
using TMPro;
using System.Collections;

using System.Collections.Generic;

[Serializable]
public class Message { public string role, content; }
[Serializable]
public class RequestBody { public string model; public Message[] messages; }
[Serializable]
public class Choice { public Message message; }
[Serializable]
public class ResponseBody { public Choice[] choices; }

public class aiCode : MonoBehaviour
{
    public TextMeshProUGUI outputText;
    public TMP_InputField userAnswerInput;
    public TextMeshProUGUI feedbackText;

    private string apiKey;
    private const string endpoint = "https://api.groq.com/openai/v1/chat/completions";

    private readonly string[] questionPrompts = {
        "Generate a multiple choice question with 4 choices (A-D) about the topic. Format it as: Question: <text> \nA) <option1> \nB) <option2> \nC) <option3> \nD) <option4>",
        "Generate a fill-in-the-blank question about the topic. Format it as: Question: <text> [____] <text>."
    };

    private string lastQuestion = "";

    void Start()
    {
        StartCoroutine(ReadApiKey());
        AskNextQuestion(UnityEngine.Random.Range(0, questionPrompts.Length));
    }

    IEnumerator ReadApiKey()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        apiKey = File.ReadAllText("Assets/Scripts/apiKey.txt").Trim();
#elif UNITY_ANDROID
        UnityWebRequest reader = UnityWebRequest.Get("Assets/Scripts/apiKey.txt");
        yield return reader.SendWebRequest();
        apiKey = reader.downloadHandler.text.Trim();
#endif
        yield return null;
    }

    public void AskNextQuestion(int index)
    {
        if (index < questionPrompts.Length)
            StartCoroutine(SendRequest(questionPrompts[index]));
    }

    IEnumerator SendRequest(string prompt)
    {
        if (!TestInfo.ifNotesProvided)
        {
            string context = $"Use second person in your responses. You are an AI assistant tasked with creating questions for {TestInfo.nickname}, a grade {TestInfo.grade} student studying {TestInfo.course}. Topic: {TestInfo.topic}. Never include the answer and follow the format provided.";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    lastQuestion = convert(response);
                    outputText.text = lastQuestion;
                });
        }
        else
        {
            string context = $"Use second person in your responses. You are an AI assistant tasked with creating questions for {TestInfo.nickname}, with an upcoming test. Their notes are here: {TestInfo.providedNotes} Never include the answer and follow the format provided when making questions.";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    lastQuestion = convert(response);
                    outputText.text = lastQuestion;
                });
        }
    }

    public void SubmitAnswer()
    {
        StartCoroutine(SendAnswerForFeedback(userAnswerInput.text));
    }

    IEnumerator SendAnswerForFeedback(string answer)
    {
        if (!TestInfo.ifNotesProvided)
        {
            string prompt = $"You asked this question: \"{lastQuestion}\". I answered: \"{answer}\". Provide feedback my the answer. If correct, explain briefly. If wrong, explain and give the correct answer. Feel free to suggest memorization or study tricks such as mnemonics, visualization or the method of loci.";

            string context = $"Use second person. You are an AI assistant assessing practice questions for {TestInfo.nickname}, a grade {TestInfo.grade} student in {TestInfo.course} on the topic {TestInfo.topic}.";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    string feedback = convert(response);
                    feedbackText.text = feedback;
                    Debug.Log(feedback);
                });

            StartCoroutine(NumericalGrade(answer));

            prompt = $"The following question was asked: \"{lastQuestion}\". The user answered: \"{answer}\". Write brief formatted notes (around 3 points) on this question topic.";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    string feedback = convert(response);
                    TestInfo.notes.Add(feedback);
                });

        }
        else
        {
            string prompt = $"You asked this question: \"{lastQuestion}\". I answered: \"{answer}\". Provide feedback my the answer. Please use the notes provided by me in your feedback: {TestInfo.providedNotes}. If correct, explain briefly. If wrong, explain and give the correct answer. Feel free to suggest memorization or study tricks such as mnemonics, visualization or the method of loci.";

            string context = $"Use second person. You are an AI assistant assessing practice questions.";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    string feedback = convert(response);
                    feedbackText.text = feedback;
                    Debug.Log(feedback);
                });

            StartCoroutine(NumericalGrade(answer));

            prompt = $"The following question was asked: \"{lastQuestion}\". The user answered: \"{answer}\". Write brief formatted notes basing off of the user's notes (around 3 points) on this question topic. The users' provided notes are: {TestInfo.providedNotes}";

            yield return SendChatRequest(
                new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = prompt }
                },
                response =>
                {
                    string feedback = convert(response);
                    TestInfo.notes.Add(feedback);
                });

        }
    }

    IEnumerator NumericalGrade(string answer)
    {
        string context = $"Use second person. You are grading for {TestInfo.nickname}, grade {TestInfo.grade}, course {TestInfo.course}, topic {TestInfo.topic}. The question was: {lastQuestion}. First, decide on what you think the answer to that question is, then if the user's answer: {answer} is the same, output 1. Otherwise, output 0. There should only be one integer output, of either 0 or 1.";

        yield return SendChatRequest(
            new Message[] {
                new Message { role = "system", content = context },
                new Message { role = "user", content = answer }
            },
            response =>
            {
                int points = int.TryParse(response.Trim(), out int result) ? result : 0;
                feedbackText.text += $"\n\nYour points for this question is: {points}/1";
                TestInfo.correctAnswers += points;
                TestInfo.totalQuestions++;
                Points.rating += points * 5;
                Debug.Log(points);
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

        string responseText = request.downloadHandler.text;
        ResponseBody resBody = JsonUtility.FromJson<ResponseBody>(responseText);
        onSuccess?.Invoke(resBody.choices[0].message.content);
    }

    private string convert(string input)
    {
        input = System.Text.RegularExpressions.Regex.Replace(input, @"\*\*(.+?)\*\*", "<b>$1</b>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"__(.+?)__", "<u>$1</u>");
        return input;
    }
}
