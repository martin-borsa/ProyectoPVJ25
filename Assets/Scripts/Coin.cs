using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public CoinType CoinType = CoinType.Zipcoin; //2
    public int ZipcoinScore;
    public int ZipcoinPlatinoScore;
    private int CoinAmount;

    public int CoinBalance => CoinAmount; //Getter

    private void OnTriggerEnter(Collider other) //5
    {
        if (other.gameObject.CompareTag("Player"))
        {
           other.gameObject.GetComponent<Player>().CoinAmount = Colect();
        }
    }

    private int GetPointsByType() //3
    {
        switch (CoinType)
        {
            case CoinType.Zipcoin:
                return ZipcoinScore;
            case CoinType.Zipcoin_Platino:
                return ZipcoinPlatinoScore;
            default: return 10;
        }
    }

    public int Colect() //4
    {
        int score = GetPointsByType();
        CoinAmount += score; //Lambda
        gameObject.SetActive(false);
        return score;
    }

    public int CoinsToUI() //6
    {
        return CoinAmount;
    }
}

public enum CoinType //1
{
    Zipcoin,
    Zipcoin_Platino,

}
