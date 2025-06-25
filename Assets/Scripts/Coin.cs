using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public CoinType CoinType; //2
    public int ZipcoinScore;
    public int ZipcoinPlatinoScore;
    private int CoinAmount;

    


    public static readonly Dictionary<CoinType, int> Coins = new Dictionary<CoinType, int>() //7
    {
        {CoinType.Zipcoin, 1}, {CoinType.Zipcoin_Platino, 10}
    };


    public int CoinBalance => CoinAmount; //Getter

    //Para Crear región Ctrl + K + S
    
    #region RECOLECCIÓN DE MONEDAS
    private void OnTriggerEnter(Collider other) //5
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().CoinAmount += Colect();
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
        int score = GetCoinValue();
        CoinAmount += score; //Lambda
        gameObject.SetActive(false);
        return score;
    }

    public int GetCoinValue() //8
    {
        if (Coins.ContainsKey(CoinType))
        {
            return Coins[CoinType];
        }
        else
        {
            return 0;
        }
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
#endregion
