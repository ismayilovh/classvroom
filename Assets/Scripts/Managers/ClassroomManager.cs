using System.IO;
using Managers;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomManager : MonoBehaviourPunCallbacks
{
    public static ClassroomManager Instance;
    private Exam _exam;

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
            if (_exam != null)
            {
                MakeExamDay(_exam);
            }
        }
    }

    private void InstantiatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Manager"), Vector3.zero, Quaternion.identity);
        
    }

    public void SetExam(Exam exam)
    {
        _exam = exam;
    }

    private void MakeExamDay(Exam exam)
    {
        photonView.RPC("SendExam", RpcTarget.All , exam);
    }

    [PunRPC]
    public void SendExam(Exam exam)
    {
        Debug.Log($"Exam received by {gameObject.name}");
    }
}