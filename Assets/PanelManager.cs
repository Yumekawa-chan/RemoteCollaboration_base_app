using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public Camera displayRenderCamera; // RenderTextureに画像を書き込んでいるカメラ
    public GameObject displayGameObject; // RenderTextureを表示しているGameObject
    public LayerMask interactableLayers;  // インタラクト可能なレイヤー

    void Update()
    {
        bool gripHeld = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool triggerNotPressed = !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        // 指さしでパネル上のオブジェクトに触れたときに発火
        if (gripHeld && triggerNotPressed)
        {
            InteractWithRenderTexture();
        }
    }
    public void InteractWithRenderTexture() // パネル上のオブジェクトに触れたときの処理
    {
        // カメラを基準にViewPortからレイを生成
        Ray ray = displayRenderCamera.ViewportPointToRay(GetLocalHitPoint().position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f, interactableLayers))
        {
            var particleSystem = hit.collider.gameObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

    }

    public Transform GetLocalHitPoint() // パネル上のオブジェクトに触れたときのローカル座標を取得
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 screenPoint = displayRenderCamera.WorldToScreenPoint(controllerPosition);
        Ray ray = displayRenderCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayers))
        {
            return hit.transform;
        }
        return null;
    }


    public Vector3 GetViewPoint(Vector3 localHitPoint) // パネル上のオブジェクトに触れたときの世界座標を取得
    {
        if (localHitPoint == null)
        {
            return Vector3.zero;
        }

        Vector3 localPosition = localHitPoint - displayGameObject.transform.position;
        Bounds bounds = displayGameObject.GetComponent<Renderer>().bounds;
        Vector3 normalizedPosition = new Vector3(
            (localPosition.x + (bounds.size.x / 2)) / bounds.size.x,
            (localPosition.y + (bounds.size.y / 2)) / bounds.size.y
        );
        return normalizedPosition;
    }


}
