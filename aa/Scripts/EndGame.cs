using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    private bool hasGameEnded = false;
    [SerializeField] RotatorScript rotator;
    [SerializeField] PinSpawner pinSpawner;
    [SerializeField] Animator animator;

    public void StopGame()
    {
        if (hasGameEnded)
            return;

        rotator.enabled = false;
        pinSpawner.enabled = false;
        animator.SetTrigger("EndGame");
        ScoreScript.currentScore -= 1;
        hasGameEnded = true;      
    }

    public void RestartLevel()
    {        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        
    }

    public void LevelRestartDelay()
    {
        Invoke("RestartLevel", 3);
    }
}
