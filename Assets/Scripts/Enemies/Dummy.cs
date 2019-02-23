public class Dummy : Entity
{
    public override void OnMoveStarted()
    {
        base.OnMoveStarted();
        OnMoveEnded();
    }
}