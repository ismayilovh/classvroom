using System.IO;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}