using UnityEngine;

public class BodyPositionCalculator : MonoBehaviour
{
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform bodyTransform;

    public float bodyHeightOffset = 0.3f; // 胴体の高さのオフセット
    public float minimumBodyHeight = 0.5f; // 胴体の最低高さ
    public float maxHorizontalDistance = 0.2f; // 胴体と頭の最大水平距離

    void Update()
    {
        if (headTransform != null && leftHandTransform != null && rightHandTransform != null && bodyTransform != null)
        {
            UpdateBodyPosition();
            AdjustHeadPosition();
        }
    }

    private void UpdateBodyPosition()
    {
        // 胴体の基本位置は頭と両手の平均位置
        Vector3 averageHandPosition = (leftHandTransform.position + rightHandTransform.position) / 2f;
        
        // 頭の位置は、Y軸は頭に、XZ軸は手の平均位置に合わせる
        Vector3 adjustedHeadPosition = new Vector3(
            averageHandPosition.x, 
            headTransform.position.y, 
            averageHandPosition.z
        );
        
        // 胴体の位置を頭の位置に近づけるが、最大水平距離を超えないようにする
        bodyTransform.position = Vector3.MoveTowards(
            bodyTransform.position,
            adjustedHeadPosition,
            maxHorizontalDistance
        );

        // 胴体の高さの調整
        bodyTransform.position = new Vector3(
            bodyTransform.position.x,
            Mathf.Clamp(bodyTransform.position.y, minimumBodyHeight, headTransform.position.y - bodyHeightOffset),
            bodyTransform.position.z
        );

        // 胴体の向きは頭の向きに合わせるが、X軸とZ軸の回転は適用しない
        bodyTransform.rotation = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
    }

    private void AdjustHeadPosition()
    {
        // 頭の位置を胴体の位置から遠く離れないように微調整する
        // ここでは、胴体と頭のY軸の位置を同期させます（必要に応じて調整）
        headTransform.position = new Vector3(
            headTransform.position.x, 
            bodyTransform.position.y + bodyHeightOffset,
            headTransform.position.z
        );
    }
}
