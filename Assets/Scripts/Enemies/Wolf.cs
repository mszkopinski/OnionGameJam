using UnityEngine;

public class Wolf : Entity
{
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        CurrentTarget = CurrentPosition + new Vector2Int(0, 1);
    }
}