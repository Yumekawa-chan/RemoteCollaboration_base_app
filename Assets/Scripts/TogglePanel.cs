using UnityEngine;
using Photon.Pun;

public class TogglePanel : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject panel;


    void Update()
    {
        if (photonView.IsMine)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                photonView.RPC("PanelState", RpcTarget.All, !panel.activeSelf);
            }

        }
    }

    [PunRPC]
    void PanelState(bool state)
    {
        panel.SetActive(state);
    }
}
