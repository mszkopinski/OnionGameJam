using UnityEngine;

public class Golem : Entity
{
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        if (IsMoving == false) return;
        CurrentTarget = CurrentPosition + new Vector2Int(1, 0);
    }
}