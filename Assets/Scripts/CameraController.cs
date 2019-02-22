using UnityEngine;
using Utils;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoSingleton<CameraController>
{
    public Camera MainCamera { get; private set; }
    
    void Awake()
    {
        MainCamera = GetComponentInChildren<Camera>();
    }

    public void SetParent(Transform newParent)
    {
        var t = MainCamera.transform;
        t.SetParent(newParent);
        t.SetPosition(0f, 0f, 0f);
    }
}
