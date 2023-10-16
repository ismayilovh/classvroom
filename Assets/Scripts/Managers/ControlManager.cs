using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private GameObject controlCanvas;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            controlCanvas.SetActive(!controlCanvas.activeInHierarchy);
        }
    }

    public void LeaveClassroom()
    {
        Destroy(ClassroomManager.Instance.gameObject);
        PhotonManager.Instance.LeaveRoom();
    }

    public void QuitApp()
    {
        LeaveClassroom();
        Application.Quit();
    }
}
