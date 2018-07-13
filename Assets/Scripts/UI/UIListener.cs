using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIListener : MonoBehaviour {

    public InputField brain;
    public Slider participants;
    public Slider time;
    public Toggle player;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        CTFSim world = FindObjectOfType<CTFSim>();
        if (world != null)
        {
            world.Init(Mathf.RoundToInt(participants.value), Mathf.Round(Mathf.Round(10 * time.value) * 6), brain.text, player.isOn);
            world.StartGame();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
