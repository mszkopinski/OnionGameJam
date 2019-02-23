using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public event Action TargetReached;

    public Vector2Int CurrentPosition => new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.z));
    
    public bool HasReachedTarget
    {
        get => hasReachedTarget;
        protected set
        {
            if (value == hasReachedTarget) return;
            hasReachedTarget = value;
            OnTargetReached();
        }
    }

    bool hasReachedTarget;
    
    protected virtual void OnTargetReached()
    {
        TargetReached?.Invoke();
    }

    protected bool CanMove(Vector2Int positionToCheck)
    {
        return Mathf.Abs(CurrentPosition.x - positionToCheck.x) + Mathf.Abs(CurrentPosition.y - positionToCheck.y) == 1;  
    }
}