using System.Collections;
using TMPro;
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

        [SerializeField] private TMP_Text messageText;
        
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
                        SendErrorMessage(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        SendErrorMessage(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        string examString = webRequest.downloadHandler.text;
                        SendDebugMessage("Exam was generated succesfully.");
                        _exam = JsonUtility.FromJson<Exam>("{\"questions\":" + examString + "}");
                        DontDestroyOnLoad(gameObject);
    
                        break;
                }
            }
        }
        
        void SendDebugMessage(string message)
        {
            messageText.color = Color.green;
            messageText.text = message;
        }
        
        void SendErrorMessage(string message)
        {
            messageText.color = Color.red;
            messageText.text = message;
        }

        public Exam GetExam()
        {
            return _exam;
        }
    }
}