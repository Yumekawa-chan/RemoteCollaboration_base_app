using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PanelManager : MonoBehaviourPun
{
    public Camera displayRenderCamera; // Camera rendering the image to the RenderTexture
    private RawImage displayGameObject; // Display panel as a RawImage
    private Vector3? colliderPoint = null; // Nullable Vector3 to represent the collision point

    void Start()
    {
        InitializeCameraAndPanel();
    }

    void Update()
    {
        bool gripHeld = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool triggerNotPressed = !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (gripHeld && triggerNotPressed && colliderPoint != null)
        {
            InteractWithRenderTexture();
        }
        InitializeCameraAndPanel();
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
                        Debug.Log(displayRenderCamera);
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

    private void InteractWithRenderTexture() // Main logic
    {
        if (colliderPoint == null) return;

        Vector3 worldSpaceHitPoint = colliderPoint.Value;

        Vector2 localHitPoint = displayGameObject.rectTransform.InverseTransformPoint(worldSpaceHitPoint);

        var rect = displayGameObject.rectTransform.rect;
        Vector2 textureCoord = localHitPoint - rect.min;
        textureCoord.x *= displayGameObject.uvRect.width / rect.width;
        textureCoord.y *= displayGameObject.uvRect.height / rect.height;
        textureCoord += displayGameObject.uvRect.min;

        Ray ray = displayRenderCamera.ViewportPointToRay(textureCoord);
        // Making Cube for Debugging
        Vector3 point = ray.GetPoint(2.0f);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = point;
        Cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Cube.GetComponent<Renderer>().material.color = Color.red;
        Destroy(Cube, 0.1f);

        if (Physics.Raycast(ray, out var hit, 10.0f))
        {
            if (hit.transform.TryGetComponent<CubeManager>(out var cubeManager))
            {
                cubeManager.StartParticleSystem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("rightHand"))
        {
            var plane = new Plane(transform.forward, transform.position);

            colliderPoint = plane.ClosestPointOnPlane(other.bounds.center);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("rightHand"))
        {
            colliderPoint = null;
        }
    }
}
