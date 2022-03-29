using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScriptQueen : MonoBehaviour
{

    int maxHP;
    float newHealth;

    public void setMaxHealthQ(int maxHP)
    {
        this.maxHP = maxHP;
        newHealth = 8;
        transform.localScale = new Vector3(0.7f,0.1f,newHealth/1);
    }

    public void setHealthQ(int health)
    {
        newHealth = (health * 1.0f) /maxHP;
        transform.localScale = new Vector3(0.7f,0.1f,(newHealth/1)*8);
    }
}
