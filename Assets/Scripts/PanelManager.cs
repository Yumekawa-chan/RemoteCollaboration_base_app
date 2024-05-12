using UnityEngine;
using Photon.Pun;

public class PanelManager : MonoBehaviourPun
{
    public Camera displayRenderCamera; // RenderTextureに画像を書き込んでいるカメラ
    private GameObject displayGameObject; // RenderTextureを表示しているGameObject
    private Vector3? colliderPoint = null;

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

    private void InteractWithRenderTexture() // メイン処理
    {
        if (colliderPoint == null) return; // パネルに触れていない場合は処理を終了

        Vector3 localHitPoint = getLocalHitPoint();
        var displayGameObjectSize = displayGameObject.GetComponent<MeshRenderer>().bounds.size;

        // Viewportを計算
        var viewportPoint = new Vector3()
        {
            x = (localHitPoint.x / displayGameObjectSize.x) + 0.5f,  // 中心を0.5にする
            y = (localHitPoint.y / displayGameObjectSize.y) + 0.5f,  // 中心を0.5にする
        }; // 値域は0~1 絶対値なんかとろうとするなよ

        // カメラを基準にViewportからのレイを生成
        Ray ray = displayRenderCamera.ViewportPointToRay(viewportPoint);
        RaycastHit hit;

        // hitした場所の座標を取得 ※デバッグ用  
        Vector3 point = ray.GetPoint(2.0f);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = point;
        Cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Cube.GetComponent<Renderer>().material.color = Color.red;
        Destroy(Cube, 0.2f);

        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            // 検出した物体のパーティクルシステムを発火
            var cubeManager = hit.transform.GetComponent<CubeManager>();
            if (cubeManager != null)
            {
                cubeManager.StartParticleSystem();
            }
        }
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
                    // 他のプレイヤーのカメラを見つける
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
                        displayGameObject = panel;
                    }
                    else
                    {
                        Debug.LogWarning("Panel/Panel not found on my object");
                    }
                }
            }
        }
    }


    private Vector3 getLocalHitPoint() // パネル上の触れた部分のローカル座標を取得
    {
        if (colliderPoint != Vector3.zero)
        {
            return colliderPoint - displayGameObject.transform.position ?? Vector3.zero;
        }
        return Vector3.zero;
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
        if (other.gameObject.tag == "rightHand")
        {
            colliderPoint = null;
        }
    }
}