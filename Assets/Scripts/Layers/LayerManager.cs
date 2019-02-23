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

        public PlayerController cachedPlayer { get; set; }
        
        readonly Queue<ILayer> savedLayers = new Queue<ILayer>();
        Vector3 lastPosition;

        void Awake()
        {
            var layers = GetComponentsInChildren(typeof(ILayer)).Cast<ILayer>();
            foreach (var l in layers)
            {
                savedLayers.Enqueue(l);
                ((MonoBehaviour)l).gameObject.SetActive(false);
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
            
            if (CurrentLayer != null)
            {
                PreviousLayer = CurrentLayer;
            }

            var poppedLayer = savedLayers.Dequeue();
            CurrentLayer = poppedLayer;
            Tile cachedTile = null;
            
            var layerOffset = Vector2Int.zero;
            var previousLayerPlayerPos = PreviousLayer?.PlayerPosition;
            if (previousLayerPlayerPos != null)
            {
                var nextPlayerPos = CurrentLayer.GetPlayerTilePosition();
                if (nextPlayerPos != null)
                {
                    layerOffset.x = previousLayerPlayerPos.Value.x - nextPlayerPos.Value.x;
                    layerOffset.y = previousLayerPlayerPos.Value.y - nextPlayerPos.Value.y;
                    
                    cachedTile = PreviousLayer?.GetTileAtPosition(previousLayerPlayerPos.Value);
                    if (cachedTile != null)
                    {
                        cachedTile.transform.SetParent(null);
                    }
                }
            }

            var newPosition = lastPosition;
            newPosition.x += layerOffset.x;
            newPosition.z += layerOffset.y;
            lastPosition = newPosition;

            var spawnPos = newPosition;
            spawnPos.x += 5f;
            spawnPos.z += 5f;
            
            poppedLayer.OnLayerPushed(
                spawnPos, 
                newPosition,
                () =>
                {
                    if (cachedTile != null)
                    {
                        cachedPlayer.transform.SetParent(null);
                    }
                    PreviousLayer?.OnLayerPopped(() =>
                    {
                        cachedPlayer.RevokeEvents();
                        cachedPlayer.transform.SetParent(((Layer) CurrentLayer).transform);
                        
                        if (cachedTile != null)
                        {
                            cachedTile.transform.SetParent(((MonoBehaviour) CurrentLayer).transform);
                            CurrentLayer.SetTile(cachedTile.CurrentPosition, cachedTile);
                        }
                    }, layerOffset);
                    
                    if (!instant)
                    {
                        var mainCamera = CameraController.Instance.MainCamera;
                        mainCamera.DOShakePosition(.2f, 1f, 2, 160f).OnComplete(() =>
                        {
                            mainCamera.transform.SetPosition(null, mainCamera.transform.position.y + poppedLayer.Height / 2f, null);
                        });
                    }
             
                }, cachedPlayer, instant);
        }
    }
}