using System;
using System.Collections;
using Layers;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsSelected { get; private set; }
    
    public TileType Type { get; private set; }
    public Vector2Int CurrentPosition => new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.z));
    public Entity CurrentEntity => LayerManager.Instance.CurrentLayer != null
        ? LayerManager.Instance.CurrentLayer.GetEntityAtPosition(CurrentPosition)
        : null;

    Action<Vector2Int> tilePressedCallback;
    Color defaultMatColor;
    Renderer tileRenderer;
    Collider collider;

    void Awake()
    {
        collider = GetComponent(typeof(Collider)) as Collider;
    }
    
    public void Initialize(Action<Vector2Int> tilePressed, TileType tileType)
    {
        tilePressedCallback = tilePressed;
        tileRenderer = GetComponentInChildren<Renderer>();
        defaultMatColor = tileRenderer.material.color;
        Type = tileType;
        if (Type == TileType.EndPoint)
        {
            tileRenderer.material.color = Color.yellow;
        }

        StartCoroutine(WaitFrameAndFuckOff());
    }
    
    IEnumerator WaitFrameAndFuckOff()
    {
        collider.enabled = false;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        collider.enabled = true;
    }
    
    void OnMouseDown()
    {
        tilePressedCallback?.Invoke(CurrentPosition);
    }

    void OnMouseOver()
    {
        if (tileRenderer == null || !LayerManager.Instance.cachedPlayer.IsMoving) return;
        var newColor = tileRenderer.material.color;
        if (CurrentEntity == null)
        {
            newColor = IsSelected ? Utils.Extensions.SelectedAndHighlightedTile : Utils.Extensions.HighlightedTile;
        }
        tileRenderer.material.color = newColor;
    }

    void OnMouseExit()
    {
        if (tileRenderer == null || !LayerManager.Instance.cachedPlayer.IsMoving) return;
        var newColor = tileRenderer.material.color;
        if (IsSelected)
        {
            if (CurrentEntity != null)
            {
                newColor = Utils.Extensions.HighlightedTile;
            }
            else
            {
                newColor = defaultMatColor;
                newColor.g += .3f;
            }
        }
        else
        {
            newColor = defaultMatColor;
        }
        tileRenderer.material.color = newColor;
    }

    public void SelectTile()
    {
        if (tileRenderer == null || !LayerManager.Instance.cachedPlayer.IsMoving) return;
        IsSelected = true;    
        var newColor = tileRenderer.material.color;
        if (CurrentEntity == null)
        {
            newColor = defaultMatColor;
            newColor.g += .3f;

        }
        else
        {
            newColor = Color.red;
        }
        tileRenderer.material.color = newColor;
    }

    public void DeselectTile()
    {
        if (tileRenderer == null) return;
        IsSelected = false;
        var newColor = tileRenderer.material.color;
        newColor = defaultMatColor;
        tileRenderer.material.color = newColor;
    }
}
