using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamage
{
    [SerializeField] protected int vida;
    [SerializeField] protected bool estado;
    [SerializeField] protected GameObject gameObject;

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }

    public virtual void TakeDamage(int danioRecibido)
    {
        vida -= danioRecibido;

        if (vida <= 0) Die();
    }
}
