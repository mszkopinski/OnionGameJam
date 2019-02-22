using System;
using DG.Tweening;
using UnityEngine;

namespace Layers
{
    public class Layer : MonoBehaviour, ILayer
    {
        public bool IsSpawned { get; private set; }
        public float Height => boxCollider.size.y;

        BoxCollider boxCollider;

        void Awake()
        {
            boxCollider = GetComponentInChildren<BoxCollider>();
        }

        public void OnLayerPushed()
        {
            gameObject.SetActive(false);
        }

        public void OnLayerPopped(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped)
        {
            gameObject.SetActive(true);
            transform.DOLocalMove(spawnPoint, 0f)
                .OnComplete(() =>
                {
                    transform
                        .DOLocalMove(destinationPoint, .1f)
                        .OnComplete(() => { layerPopped?.Invoke(); });
                });
        }
    }

    public interface ILayer
    {
        float Height { get; }
        void OnLayerPushed();
        void OnLayerPopped(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped);
    }
}

