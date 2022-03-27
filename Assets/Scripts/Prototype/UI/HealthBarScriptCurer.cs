using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScriptCurer : MonoBehaviour
{

    //public Vector3 slider;
    //public Gradient gradient;
    //public Image fill;

    int maxHP;
    float newHealth;

    public void setMaxHealthC(int maxHP)
    {
        this.maxHP = maxHP;
        newHealth = 5.0f;
        transform.localScale = new Vector3(newHealth/1,0.001f,0.65f);
        //fill.colour = gradient.Evaluate(if);
    }

    public void setHealthC(int health)
    {
        newHealth = (health * 1.0f) /maxHP;
        //Debug.Log("HP: " + health + ", maxHP: " + maxHP + ", ratio: " + newHealth);
        transform.localScale = new Vector3((newHealth/1)*5,0.001f,0.65f);
        //fill.colour = gradient.Evaluate(slider.normalizedValue);
    }
}
