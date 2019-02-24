using System;
using Layers;
using UnityEngine;

public class Golem : Entity
{
    public override void OnMoveStarted(Action onMoveEnded)
    {
        base.OnMoveStarted(onMoveEnded);
        if (IsMoving == false) return;
        var currentTile = LayerManager.Instance.CurrentLayer?.GetTileAtPosition(CurrentPosition);
        if (currentTile == null)
        {
            return;
        }        

        CurrentTarget = CurrentPosition + new Vector2Int(1, 0);
    }
}