using UnityEngine;
using Photon.Pun;

public class PanelManager : MonoBehaviourPun
{
    public Camera displayRenderCamera; // RenderTextureに画像を書き込んでいるカメラ
    public GameObject displayGameObject; // RenderTextureを表示しているGameObject
    private bool collisionStatus = false;
    private Vector3 colliderPoint = Vector3.zero;

    void Start()
    {
        PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();

        foreach (PhotonView view in allPhotonViews)
        {
            if (!view.IsMine && view.Owner != null) // このPhotonViewがローカルプレイヤーのものでないことを確認
            {
                // プレイヤーのHeadの下にあるCameraを探す
                Camera foundCamera = view.gameObject.transform.Find("Head/ViewCamera")?.GetComponent<Camera>();
                if (foundCamera != null)
                {
                    displayRenderCamera = foundCamera;
                    break;
                }
            }
        }
    }
    void Update()
    {
        bool gripHeld = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool triggerNotPressed = !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (gripHeld && triggerNotPressed && collisionStatus)
        {
            InteractWithRenderTexture();
        }
    }

    private void InteractWithRenderTexture() // メイン処理
    {
        Vector3 localHitPoint = getlocalHitPoint();
        var displayGameObjectSize = displayGameObject.GetComponent<MeshRenderer>().bounds.size;

        // Viewportを計算
        var viewportPoint = new Vector3()
        {
            x = (localHitPoint.x + (displayGameObjectSize.x / 2)) / displayGameObjectSize.x,
            y = (localHitPoint.y + (displayGameObjectSize.y / 2)) / displayGameObjectSize.y,
            // x = localHitPoint.x / displayGameObjectSize.x,
            // y = localHitPoint.y / displayGameObjectSize.y,
        };

        // カメラを基準にViewportからのレイを生成
        Ray ray = displayRenderCamera.ViewportPointToRay(viewportPoint);
        RaycastHit hit;

        // hitした場所の座標を取得 ※デバッグ用  
        Vector3 point = ray.GetPoint(2.0f);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = point;
        Cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(Cube, 0.1f);

        if (Physics.Raycast(ray, out hit, 2.0f))
        {
            // 検出した物体のパーティクルシステムを発火
            var cubeManager = hit.transform.GetComponent<CubeManager>();
            if (cubeManager != null)
            {
                cubeManager.StartParticleSystem();
            }
        }

    }

    private Vector3 getlocalHitPoint() // パネル上の触れた部分のローカル座標を取得
    {
        Vector3 localHitPoint = colliderPoint;
        if (localHitPoint != Vector3.zero)
        {
            return localHitPoint - displayGameObject.transform.position;
        }
        return Vector3.zero;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rightHand")
        {
            collisionStatus = true;
            colliderPoint = other.ClosestPointOnBounds(transform.position);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "rightHand")
        {
            collisionStatus = false;
            colliderPoint = Vector3.zero;
        }
    }
}
