using System.Collections;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

public class ClassroomManager : MonoBehaviourPunCallbacks
{
    public static ClassroomManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Classroom")
        {
            InstantiatePlayer();
        }
    }

    private void InstantiatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Manager"), Vector3.zero, Quaternion.identity);
    }
    
    public void GenerateExam()
    {
        string subject = "physics";
        int questionCount = 10;
        string difficulty = "average";
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
                    Exam exam = JsonUtility.FromJson<Exam>("{\"questions\":" + examString + "}");

                    foreach (Question question in exam.questions)
                    {
                        Debug.Log("Question: " + question.question);
                        Debug.Log("Variants: " + string.Join(", ", question.variants));
                        Debug.Log("Correct Answer: " + question.correct);
                    }
                    break;
            }
        }
    }
}