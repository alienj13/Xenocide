using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    //public Vector3 slider;
    //public Gradient gradient;
    //public Image fill;

    float newHealth;

    public void setMaxHealth(int health)
    {
        newHealth = health/health;
        transform.localScale = new Vector3(newHealth/10,0.025f,0.025f);
        //fill.colour = gradient.Evaluate(if);
    }

    public void setHealth(int health)
    {
        float newHealthU = health/newHealth;
        transform.localScale = new Vector3(newHealthU/1000,0.025f,0.025f);
        //fill.colour = gradient.Evaluate(slider.normalizedValue);
    }
}
