using UnityEngine;

public class Golem : Entity
{
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        CurrentTarget = CurrentPosition + new Vector2Int(1, 0);
    }
}