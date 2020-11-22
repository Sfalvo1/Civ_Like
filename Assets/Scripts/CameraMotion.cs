using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    Vector3 oldPosition;
    HexComponent[] hexes;

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO code to click and drag
        // WASD
        // Zoom

        CheckIfCameraMoved();

    }

    private void CheckIfCameraMoved()
    {
        if(oldPosition != this.transform.position)
        {
            // Something moved the camera
            oldPosition = this.transform.position;

            // TODO: Hexmap will prob have dict for hexes later
            if (hexes == null)
            {
                hexes = GameObject.FindObjectsOfType<HexComponent>();
            }

            foreach(HexComponent hex in hexes)
            {
                hex.UpdatePosition();
            }
        }
    }

    public void PanToHex(Hex hex)
    {
        // TODO Move camera to hex
    }

}
