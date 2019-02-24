using System;
using UnityEngine;
using Layers;

public class Wolf : Entity
{
    public override void OnMoveStarted(Action onMoveEnded)
    {
        base.OnMoveStarted(onMoveEnded);
        if (IsMoving == false) return;
        var currentTile = LayerManager.Instance.CurrentLayer?.GetTileAtPosition(CurrentPosition);
        if (currentTile == null)
        {
	        return;
        }        
        
        //CurrentTarget = CurrentPosition + new Vector2Int(0, 1);
        if (CheckIfPlacerIsClose() != new Vector2Int(0, 0)) {
        	Debug.Log("0");
        	CurrentTarget = CheckIfPlacerIsClose();
        } else if (PlayerPos.x > CurrentPosition.x) {
        	Debug.Log("1");
        	CurrentTarget = CurrentPosition + new Vector2Int(1, 0);
        } else if (PlayerPos.x < CurrentPosition.x) {
        	Debug.Log("2");
        	CurrentTarget = CurrentPosition + new Vector2Int(-1, 0);
        } else if (PlayerPos.y > CurrentPosition.y){
        	Debug.Log("3");
        	CurrentTarget = CurrentPosition + new Vector2Int(0, 1);
        } else if (PlayerPos.y < CurrentPosition.y){
        	Debug.Log("4");
        	CurrentTarget = CurrentPosition + new Vector2Int(0, -1);
        }
    }


    private Vector2Int PlayerPos {
    	get {
    		return LayerManager.Instance.cachedPlayer.CurrentPosition;
    	}
    }

    private Vector2Int CheckIfPlacerIsClose() {
    	if (PlayerPos == CurrentPosition + new Vector2Int(1, 0)) {
    		return CurrentPosition + new Vector2Int(1, 0);
    	}
    	if (PlayerPos == CurrentPosition + new Vector2Int(-1, 0)) {
    		return CurrentPosition + new Vector2Int(1, 0);
    	}
    	if (PlayerPos == CurrentPosition + new Vector2Int(0, 1)) {
    		return CurrentPosition + new Vector2Int(0, 1);
    	}
    	if (PlayerPos == CurrentPosition + new Vector2Int(0, -1)) {
    		return CurrentPosition + new Vector2Int(0, -1);
    	}
    	return new Vector2Int(0, 0);
    }
}