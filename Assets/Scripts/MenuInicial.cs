using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void Back()
    {
        SceneManager.LoadScene("MenuInicial");
    }

    public void Controles()
    {
        SceneManager.LoadScene("Controles"); 
    }

    //public void Exit()
    //{
    //    Application.Quit();
    //}
}
