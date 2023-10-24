using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    [System.Serializable]
    public class Question
    {
        public string question;
        public string[] variants;
        public string correct;
    }

    [System.Serializable]
    public class Exam
    {
        public Question[] questions;
    }
    
    public class ExamManager : MonoBehaviour
    {
        public static ExamManager Instance { get; private set; }
        private Exam _exam;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public void GenerateExam(string subject, string questionCount, string difficulty)
        {
            StartCoroutine(nameof(GetExam), $"https://classvroom.tunardev.vercel.app/generate-exam?subject={subject}&questionCount={questionCount}difficulty={difficulty}");
        }
    
        IEnumerator GetExam(string uri)
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
                        string examString = webRequest.downloadHandler.text;
                        Debug.Log(pages[page] + ":\nReceived: " + examString);
                        _exam = JsonUtility.FromJson<Exam>("{\"questions\":" + examString + "}");
                        DontDestroyOnLoad(gameObject);
    
                        break;
                }
            }
        }

        public Exam GetExam()
        {
            return _exam;
        }
    }
}