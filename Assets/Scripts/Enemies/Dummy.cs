using System;

public class Dummy : Entity
{
    public override void OnMoveStarted(Action onMoveEnded)
    {
        base.OnMoveStarted(onMoveEnded);
        OnMoveEnded();
    }
}