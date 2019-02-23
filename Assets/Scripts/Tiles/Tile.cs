using System;
using Layers;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsSelected { get; private set; }
    
    public Vector2Int CurrentPosition { get; private set; }
    public Entity CurrentEntity => LayerManager.Instance.CurrentLayer != null
        ? LayerManager.Instance.CurrentLayer.GetEntityAtPosition(CurrentPosition)
        : null;

    Action<Vector2Int> tilePressedCallback;
    Color defaultMatColor;
    Renderer tileRenderer;
    
    public void Initialize(Vector2Int position, Action<Vector2Int> tilePressed)
    {
        CurrentPosition = position;
        tilePressedCallback = tilePressed;
        tileRenderer = GetComponentInChildren<Renderer>();
        defaultMatColor = tileRenderer.material.color;
    }
    
    void OnMouseDown()
    {
        tilePressedCallback?.Invoke(CurrentPosition);
    }

    void OnMouseOver()
    {
        if (tileRenderer == null) return;
        var newColor = tileRenderer.material.color;
        if (CurrentEntity == null)
        {
            newColor = IsSelected ? Utils.Extensions.SelectedAndHighlightedTile : Utils.Extensions.HighlightedTile;
        }
        tileRenderer.material.color = newColor;
    }

    void OnMouseExit()
    {
        if (tileRenderer == null || IsSelected) return;
        var newColor = tileRenderer.material.color;
        newColor = defaultMatColor;
        tileRenderer.material.color = newColor;
    }

    public void SelectTile()
    {
        if (tileRenderer == null) return;
        IsSelected = true;
        var newColor = tileRenderer.material.color;
        newColor = CurrentEntity == null ? Utils.Extensions.SelectedTile : Color.red;
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
