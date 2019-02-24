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

    //This Method display win screen
    public void WinLevel ()
    {
        completeLevelUI.SetActive(true);
    }

    //This Method cause lose and restart level
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
    Layer currentLayer;

    public void StartNextMove(Layer layer)
    {
        if (CheckEndConditions())
        {
            currentLayer = null;
            Debug.Log(" NO I CHUJ");
            LayerManager.Instance.PopLayer();
            return;
        }

        if (currentLayer == null || layer != currentLayer)
        {
            currentLayer = layer;
            lastMoveIndex = -1;
        }
        
        if (currentLayer == null) return;
        
        var moveQueue = currentLayer.EnemiesMoveQueue;
        lastMoveIndex = lastMoveIndex >= moveQueue.Count - 1 ? 0 : lastMoveIndex + 1;
        
        var nextEntity = moveQueue.ElementAtOrDefault(lastMoveIndex);
        if (nextEntity == null) return;
        Debug.Log(currentLayer + " enemies queue count: " + moveQueue.Count + ". Current move " + nextEntity);
        currentMove = nextEntity;
        if (currentMove is PlayerController)
        {
            currentLayer.OnTurnEnded();
        }
        currentMove.OnMoveStarted(OnCurrentMoveEnded);
    }

    void OnCurrentMoveEnded()
    {
        if (currentLayer != null)
        {
            StartNextMove(currentLayer);
        }
    }

    public bool CheckEndConditions()
    {
        return currentLayer != null && currentLayer.CheckEndConditions();
    }

    Entity currentMove;
}
