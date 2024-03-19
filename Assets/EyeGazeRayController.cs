using UnityEngine;
using Photon.Pun; // PUN2の名前空間を使用

public class EyeGazeRayController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform eyeTransform; // 目のTransformをInspectorから設定
    public float rayLength = 10f; // Rayの長さ
    private LineRenderer lineRenderer; // LineRendererの参照

    private bool isRayActive = false; // Rayがアクティブかどうかのフラグ

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>(); // LineRendererコンポーネントの取得
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Oculusの入力に応じてRayのon/offを切り替え
            if (OVRInput.GetDown(OVRInput.Button.One)) // Aボタン
            {
                isRayActive = true;
            }
            else if (OVRInput.GetDown(OVRInput.Button.Two)) // Bボタン
            {
                isRayActive = false;
            }

            // Rayの描画と更新
            UpdateRayVisualization(isRayActive);
        }
    }

    private void UpdateRayVisualization(bool isActive)
    {
        if (isActive)
        {
            RaycastHit hit;
            Vector3 forward = eyeTransform.forward;
            Vector3 startPoint = eyeTransform.position;
            Vector3 endPoint = startPoint + forward * rayLength;

            if (Physics.Raycast(startPoint, forward, out hit, rayLength))
            {
                endPoint = hit.point; // Rayが何かに当たった場合は、その点を終点とする
            }

            // LineRendererでRayを描画
            lineRenderer.SetPositions(new Vector3[] { startPoint, endPoint });
            lineRenderer.enabled = true; // LineRendererを有効にする
        }
        else
        {
            lineRenderer.enabled = false; // LineRendererを無効にする
        }
    }

    // IPunObservableの実装で、Rayの状態を同期する
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // データを送信する
            stream.SendNext(isRayActive);
        }
        else
        {
            // データを受信する
            isRayActive = (bool)stream.ReceiveNext();
        }
    }
}
