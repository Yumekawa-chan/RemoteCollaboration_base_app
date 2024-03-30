using UnityEngine;
using Photon.Pun;

public class ToggleObject : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject eyeGaze;
    [SerializeField]

    private GameObject panel;

    void Update()
    {
        if (photonView.IsMine)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                photonView.RPC("ToggleEyeGaze", RpcTarget.All, !eyeGaze.activeSelf);
            }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                photonView.RPC("TogglePanel", RpcTarget.All, !panel.activeSelf);
            }
        }
    }

    [PunRPC]
    void ToggleEyeGaze(bool state)
    {
        eyeGaze.SetActive(state);
    }
    [PunRPC]
    void TogglePanel(bool state)
    {
        panel.SetActive(state);
    }
}
