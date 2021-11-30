using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // [.] Imported from Junaid
    [SerializeField] private Camera cam1;  //camera for player assigned to P1
    [SerializeField] private Camera cam2;  //camera for player assigned to P2
    public Camera c;

    public XPlayer activePlayer;

    private float speed = 20;

    private void Update()
    {
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
        else
        {
            Camera.main.enabled = true;
            c = Camera.main;
        }

        if (Input.GetKey(KeyCode.D))
        {
            c.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            c.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            c.transform.Translate(new Vector3(0, -0.15f, -speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.W))
        {
            c.transform.Translate(new Vector3(0, 0.15f, speed * Time.deltaTime));
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            c.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime * 5));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            c.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime * 5));
        }
    }

    public void SetActivePlayer(XPlayer activePlayer)
    {
        this.activePlayer = activePlayer;
    }
}
