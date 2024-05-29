using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class PanelManager : MonoBehaviourPun
{
    public Camera displayRenderCamera; // RenderTextureに画像を書き込んでいるカメラ
    private RawImage displayGameObject; // RenderTextureを表示しているGameObject
    private Vector3? colliderPoint = null;
    private bool isCooldown = false;
    void Start()
    {
        InitializeCameraAndPanel();
    }

    void Update()
    {
        bool gripHeld = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool triggerNotPressed = !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (gripHeld && triggerNotPressed)
        {
            InteractWithRenderTexture();
        }
        InitializeCameraAndPanel();
    }

    private void InteractWithRenderTexture()
    {
        if (!colliderPoint.HasValue || isCooldown) return;

        Vector2 localHitPoint = displayGameObject.transform.InverseTransformPoint(colliderPoint.Value);
        var rect = displayGameObject.rectTransform.rect;
        Vector2 textureCoord = localHitPoint - rect.min;
        textureCoord.x *= displayGameObject.uvRect.width / rect.width;
        textureCoord.y *= displayGameObject.uvRect.height / rect.height;
        textureCoord += displayGameObject.uvRect.min;
        textureCoord = new Vector2(1 - textureCoord.x, 1 - textureCoord.y + 0.1f);

        Ray ray = displayRenderCamera.ViewportPointToRay(textureCoord);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            var cubeManager = hit.transform.GetComponent<CubeManager>();
            if (cubeManager != null)
            {
                StartCoroutine(ActivateWithCooldown(cubeManager));
                StartCoroutine(Vibrate(duration: 0.2f, controller: OVRInput.Controller.RTouch));
            }
        }
    }

    IEnumerator ActivateWithCooldown(CubeManager cubeManager)
    {
        isCooldown = true;
        cubeManager.StartParticleSystem();
        yield return new WaitForSeconds(2f);
        isCooldown = false;
    }

    public static IEnumerator Vibrate(float duration = 0.1f, float frequency = 0.1f, float amplitude = 0.1f, OVRInput.Controller controller = OVRInput.Controller.Active)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);

        yield return new WaitForSeconds(duration);

        OVRInput.SetControllerVibration(0, 0, controller);
    }

    private void InitializeCameraAndPanel()
    {
        PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();

        foreach (PhotonView view in allPhotonViews)
        {
            if (view.Owner != null)
            {
                if (view.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    GameObject camera = view.gameObject.transform.Find("Head/ViewCamera")?.gameObject;
                    if (camera != null)
                    {
                        displayRenderCamera = camera.GetComponent<Camera>();
                    }
                }
                else if (view.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    GameObject panel = view.gameObject.transform.Find("Panel/Panel")?.gameObject;
                    if (panel != null)
                    {
                        displayGameObject = panel.GetComponent<RawImage>();
                    }
                    else
                    {
                        Debug.LogWarning("Panel/Panel not found on my object");
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("rightHand"))
        {
            var plane = new Plane(transform.forward, transform.position);
            colliderPoint = plane.ClosestPointOnPlane(other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "rightHand")
        {
            colliderPoint = null;
        }
    }
}
