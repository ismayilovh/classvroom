using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _pv;
    
    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_pv.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        //await Task.Delay(1000);
        PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Controller"), Vector3.zero, Quaternion.identity);
    }
}