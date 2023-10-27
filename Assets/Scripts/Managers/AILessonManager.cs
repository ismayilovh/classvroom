using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AILessonManager : MonoBehaviour
{
    public static AILessonManager Instance { get; private set; }
    
    private string _lesson;
    private string[] _dialogue;

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    public void GenerateLesson(string subject, string questionCount, string difficulty)
    {
        StartCoroutine(nameof(GetLesson), $"https://classvroom.tunardev.vercel.app/generate-exam?subject={subject}&difficulty={difficulty}");
    }

    IEnumerator GetLesson(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    _lesson = JsonUtility.FromJson<string>(webRequest.downloadHandler.text);
                    MakeDialogue();
                    DontDestroyOnLoad(gameObject);

                    break;
            }
        }
    }

    private void MakeDialogue()
    {
        _dialogue = _lesson.Split(".");
    }

    public string[] GetDialogue()
    {
        return _dialogue;
    }

    public string GetLesson()
    {
        return _lesson;
    }
}
