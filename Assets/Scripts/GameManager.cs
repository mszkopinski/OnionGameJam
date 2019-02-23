using Utils;
using TMPro;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public GameObject PlayerPrefab;
    
    public int rounds = 0;
    public TextMeshProUGUI RoundsLabel = null;

    public void Start()
    {
        if(RoundsLabel)
        {
            RoundsLabel.text = "ROUNDS: " + rounds;
        }
    }
}
