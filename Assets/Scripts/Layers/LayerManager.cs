using Utils;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Layers
{
    public class LayerManager : MonoSingleton<LayerManager>
    {
        [SerializeField] Transform spawnPoint;
        
        readonly Stack<ILayer> savedLayers = new Stack<ILayer>();
        Vector3 lastPosition;

        void Awake()
        {
            var layers = GetComponentsInChildren(typeof(ILayer)).Cast<ILayer>();
            foreach (var l in layers)
            {
                savedLayers.Push(l);
                l.OnLayerPushed();
            }
        }

        void Start()
        {
            lastPosition = transform.position;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PopLayer();
            }
        }

        public void PopLayer()
        {
            if (savedLayers.Count == 0) return;
            var poppedLayer = savedLayers.Pop();
            poppedLayer.OnLayerPopped(
                spawnPoint.position, 
                lastPosition,
                () =>
                {
                    var newPosition = lastPosition;
                    newPosition.y += poppedLayer.Height / 2f;
                    lastPosition = newPosition;
                    
                    var mainCamera = CameraController.Instance.MainCamera;
                    mainCamera.DOShakePosition(.2f, 1f, 2, 160f).OnComplete(() =>
                    {
                        mainCamera.transform.SetPosition(null, mainCamera.transform.position.y + poppedLayer.Height / 2f, null);
                    });
                });
        }
    }
}