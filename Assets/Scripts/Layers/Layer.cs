using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Layers
{
    public class Layer : MonoBehaviour, ILayer
    {
        [SerializeField] GameObject tilePrefab;
        [SerializeField] Vector2Int layerSize;
        
        public bool IsSpawned { get; private set; }
        public float Height { get; private set; }
        
        readonly List<GameObject> spawnedTiles = new List<GameObject>();

        public void OnLayerPushed()
        {
            gameObject.SetActive(false);
        }

        public void OnLayerPopped(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped, bool instant = false)
        {
            gameObject.SetActive(true);
            SpawnTiles();
            transform.DOLocalMove(spawnPoint, 0f)
                .OnComplete(() =>
                {
                    transform
                        .DOLocalMove(destinationPoint, instant ? 0f : .1f)
                        .OnComplete(() =>
                        {
                            IsSpawned = true;
                            layerPopped?.Invoke();
                        });
                });
        }

        void SpawnTiles()
        {
            for (int i = -layerSize.x / 2; i < layerSize.x / 2; ++i)
            {
                for (int j = -layerSize.y / 2; j < layerSize.y / 2; ++j)
                {
                    var newTile = Instantiate(tilePrefab, transform);
                    newTile.transform.SetPosition(i, transform.position.y, j);
                    spawnedTiles.Add(newTile);
                }
            }

            var firstTile = spawnedTiles.FirstOrDefault();
            if (firstTile == null) return;
            var boxCollider = firstTile.GetComponent<BoxCollider>();
            if (boxCollider == null) return;
            Height = boxCollider.size.y;
        }
    }

    public interface ILayer
    {
        float Height { get; }
        void OnLayerPushed();
        void OnLayerPopped(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped, bool instant = false);
    }
}

