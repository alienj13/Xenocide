using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // [.] Imported from Junaid
    [Header("Cameras")]
    [SerializeField] private Camera cam1;  //camera for player assigned to P1
    [SerializeField] private Camera cam2;  //camera for player assigned to P2
    public Camera c;

    [Header("Game Controller")]
    [SerializeField] public GameController gameController;

    public XPlayer activePlayer;
    private bool active = false;

    private float speed = 20;

    private void Awake()
    {
        Camera.main.enabled = true;
        c = Camera.main;
    }

    public void SetDependencies(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void Update()
    {
        if (c != null && active)
        {
            // Left & Right (A & D)
            if (Input.GetKey(KeyCode.D))
                c.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            if (Input.GetKey(KeyCode.A))
                c.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            // Forward and backward (W & S)
            if (Input.GetKey(KeyCode.W))
            {
                if (c == cam1)
                    c.transform.Translate(0, 0, speed * Time.deltaTime, Space.World);
                else if (c == cam2)
                    c.transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (c == cam1)
                    c.transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);
                else if (c == cam2)
                    c.transform.Translate(0, 0, speed * Time.deltaTime, Space.World);
            }
            // Up and down (scroll up & down)
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                c.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime * 5));
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                c.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime * 5));
        }

        
    }

    public void SetActivePlayer(XPlayer activePlayer)
    {
        this.activePlayer = activePlayer;
    }

    // Singleplayer implementation
    public void SetActivePlayerCamera(XPlayer activePlayer)
    {
        this.activePlayer = activePlayer;
        if (activePlayer != null)
        {
            Camera.main.enabled = false;
            if (activePlayer.team == PlayerTeam.P1)
            {
                cam1.enabled = true;
                cam2.enabled = false;
                c = cam1;
            }
            else if (activePlayer.team == PlayerTeam.P2)
            {
                cam1.enabled = false;
                cam2.enabled = true;
                c = cam2;
            }
        }
    }

    // Multiplayer implementation
    public void SetLocalPlayerCamera(PlayerTeam team)
    {
        Camera.main.enabled = false;
        if (team == PlayerTeam.P1)
        {
            cam1.enabled = true;
            cam2.enabled = false;
            c = cam1;
        }
        else if (team == PlayerTeam.P2)
        {
            cam1.enabled = false;
            cam2.enabled = true;
            c = cam2;
        }
    }

    public void setActive(bool activation)
    {
        this.active = activation;
    }
}
