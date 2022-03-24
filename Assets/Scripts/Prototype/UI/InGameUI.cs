using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
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

    [Header("Videos")]
    [SerializeField] public VideoClip[] vids;
    [SerializeField] public VideoPlayer vp;
    [SerializeField] public Texture[] images;
    [SerializeField] public RawImage display;

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

        display.texture = images[0];
        if (unit is XDrone && unit.Team == PlayerTeam.P1)
        {
            vp.clip = vids[0];
            vp.Play();
        }
        else if (unit is XDrone && unit.Team == PlayerTeam.P2)
        {
            vp.clip = vids[1];
            vp.Play();
        }
        else if (unit is XWarrior && unit.Team == PlayerTeam.P1)
        {
            vp.clip = vids[2];
            vp.Play();
        }
        else if (unit is XWarrior && unit.Team == PlayerTeam.P2)
        {
            vp.clip = vids[3];
            vp.Play();
        }
        else if (unit is Destroyer && unit.Team == PlayerTeam.P1)
        {
            vp.clip = vids[4];
            vp.Play();
        }
        else if (unit is Destroyer && unit.Team == PlayerTeam.P2)
        {
            vp.clip = vids[5];
            vp.Play();
        }
        else if (unit is Curer && unit.Team == PlayerTeam.P1)
        {
            vp.clip = vids[6];
            vp.Play();
        }
        else if (unit is Curer && unit.Team == PlayerTeam.P2)
        {
            vp.clip = vids[7];
            vp.Play();
        }
        else if (unit.Team == PlayerTeam.P1)
        {
            display.texture = images[1];
        }
        else if (unit.Team == PlayerTeam.P2)
        {
            display.texture = images[2];
        }
    }
}
