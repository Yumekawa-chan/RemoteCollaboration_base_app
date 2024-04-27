using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    public Camera displayRenderCamera; // RenderTextureに画像を書き込んでいるカメラ
    public GameObject displayGameObject; // RenderTextureを表示しているGameObject
    public bool collisionStatus = false;


    void Update()
    {
        bool gripHeld = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool triggerNotPressed = !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (gripHeld && triggerNotPressed && collisionStatus)
        {
            Destroy(displayGameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rightHand")
        {
            collisionStatus = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "rightHand")
        {
            collisionStatus = false;
        }
    }
}
