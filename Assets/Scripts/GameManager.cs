﻿using System.Linq;
using Layers;
using Utils;
using TMPro;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public GameObject PlayerPrefab;
    
    public int rounds = 0;
    public TextMeshProUGUI RoundsLabel = null;

    public GameObject completeLevelUI;

    public void Start()
    {
        if(RoundsLabel)
        {
            RoundsLabel.text = "ROUNDS: " + rounds;
        }
        if(!completeLevelUI)
        {
            completeLevelUI = null;
        }
    }

    public void CompleteLevel ()
    {
        completeLevelUI.SetActive(true);
    }

    int lastMoveIndex = -1;

    public void StartNextMove()
    {
        CheckEndConditions();
        var currentLayer = LayerManager.Instance.CurrentLayer;
        if (currentLayer != null)
        {
            var moveQueue = currentLayer.EnemiesMoveQueue;
            lastMoveIndex = lastMoveIndex >= moveQueue.Count - 1 ? 0 : lastMoveIndex + 1;
            var nextEntity = moveQueue.ElementAtOrDefault(lastMoveIndex);
            if (nextEntity == null) return;
            if (CurrentMove != null)
            {
                CurrentMove.MoveEnded -= StartNextMove;
            }
            CurrentMove = nextEntity;
            CurrentMove.MoveEnded += StartNextMove;
            CurrentMove.OnMoveStarted();
        }
    }

    public void CheckEndConditions()
    {
        var currentLayer = LayerManager.Instance.CurrentLayer;
        if (currentLayer != null)
        {
            var moveQueue = currentLayer.EnemiesMoveQueue;
            if (moveQueue.Count <= 1)
            {
                LayerManager.Instance.PopLayer();
            }
        }
    }
    
    public Entity CurrentMove
    {
        get => currentMove;
        set
        {
            if (ReferenceEquals(value, currentMove)) return;
            currentMove = value;
            currentMove.OnMoveStarted();
        }
    }

    Entity currentMove;
}
