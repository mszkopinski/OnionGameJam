using UnityEngine;
using Utils;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoSingleton<CameraController>
{
    public Camera MainCamera { get; private set; }
    
    void Awake()
    {
        MainCamera = GetComponent<Camera>();
    }
}
