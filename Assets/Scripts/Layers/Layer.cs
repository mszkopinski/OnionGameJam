using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Layers
{
    public class Layer : SerializedMonoBehaviour, ILayer
    {
        public event Action<Vector2Int> TilePressed; 
        
        [TableMatrix, SerializeField] TileType[,] tiles;
        [SerializeField] List<TileWithPosition> moveQueue = new List<TileWithPosition>();

        [SerializeField] GameObject tilePrefab;
        [SerializeField] int layerWidth;
        [SerializeField] int layerHeight;
        
        public bool IsSpawned { get; private set; }
        public float Height { get; private set; }

        int previousGolemsNumber, previousWolfsNumber;
        PlayerController cachedPlayer;
        readonly List<Tile> cachedTiles = new List<Tile>();
        readonly List<Entity> cachedEntities = new List<Entity>();
        
        void OnValidate()
        {
            if (tiles == null || layerWidth != tiles.GetLength(0) || layerHeight != tiles.GetLength(1))
            {
                tiles = new TileType[layerWidth, layerHeight];
            }

            var golemsNumber = tiles.Count(t => t == TileType.Golem);
            var wolfsNumber = tiles.Count(t => t == TileType.Wolf);
            if (wolfsNumber != previousWolfsNumber || golemsNumber != previousGolemsNumber)
            {
                previousWolfsNumber = wolfsNumber;
                previousGolemsNumber = golemsNumber;
                moveQueue.Clear();
                tiles.ForEach((t, i) =>
                {
                    if (t == TileType.Wolf || t == TileType.Golem)
                    {
                        moveQueue.Add(new TileWithPosition(t, i));
                    }
                });
            }
        }
        
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
            int width = tiles.GetLength(0), height = tiles.GetLength(1);
            
            for (var x = 0; x < width; ++x) 
            {
                for (var y = 0; y < height; ++y)
                {
                    var tileType = tiles[x, y];
                    if (tileType == TileType.EmptyTile) continue;
                    var (groundTile, entityTile) = GetTileByType(tileType);
                    if (groundTile != null)
                    {
                        var temp = Instantiate(groundTile, transform);
                        temp.transform.SetPosition(x, transform.position.y, y);
                        var tileComponent = temp.AddComponent<Tile>();
                        tileComponent.Initialize(new Vector2Int(x, y), OnTilePressed);
                        cachedTiles.Add(tileComponent);
                        
                    }
                    if (entityTile != null)
                    {
                        var temp = Instantiate(entityTile, transform);
                        temp.transform.SetPosition(x, transform.position.y, y);
                        var entity = temp.GetComponent(typeof(Entity));
                        if (entity == null) continue;
                        cachedEntities.Add((Entity)entity);
                    }
                }
            }
            
            cachedPlayer = cachedPlayer != null
                ? cachedPlayer
                : cachedEntities.FirstOrDefault(e => e is PlayerController) as PlayerController;
            if (cachedPlayer == null) return;
            cachedPlayer.TargetReached += RefreshPlayerPossibleMoves;
            RefreshPlayerPossibleMoves();
        }

        void RefreshPlayerPossibleMoves()
        {
            var playerPos = cachedPlayer.CurrentPosition;
            DeselectAllTiles();

            var leftTile = cachedTiles.FirstOrDefault(t =>
                t.CurrentPosition.x == playerPos.x - 1 && t.CurrentPosition.y == playerPos.y);
            if (leftTile != null)
            {
                leftTile.SelectTile();
            }

            var rightTile = cachedTiles.FirstOrDefault(t =>
                t.CurrentPosition.x == playerPos.x + 1 && t.CurrentPosition.y == playerPos.y);
            if (rightTile != null)
            {
                rightTile.SelectTile();
            }
            
            var upperTile = cachedTiles.FirstOrDefault(t =>
                t.CurrentPosition.x == playerPos.x && t.CurrentPosition.y == playerPos.y + 1);
            if (upperTile != null)
            {
                upperTile.SelectTile();
            }
            
            var lowerTile = cachedTiles.FirstOrDefault(t =>
                t.CurrentPosition.x == playerPos.x && t.CurrentPosition.y == playerPos.y - 1);
            if (lowerTile != null)
            {
                lowerTile.SelectTile();
            }
        }

        Tuple<GameObject, GameObject> GetTileByType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Wolf: case TileType.Dummy: case TileType.Golem:
                    return new Tuple<GameObject, GameObject>(tilePrefab, EnemiesConfig.Instance.GetEnemyPrefab(tileType));
                case TileType.SolidTile:
                    return new Tuple<GameObject, GameObject>(tilePrefab, null);
                case TileType.EndPoint:
                    return new Tuple<GameObject, GameObject>(null, null);
                case TileType.Player:
                    return new Tuple<GameObject, GameObject>(tilePrefab, GameManager.Instance.PlayerPrefab);
            }

            return null;
        }

        public void DeselectAllTiles()
        {
            cachedTiles.ForEach(t => t.DeselectTile());
        }

        public Entity GetEntityAtPosition(Vector2Int position)
        {
            return cachedEntities.Where(e => e != null).FirstOrDefault(e => e.CurrentPosition == position);
        }

        protected virtual void OnTilePressed(Vector2Int tilePosition)
        {
            TilePressed?.Invoke(tilePosition);
        }
    }

    [Serializable]
    public class TileWithPosition
    {
        [ReadOnly]
        public readonly TileType Type;
        [ReadOnly]
        public readonly Vector2Int Position;

        public TileWithPosition(TileType type, Vector2Int position)
        {
            Type = type;
            Position = position;
        }
    }

    public interface ILayer
    {
        event Action<Vector2Int> TilePressed;
        float Height { get; }
        void OnLayerPushed();
        void OnLayerPopped(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped, bool instant = false);
        void DeselectAllTiles();
        Entity GetEntityAtPosition(Vector2Int position);
    }
}

