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
    [SerializeField] public VideoPlayer vp;
    [SerializeField] public VideoClip[] vids;

    // [] Temporary solution
    // To detect when video end
    // End me!
    private bool played = false;

    public void ShowExecutionAnimation(Unit attacker, Unit defender)
    {
        // (attacker) -> (defender)
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
            vp.Play();
            played = true;
        }
    }

    public void Update()
    {
        if (played)
        {
            if (vp.isPaused)
            {
                played = false;
                ui.HideAnimationScreen();
            }
        }
    }
}
