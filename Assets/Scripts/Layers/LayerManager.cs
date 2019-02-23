using Utils;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Layers
{
    public class LayerManager : MonoSingleton<LayerManager>
    {
        public ILayer CurrentLayer { get; private set; }
        
        [SerializeField] Transform spawnPoint;
        [SerializeField] Vector2 layerOffset = new Vector2(0f, .5f);
        
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
            PopLayer();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PopLayer();
            }
        }

        void PopLayer(bool instant = false)
        {
            if (savedLayers.Count == 0) return;
            var poppedLayer = savedLayers.Pop();
            CurrentLayer = poppedLayer;
            poppedLayer.OnLayerPopped(
                spawnPoint.position, 
                lastPosition,
                () =>
                {
                    var newPosition = lastPosition;
                    newPosition.x += layerOffset.x;
                    newPosition.y += poppedLayer.Height / 2f + layerOffset.y;
                    lastPosition = newPosition;

                    if (!instant)
                    {
                        var mainCamera = CameraController.Instance.MainCamera;
                        mainCamera.DOShakePosition(.2f, 1f, 2, 160f).OnComplete(() =>
                        {
                            mainCamera.transform.SetPosition(null, mainCamera.transform.position.y + poppedLayer.Height / 2f, null);
                        });
                    }
             
                }, instant);
        }
    }
}