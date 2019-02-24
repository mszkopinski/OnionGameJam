using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enemies;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Utils;

namespace Layers
{
    public class Layer : SerializedMonoBehaviour, ILayer
    {
        public event Action<Vector2Int> TilePressed;

        public Vector2Int? PlayerPosition => cachedPlayer?.CurrentPosition;

        [TableMatrix, SerializeField] TileType[,] tiles;
        [SerializeField] List<TileWithPosition> moveQueue = new List<TileWithPosition>();

        [SerializeField] GameObject tilePrefab;
        [SerializeField] int layerWidth;
        [SerializeField] int layerHeight;
        [SerializeField] int turnsPerLayer;
        
        public bool IsSpawned { get; private set; }
        public List<Entity> EnemiesMoveQueue { get; } = new List<Entity>();

        public float Height { get; private set; }
        public int Turns => turnsPerLayer;

        int elapsedTurns;
        int previousGolemsNumber, previousWolfsNumber;
        readonly List<Tile> cachedTiles = new List<Tile>();
        readonly List<Entity> cachedEntities = new List<Entity>();
        PlayerController cachedPlayer;

        void Awake()
        {
            elapsedTurns = 0;
        }
        
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
        
        public void OnLayerPopped(Action callback, Vector2Int layerOffset)
        {
            transform.DOMoveY(transform.position.y - 20f, .5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();

                var mainCamera = CameraController.Instance.MainCamera;
                var newCameraPos = mainCamera.transform.position;
                newCameraPos.x += layerOffset.x;
                newCameraPos.z += layerOffset.y;
                mainCamera.transform.DOMove(newCameraPos, 1.5f);
            });
        }

        public void OnLayerPushed(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped, PlayerController playerController = null, bool instant = false)
        {
            gameObject.SetActive(true);
            cachedPlayer = playerController;

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
                            GameManager.Instance.StartNextMove(this);
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
                        tileComponent.Initialize(OnTilePressed, tiles[x, y]);
                        cachedTiles.Add(tileComponent);
                        
                    }
                    if (entityTile != null)
                    {
                        var temp = Instantiate(entityTile);
                        if (tileType != TileType.Player)
                        {
                            temp.transform.SetParent(transform);
                        }
                        temp.transform.SetPosition(x, transform.position.y, y);
                        var entity = temp.GetComponent(typeof(Entity));
                        if (entity == null) continue;
                        if (cachedPlayer == null && entity is PlayerController playerController)
                        {
                            LayerManager.Instance.cachedPlayer = playerController;
                            cachedPlayer = LayerManager.Instance.cachedPlayer;
                        }
                        cachedEntities.Add((Entity)entity);
                    }
                }
            }
            
            EnemiesMoveQueue.Clear();
            EnemiesMoveQueue.Add(cachedPlayer);
            moveQueue.ForEach(move =>
            {
                var entity = cachedEntities.FirstOrDefault(e => e.CurrentPosition == move.Position);
                if (entity != null && !EnemiesMoveQueue.Contains(entity))
                {
                    EnemiesMoveQueue.Add(entity);
                    entity.Died += () =>
                    {
                        EnemiesMoveQueue.Remove(entity);
                        cachedEntities.Remove(entity);
                        GameManager.Instance.CheckEndConditions();
                    };
                }
            });
            
            cachedPlayer.MoveStarted += RefreshPlayerPossibleMoves;
            cachedPlayer.Pushed += RefreshPlayerPossibleMoves;
        }

        public void RefreshPlayerPossibleMoves()
        {
            DeselectAllTiles();
            var playerPos = cachedPlayer.CurrentPosition;

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
                    return new Tuple<GameObject, GameObject>(tilePrefab, null);
                case TileType.Player:
                    return new Tuple<GameObject, 
                        GameObject>(LayerManager.Instance.PreviousLayer == null ? tilePrefab : null, 
                        LayerManager.Instance.PreviousLayer == null ? GameManager.Instance.PlayerPrefab : null);
            }

            return null;
        }
        
        public void ClearEntities()
        {
            cachedEntities.Where(e => !(e is PlayerController)).ForEach(e => Destroy(e.gameObject));
        }

        public void DeselectAllTiles()
        {
            cachedTiles.ForEach(t => t.DeselectTile());
        }

        public Entity GetEntityAtPosition(Vector2Int position)
        {
            var entity = cachedEntities.Where(e => e != null).FirstOrDefault(e => e.CurrentPosition == position);
            if (cachedPlayer.CurrentPosition == position)
            {
                entity = cachedPlayer;
            }
            return entity;
        }
        
        public void SetTile(Vector2Int position, Tile tile)
        {
            tiles[position.x, position.y] = tile.Type;
            cachedTiles.Add(tile);
        }
        
        public Tile GetTileAtPosition(Vector2Int position)
        {
            return cachedTiles.Where(e => e != null).FirstOrDefault(e => e.CurrentPosition == position);
        }

        public Vector2Int? GetPlayerTilePosition()
        {
            Vector2Int? pos = null;
            tiles.ForEach((t, i) =>
            {
                if (t == TileType.Player)
                {
                    pos = i;
                }
            });
            return pos;
        }
        
        public void OnTurnEnded()
        {
            ++elapsedTurns;
        }

        public bool CheckEndConditions()
        {
            return elapsedTurns >= Turns;
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
        List<Entity> EnemiesMoveQueue { get; }
        float Height { get; }
        int Turns { get; }
        void OnLayerPopped(Action callback, Vector2Int layerOffset);
        void OnLayerPushed(Vector3 spawnPoint, Vector3 destinationPoint, Action layerPopped, PlayerController playerController, bool instant = false);
        void DeselectAllTiles();
        void SetTile(Vector2Int position, Tile tile);
        void ClearEntities();
        void RefreshPlayerPossibleMoves();
        void OnTurnEnded();
        bool CheckEndConditions();
        Vector2Int? PlayerPosition { get; }
        Entity GetEntityAtPosition(Vector2Int position);
        Tile GetTileAtPosition(Vector2Int position);
        Vector2Int? GetPlayerTilePosition();
    }
}

