using Layers;
using UnityEngine;

public class PlayerController : Entity
{
    public Vector2Int CurrentTarget { get; set; }
    
    const float PlayerSpeed = 2f;
    const float RotationSpeed = 10f;
    Vector3 lookDirection;
    Quaternion lookRotation;
    bool initialPositionFetched;
    
    void Awake()
    {
        if (LayerManager.Instance.CurrentLayer != null)
        {
            LayerManager.Instance.CurrentLayer.TilePressed += OnTilePressed;
        }
    }
    
    void OnDestroy()
    {
        if (LayerManager.Instance.CurrentLayer != null)
        {
            LayerManager.Instance.CurrentLayer.TilePressed -= OnTilePressed;
        }
    }

    void Update()
    {
        if (!initialPositionFetched)
        {
            initialPositionFetched = true;
            CurrentTarget = CurrentPosition;
        }
        
        var targetPos = transform.localPosition;
        targetPos.x = CurrentTarget.x;
        targetPos.z = CurrentTarget.y;

        if (Vector3.Distance(transform.localPosition, targetPos) >= 0.005f)
        {
            HasReachedTarget = false;
            
            var step =  PlayerSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);
        
            lookDirection = (targetPos- transform.localPosition).normalized;
            lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
        }
        else
        {
            HasReachedTarget = true;
        }
    }
    
    void OnTilePressed(Vector2Int tilePosition)
    {
        if (CanMove(tilePosition))
        {
            CurrentTarget = tilePosition;
            var currentLayer = LayerManager.Instance.CurrentLayer;
            currentLayer?.DeselectAllTiles();
        }
    }
}
