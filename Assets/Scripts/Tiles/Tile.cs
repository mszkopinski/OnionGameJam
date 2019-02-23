using System;
using Layers;
using UnityEditor.Experimental.UIElements.GraphView;
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
        if (tileRenderer == null || !PlayerController.IsMoving) return;
        var newColor = tileRenderer.material.color;
        if (CurrentEntity == null)
        {
            newColor = IsSelected ? Utils.Extensions.SelectedAndHighlightedTile : Utils.Extensions.HighlightedTile;
        }
        tileRenderer.material.color = newColor;
    }

    void OnMouseExit()
    {
        if (tileRenderer == null || !PlayerController.IsMoving) return;
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
        if (tileRenderer == null || !PlayerController.IsMoving) return;
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
