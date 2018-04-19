using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays the result of the previous game
public class ResultScript : MonoBehaviour
{
    private Text tex;

    void Start()
    {
        // Resize the UI Element and get the Text Object
        tex = GetComponent<Text>();
        float wid = Mathf.Min(Mathf.Max(0, Screen.width - 540) / 740, 1);
        tex.fontSize = 20 + Mathf.RoundToInt(40 * wid);
        GetComponent<RectTransform>().sizeDelta = new Vector2(100 + 150 * wid, 30 + 60 * wid);
        tex.text = "";
    }

    // Display the game results
    public void PublishResult(string winner)
    {
        tex.text = winner;
    }

    // Hide the game results
    public void EndResult()
    {
        tex.text = "";
    }
}