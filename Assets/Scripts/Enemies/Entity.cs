using System;
using DG.Tweening;
using Layers;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float rotationSpeed = 10f;
        
    public Vector2Int? CurrentTarget { get; set; }
    public static bool IsMoving { get; protected set; }
    
    public event Action TargetReached;
    public event Action MoveEnded;
    public event Action MoveStarted;
    public event Action Pushed;
    public event Action Died;

    public Vector2Int CurrentPosition => new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.z));
    public Vector2Int PreviousPosition { get; protected set; }
    
    Vector3 lookDirection;
    Quaternion lookRotation;
    bool initialPositionFetched;

    protected void Update()
    {
        if (CurrentTarget == null) return;
        
        var targetPos = transform.localPosition;
        targetPos.x = CurrentTarget.Value.x;
        targetPos.z = CurrentTarget.Value.y;

        if (Vector3.Distance(transform.localPosition, targetPos) <= .1f)
        {
            OnTargetReached();
            var currentTile = LayerManager.Instance.CurrentLayer?.GetTileAtPosition(CurrentPosition);
            var entityNextToThis = LayerManager.Instance.CurrentLayer?.GetEntityAtPosition(CurrentPosition);            
            if (currentTile == null)
            {
                OnEmptyPlaceReached();
            }

            if (entityNextToThis != null && !ReferenceEquals(entityNextToThis, this))
            {
                var direction = entityNextToThis.CurrentPosition - PreviousPosition;
                entityNextToThis.Push(direction);
            }

            CurrentTarget = null;
        }
        else
        {
            var step =  moveSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);
        
            lookDirection = (targetPos- transform.localPosition).normalized;
            lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    
    protected virtual void OnTargetReached()
    {
        TargetReached?.Invoke();
        OnMoveEnded();
    }

    protected virtual void OnPushed()
    {
        Pushed?.Invoke();
        var currentTile = LayerManager.Instance.CurrentLayer?.GetTileAtPosition(CurrentPosition);
        if (currentTile == null)
        {
            OnEmptyPlaceReached();
        }
    }

    public virtual void OnMoveStarted()
    {
        IsMoving = true;
        MoveStarted?.Invoke();
        PreviousPosition = CurrentPosition;
    }
    
    protected virtual void OnMoveEnded()
    {
        IsMoving = false;
        MoveEnded?.Invoke();
    }

    protected virtual void OnEntityDied()
    {
        Died?.Invoke();
        OnMoveEnded();
    }

    protected bool CanMove(Vector2Int positionToCheck, out Entity entity)
    {
        entity = null;
        var currentLayer = LayerManager.Instance.CurrentLayer;
        if (currentLayer != null)
        {
            entity = currentLayer.GetEntityAtPosition(positionToCheck);
        }
        return Mathf.Abs(CurrentPosition.x - positionToCheck.x) + Mathf.Abs(CurrentPosition.y - positionToCheck.y) == 1;  
    }

    public void Push(Vector2Int direction, Action pushedCallback = null)
    {
        var pushTime = .3f;
        transform.DOLocalMoveX(CurrentPosition.x + direction.x, pushTime);
        transform.DOLocalMoveZ(CurrentPosition.y + direction.y, pushTime).OnComplete(() =>
        {
            OnPushed();
            pushedCallback?.Invoke();
            var currentTile = LayerManager.Instance.CurrentLayer?.GetTileAtPosition(CurrentPosition);
            if (currentTile != null)
            {
                var nextEntity = LayerManager.Instance.CurrentLayer?.GetEntityAtPosition(CurrentPosition);
                if (nextEntity == null || ReferenceEquals(this, nextEntity)) return;
                var dir = nextEntity.CurrentPosition - (CurrentPosition - direction);
                nextEntity.Push(dir);
            }
            else
            {
                OnEmptyPlaceReached();
            }
        });
    }

    void OnEmptyPlaceReached()
    {
        IsMoving = false;
        transform.DOLocalMoveY(CurrentPosition.y - 10f, .2f).SetEase(Ease.InOutSine).SetDelay(0.6f)
            .OnComplete(() => { transform.DOScale(0f, .1f).OnComplete(OnEntityDied); });
    }
}