using Layers;
using UnityEngine;

public class PlayerController : Entity
{
    void Awake()
    {
        if (LayerManager.Instance.CurrentLayer != null)
        {
            LayerManager.Instance.CurrentLayer.TilePressed += OnTilePressed;
        }
    }
    
    void OnDestroy()
    {
        if (LayerManager.Instance.CurrentLayer != null)
        {
            LayerManager.Instance.CurrentLayer.TilePressed -= OnTilePressed;
        }
    }

    public void RevokeEvents()
    {
        LayerManager.Instance.CurrentLayer.TilePressed += OnTilePressed;
    }

    protected override void OnTargetReached()
    {
        base.OnTargetReached();
        var tile = LayerManager.Instance.CurrentLayer.GetTileAtPosition(CurrentPosition);
        if (tile != null && tile.Type == TileType.EndPoint)
        {
            LayerManager.Instance.PopLayer();
        }
    }

    void OnTilePressed(Vector2Int tilePosition)
    {
        if (CanMove(tilePosition, out var anotherEntity) && IsMoving)
        {
            IsMoving = false;
            if (!anotherEntity)
            {
                CurrentTarget = tilePosition;
            }
            else
            {
                var direction = anotherEntity.CurrentPosition - CurrentPosition;
                anotherEntity.Push(direction, OnMoveEnded);
            }
        }
    }

    protected override void OnMoveEnded()
    {
        base.OnMoveEnded();
        LayerManager.Instance.CurrentLayer?.DeselectAllTiles();
    }
}
