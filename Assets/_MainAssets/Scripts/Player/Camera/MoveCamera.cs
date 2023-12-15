using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the camera to the player's camera holder.
public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
