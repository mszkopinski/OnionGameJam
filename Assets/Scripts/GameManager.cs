using Utils;
using TMPro;


public class GameManager : MonoSingleton<GameManager>
{
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
