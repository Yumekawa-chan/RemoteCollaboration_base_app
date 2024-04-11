using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform Panel;

    private bool panelFlug;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;
    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                if (item.CompareTag("Head"))
                {
                    item.enabled = true;
                }
                else
                {
                    item.enabled = false;
                }
            }
        }
        if (!photonView.IsMine)
    {
        Panel.gameObject.SetActive(false);
    }
    else
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("panel");
    }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            panelFlug = !panelFlug;
        }
        if (photonView.IsMine && panelFlug == false)
        {
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
            MapPosition(Panel, leftHandRig);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }

        if (photonView.IsMine && panelFlug == true)
        {
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
{
    if (photonView.IsMine)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float flexValue))
        {
            handAnimator.SetFloat("Flex", flexValue);
        }
        else
        {
            handAnimator.SetFloat("Flex", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float pinchValue))
        {
            handAnimator.SetFloat("Pinch", pinchValue);
        }
        else
        {
            handAnimator.SetFloat("Pinch", 0);
        }

        var handPose = DeterminePose(targetDevice, flexValue, pinchValue);
        handAnimator.SetInteger("Pose", handPose);
    }
}


    int DeterminePose(InputDevice targetDevice, float flexValue, float pinchValue)
    {
        if (flexValue > 0.5f && pinchValue > 0.5f)
        {
            return 1;
        }
        else if (flexValue > 0.5f)
        {
            return 2;
        }
        else if (pinchValue > 0.5f)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
