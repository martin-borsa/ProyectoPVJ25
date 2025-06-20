using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int respawnDelay;
    public void RespawnPlayer(GameObject Player)
    {
        ReloadScene();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += RespawnPlayer;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= RespawnPlayer;
    }

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneWithDelay(respawnDelay));
    }

    public IEnumerator ReloadSceneWithDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(CurrentSceneIndex);
    }
}
