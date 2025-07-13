using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugBot : Entity
{
    public override void TakeDamage(int danioRecibido)
    {
       base.TakeDamage(danioRecibido);
       Debug.Log("Bug-Bot daniado");
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("Bug-Bot eliminado");
    }

    

    
}
