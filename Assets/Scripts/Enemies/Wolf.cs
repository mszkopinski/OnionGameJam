using UnityEngine;

public class Wolf : Entity
{
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        if (IsMoving == false) return;
        CurrentTarget = CurrentPosition + new Vector2Int(0, 1);
    }
}