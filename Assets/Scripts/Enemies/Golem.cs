using System;
using UnityEngine;

public class Golem : Entity
{
    public override void OnMoveStarted(Action onMoveEnded)
    {
        base.OnMoveStarted(onMoveEnded);
        if (IsMoving == false) return;
        CurrentTarget = CurrentPosition + new Vector2Int(1, 0);
    }
}