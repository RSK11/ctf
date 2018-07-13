using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A third person camera for following the player
public class ThrdCam : MonoBehaviour {
    // The offset from the player's position
    public Vector3 offset = new Vector3(0,1,0);
    // the allowed range for x axis rotation
    public Vector2 TopRange = new Vector2(5f,85f);
    // the x and y axis rotation speeds
    public Vector2 Speed = new Vector2(1.5f, 1.5f);
    // The object to look at
    public GameObject target;
    // The maximum distance for the camera to use
    public float distance = 20f;
    // The minimum distance for the camera to use
    public float mindist = 6f;
    // The camera's current distance
    private float dist = 6f;
    // The current x axis rotation
    public float angleTop = 60f;
    // The current y axis rotation
    public float angleSide = 270f;

    // The original positions for resetting
    private float origTop;
    private float origSide;
    // Whether or not to invert the vertical movement
    public bool invert = false;

    void Start()
    {
        // Lock the cursor and initialize the data
        if (invert)
        {
            Speed.y = -Speed.y;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        origTop = angleTop;
        origSide = angleSide;
    }
	
	void LateUpdate () {
        // Get the mouse movement
        Vector3 delta = new Vector3(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"),0f);

        // Determine the new angles
        angleTop = Mathf.Clamp(angleTop + Speed.y * delta.y, TopRange.x, TopRange.y);
        angleSide = (angleSide - Speed.x * delta.x) % 360f;

        // Calculate distance based on x axis rotation
        dist = distance - (distance - mindist) * ((angleTop - TopRange.x) / (TopRange.y - TopRange.x));

        // Calculate the camera position
        delta.x = dist * Mathf.Cos(angleSide * Mathf.PI / 180f) * Mathf.Sin(angleTop * Mathf.PI / 180f);
        delta.z = dist * Mathf.Sin(angleSide * Mathf.PI / 180f) * Mathf.Sin(angleTop * Mathf.PI / 180f);
        delta.y = dist * Mathf.Cos(angleTop * Mathf.PI / 180f);
        transform.localPosition = offset + delta;

        // Look at the target
        transform.LookAt(target.transform.position + offset);

        // Toggle the cursor
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        // Quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    // The forward vector of the camera, ignoring the height
    public Vector3 Forward()
    {
        return Vector3.Normalize(transform.forward + new Vector3(0f, -transform.forward.y, 0f));
    }

    // The right vector of the camera, ignoring the height
    public Vector3 Right()
    {
        return Vector3.Normalize(transform.right + new Vector3(0f, -transform.right.y, 0f));
    }

    // Reset the camera to its starting position
    public void Reset()
    {
        angleTop = origTop;
        angleSide = origSide;
    }
}
