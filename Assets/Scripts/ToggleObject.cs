using UnityEngine;
using Photon.Pun;

public class ToggleObject : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject eyeGaze;


    void Update()
    {
        if (photonView.IsMine)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                photonView.RPC("ToggleEyeGaze", RpcTarget.All, !eyeGaze.activeSelf);
            }

        }
    }

    [PunRPC]
    void ToggleEyeGaze(bool state)
    {
        eyeGaze.SetActive(state);
    }
}
