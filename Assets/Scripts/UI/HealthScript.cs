using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays a player's health on screen
public class HealthScript : MonoBehaviour
{
    // The player to display the health of
    public CTFPlayer player;

    private Text tex;

    void Start()
    {
        // Position the UI Element and get the Text Object
        tex = GetComponent<Text>();
        float wid = Mathf.Min(Mathf.Max(0, Screen.width - 540) / 740, 1);
        tex.fontSize = 20 + Mathf.RoundToInt(30 * wid);
        GetComponent<RectTransform>().sizeDelta = new Vector2(130 + 150 * wid, 30 + 60 * wid);
        tex.text = "";
    }

    void Update()
    {
        if (player != null)
        {
            // Display the health
            tex.text = "Health: " + player.health;
        }
    }

    public void SetPlayer(CTFPlayer target)
    {
        player = target;
    }
}
