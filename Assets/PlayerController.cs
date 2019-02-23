using Layers;
using UnityEngine;

public class PlayerController : Entity
{
    public static bool IsMoving { get; private set; }

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

    void OnTilePressed(Vector2Int tilePosition)
    {
        if (CanMove(tilePosition, out var anotherEntity) && IsMoving)
        {
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
    
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        IsMoving = true;
    }

    protected override void OnMoveEnded()
    {
        base.OnMoveEnded();
        IsMoving = false;
        LayerManager.Instance.CurrentLayer?.DeselectAllTiles();
    }
}
