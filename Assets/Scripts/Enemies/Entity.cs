using System;
using DG.Tweening;
using Layers;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float rotationSpeed = 10f;
	[SerializeField] Animator anim = null;
        
    public Vector2Int? CurrentTarget { get; set; }
    public bool IsMoving { get; protected set; }
    
    public event Action TargetReached;
    public event Action MoveStarted;
    public event Action Pushed;
    public event Action Died;

    public Vector2Int CurrentPosition => new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.z));
    public Vector2Int PreviousPosition { get; protected set; }
    public AudioClip walkClip;

    Vector3 lookDirection;
    Quaternion lookRotation;
    bool initialPositionFetched;

    bool isWalkSoundPlayed = false;

    protected void Update()
    {
        if (CurrentTarget == null) return;
        
        var targetPos = transform.localPosition;
        targetPos.x = CurrentTarget.Value.x;
        targetPos.z = CurrentTarget.Value.y;

        if (Vector3.Distance(transform.localPosition, targetPos) <= .0001f)
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

            PlayWalkSound();
        }
    }
    
    protected virtual void OnTargetReached()
    {
        TargetReached?.Invoke();
        OnMoveEnded();
        isWalkSoundPlayed = false;
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

    Action moveEndedCallback;
    
    public virtual void OnMoveStarted(Action onMoveEnded)
    {
        MoveStarted?.Invoke();
        PreviousPosition = CurrentPosition;
        IsMoving = true;
        
        LayerManager.Instance.CurrentLayer?.RefreshPlayerPossibleMoves();
        Debug.Log("STARTED MOVING " + gameObject.name);
        moveEndedCallback = onMoveEnded;
		
		if (anim != null) {
			anim.SetBool("walk", true);
			anim.SetBool("attack", false);
		}
    }

    void PlayWalkSound()
    {
        var audioSource = GetComponent<AudioSource>();

        if (audioSource != null && !isWalkSoundPlayed)
        {
            audioSource.PlayOneShot(walkClip);
            isWalkSoundPlayed = true;

        }
    }
    
    protected virtual void OnMoveEnded()
    {
        IsMoving = false;
        
        Debug.Log("ENDED MOVING " + gameObject.name);
		
		if (anim != null) {
			anim.SetBool("walk", false);
			anim.SetBool("attack", false);
		}
		
        moveEndedCallback?.Invoke();
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
		
		if (anim != null) {
			anim.SetBool("walk", false);
			anim.SetBool("attack", true);
		}
    }

    void OnEmptyPlaceReached()
    {
        IsMoving = false;
        transform.DOLocalMoveY(CurrentPosition.y - 10f, .2f).SetEase(Ease.InOutSine).SetDelay(0.6f)
            .OnComplete(() => { transform.DOScale(0f, .1f).OnComplete(OnEntityDied); });
    }
}
