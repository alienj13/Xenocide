using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("User details")]
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI userRank;

    [Header("Unit details")]
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitHP;
    [SerializeField] private TextMeshProUGUI unitATK;
    [SerializeField] private TextMeshProUGUI unitDEF;

    public void UpdateUserDetails(string username, string rank)
    {
        this.username.SetText(username);
        this.userRank.SetText(rank);
    }

    public void UpdateUnitDetails(Unit unit)
    {
        unitName.SetText(unit.GetType().ToString());
        unitHP.SetText("HP: " + unit.getHP().ToString());
        unitATK.SetText("ATK: " + unit.getATK().ToString());
        unitDEF.SetText("DEF: " + unit.getDEF().ToString());
    }
}
