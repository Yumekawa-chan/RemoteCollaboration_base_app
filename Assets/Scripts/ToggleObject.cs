using UnityEngine;
using Photon.Pun;

public class ToggleObject : MonoBehaviourPunCallbacks
{
    public GameObject eyeGaze;

    void Update()
    {
        if (photonView.IsMine)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                if (eyeGaze != null)
                {
                    eyeGaze.SetActive(!eyeGaze.activeSelf);
                }
            }
        }
    }
}
