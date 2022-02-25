using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class AnimationUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] public UIManager ui;

    [Header("Videos")]
    [SerializeField] public RawImage display;
    [SerializeField] public VideoPlayer vp;
    [SerializeField] public VideoClip[] vids;

    // [] Temporary solution
    // To detect when video end
    // End me!
    private bool played = false;

    public void ShowExecutionAnimation(Unit attacker, Unit defender)
    {
        // (attacker) -> (defender)
        HideDisplays();
        vp.clip = null;
        played = false;

        // Drone -> Drone
        if (attacker is XDrone && defender is XDrone)
        {
            // Black -> Red
            if (attacker.Team == PlayerTeam.P2 && defender.Team == PlayerTeam.P1)
                vp.clip = vids[0];
            if (attacker.Team == PlayerTeam.P1 && defender.Team == PlayerTeam.P2)
                vp.clip = vids[1];
        }

        // Final check
        if (vp.clip != null)
        {
            ShowDisplays();
            vp.Play();
            played = true;
        }
        else {
            ui.HideAnimationScreen();
        }
    }

    public void Update()
    {
        if (played)
        {
            if (vp.isPaused)
            {
                played = false;
                HideDisplays();
                ui.HideAnimationScreen();
            }
        }
    }

    public void ShowDisplays()
    {
        display.gameObject.SetActive(true);
        vp.gameObject.SetActive(true);
    }

    public void HideDisplays()
    {
        display.gameObject.SetActive(false);
        vp.gameObject.SetActive(false);
    }
}
