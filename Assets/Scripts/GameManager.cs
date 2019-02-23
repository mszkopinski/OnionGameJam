using System.Linq;
using Layers;
using Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public GameObject PlayerPrefab;
    
    public int rounds = 0;
    public TextMeshProUGUI RoundsLabel = null;

    public GameObject completeLevelUI;

    bool gameHasEnded = false;


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

    public void WinLevel ()
    {
        completeLevelUI.SetActive(true);
    }

    public void LoseLevel()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Invoke("Restart", 2f);
            
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    int lastMoveIndex = -1;

    public void StartNextMove()
    {
        if (CheckEndConditions())
            return;
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

    public bool CheckEndConditions()
    {
        var currentLayer = LayerManager.Instance.CurrentLayer;
        if (currentLayer != null)
        {
            var moveQueue = currentLayer.EnemiesMoveQueue;
            if (moveQueue.Count <= 1)
            {
                lastMoveIndex = 0;
                LayerManager.Instance.PopLayer();
                return true;
            }
        }

        return false;
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
