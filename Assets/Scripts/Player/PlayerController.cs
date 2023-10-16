using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviourPun
{
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(transform.Find("Locomotion System").gameObject);
            foreach (var controller in GetComponentsInChildren<ActionBasedController>())
            {
                controller.enableInputActions = false;
                controller.enableInputTracking = false;
                controller.gameObject.GetComponent<XRRayInteractor>().enabled = false;
            }
        }
    }
}