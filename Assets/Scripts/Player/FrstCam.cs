using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrstCam : MonoBehaviour {

    public float Speed = 30f;
    public float RotateSpeed = 40f;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        move.y = 0f;
        move += Vector3.up * Input.GetAxis("QE");
        transform.position += move.normalized * Speed * Time.deltaTime;

        Vector3 angles = transform.rotation.eulerAngles;
        angles.z = 0f;
        Vector3 desired = angles + new Vector3 (Input.GetAxis("Mouse Y") * -RotateSpeed , Input.GetAxis("Mouse X") * RotateSpeed);
        desired.z = 0f;
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(desired), Time.deltaTime);
    }

    // Update is called once per frame
    void Update () {
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
}
