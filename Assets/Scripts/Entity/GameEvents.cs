using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static event Action<GameObject> OnPlayerDeath;

    public static void PlayerTriggerDeath(GameObject Player)
    {
        OnPlayerDeath?.Invoke(Player);
    }
}
