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
        public ILayer PreviousLayer { get; private set; }
        
        [SerializeField] Transform spawnPoint;
        
        readonly Queue<ILayer> savedLayers = new Queue<ILayer>();
        Vector3 lastPosition;

        void Awake()
        {
            var layers = GetComponentsInChildren(typeof(ILayer)).Cast<ILayer>();
            foreach (var l in layers)
            {
                savedLayers.Enqueue(l);
                l.OnLayerPopped();
            }
        }

        void Start()
        {
            lastPosition = transform.position;
            PopLayer();
        }

        public void PopLayer(bool instant = false)
        {
            if (savedLayers.Count == 0) return;
            var poppedLayer = savedLayers.Dequeue();
            if (CurrentLayer != null)
            {
                PreviousLayer = CurrentLayer;
            }
            CurrentLayer = poppedLayer;
            PreviousLayer?.OnLayerPopped();
            poppedLayer.OnLayerPushed(
                spawnPoint.position, 
                lastPosition,
                () =>
                {
                    Vector2Int layerOffset = Vector2Int.zero;

                    var previousLayerPlayerPos = PreviousLayer?.PlayerPosition;
                    if (previousLayerPlayerPos != null)
                    {
                        var nextPlayerPos = CurrentLayer.PlayerPosition;
                        layerOffset.x = nextPlayerPos.Value.x - previousLayerPlayerPos.Value.x;
                        layerOffset.y = nextPlayerPos.Value.y - previousLayerPlayerPos.Value.y;
                    }

                    var newPosition = lastPosition;
                    newPosition.x += layerOffset.x;
                    newPosition.z += layerOffset.y;
                    lastPosition = newPosition;

                    if (!instant)
                    {
                        var mainCamera = CameraController.Instance.MainCamera;
                        mainCamera.DOShakePosition(.2f, 1f, 2, 160f).OnComplete(() =>
                        {
                            mainCamera.transform.SetPosition(null, mainCamera.transform.position.y + poppedLayer.Height / 2f, null);
                            PreviousLayer.Destroy();
                        });
                    }
             
                }, instant);
        }
    }
}