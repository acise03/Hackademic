using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;
using TMPro;
using System.Collections;

using System.Collections.Generic;


public class Download : MonoBehaviour
{
    [SerializeField] Button DownloadButton;
    public TextMeshProUGUI results;
    public TextMeshProUGUI keyPoints;


    // Start is called before the first frame update
    void Start()

    {
        StartCoroutine(ReadApiKey());
        DownloadButton.onClick.AddListener(ExportNotesToFile);
        results.text = "Score: " + TestInfo.correctAnswers + "/" + TestInfo.totalQuestions;
        SummarizeNotes(TestInfo.notes);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExportNotesToFile()
    {
        string fileName = "StudyNotes.txt";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, string.Join("\n\n---------------------------\n\n", TestInfo.notes));
        Debug.Log("Notes exported to: " + path);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
#endif
    }


    private string apiKey;
    private const string endpoint = "https://api.groq.com/openai/v1/chat/completions";

    private readonly string[] questionPrompts = {
        "Generate a multiple choice question with 4 choices (A-D) about the topic. Format it as: Question: <text> \nA) <option1> \nB) <option2> \nC) <option3> \nD) <option4>",
        "Generate a fill-in-the-blank question about the topic. Format it as: Question: <text> [____] <text>."
    };

    private string lastQuestion = "";


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
    public void SummarizeNotes(List<string> notes)
    {
        StartCoroutine(Summarize(string.Join("\n\n---------------------------\n\n", notes)));
    }


    IEnumerator Summarize(string answer)
    {
        string prompt = $"Summarize the key information from: {answer}. Maximum of 4-5 sentences.";


        yield return SendChatRequest(
            new Message[] {
                new Message { role = "user", content = prompt }
            },
            response =>
            {
                string feedback = convert(response);
                keyPoints.text = "KEY POINTS:\n" + feedback;
            });

    }

    private string convert(string input)
    {
        input = System.Text.RegularExpressions.Regex.Replace(input, @"\*\*(.+?)\*\*", "<b>$1</b>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        input = System.Text.RegularExpressions.Regex.Replace(input, @"__(.+?)__", "<u>$1</u>");
        return input;
    }
}

