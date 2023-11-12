using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AILessonManager : MonoBehaviour
{
    public static AILessonManager Instance { get; private set; }
    
    private string _lesson;
    private string[] _dialogue;

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    public void GenerateLesson(string subject, string lesson)
    {
        // SceneManager.LoadScene("AI Lesson"); // TODO: Remove this
        StartCoroutine(nameof(GetLesson), $"https://classvroom.tunardev.vercel.app/generate-text?subject={subject}&lesson={lesson}");
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
                    _lesson = webRequest.downloadHandler.text;//.Replace("*", string.Empty);
                    MakeDialogue();
                    SceneManager.LoadScene("AI Lesson");
                    DontDestroyOnLoad(gameObject);

                    break;
            }
        }
    }

    private void MakeDialogue()
    {
        // var _lessonCleaned = _lesson.Remove('*');
        _dialogue = _lesson.Split(".");//_lessonCleaned.Split(".");
        foreach (var sentence in _dialogue)
        {
            Debug.Log(sentence);
        }
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