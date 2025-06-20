using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int CoinAmount;

    public void ColectCoins(int amount)
    {
        CoinAmount += amount;
    }
}
