using UnityEngine;

public class EyeGazeControl : MonoBehaviour
{
    public GameObject eyeGazeObject;

    private void Update()
    {
        // コントローラーのグリップボタンが押されているかをチェック
        if (Input.GetButton("GripButton"))
        {
            eyeGazeObject.SetActive(true); // ボタンが押されていればeyeGazeObjectを表示
        }
        else
        {
            eyeGazeObject.SetActive(false); // ボタンが押されていなければeyeGazeObjectを非表示
        }
    }
}
