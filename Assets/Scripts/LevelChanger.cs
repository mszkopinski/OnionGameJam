using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;
    private int levelToLoad;
    // Start is called before the first frame update
    void Start()
    {
       // animator = GetComponents<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeToNextLevel()
    {
        FadeToLevel(1);
    }

    public void FadeToLevel (int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("Fade_Out");
    }

    public void OnFadeComplete ()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
