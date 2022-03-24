using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    //public Vector3 slider;
    //public Gradient gradient;
    //public Image fill;

    int maxHP;
    float newHealth;

    public void setMaxHealth(int maxHP)
    {
        this.maxHP = maxHP;
        newHealth = 1.0f;
        transform.localScale = new Vector3(newHealth/10,0.025f,0.025f);
        //fill.colour = gradient.Evaluate(if);
    }

    public void setHealth(int health)
    {
        newHealth = (health * 1.0f) /maxHP;
        //Debug.Log("HP: " + health + ", maxHP: " + maxHP + ", ratio: " + newHealth);
        transform.localScale = new Vector3(newHealth/10,0.025f,0.025f);
        //fill.colour = gradient.Evaluate(slider.normalizedValue);
    }
}
