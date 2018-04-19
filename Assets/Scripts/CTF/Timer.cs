using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays the game timer on the screen
public class Timer : MonoBehaviour {
    Text tex;

	void Start () {
        // Resize the UI Element and Get the Text Object
        tex = GetComponent<Text>();
        float wid = Mathf.Min(Mathf.Max(0, Screen.width - 540) / 740,1);
        tex.fontSize = 20 + Mathf.RoundToInt(40 * wid);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -(30 + 60 * wid), 0f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(100 + 150 * wid, 30 + 60 * wid);
    }
	
	void Update () {
        // Display the timer from the Game Script
        tex.text = CTFScript.CurrentTime();
	}
}
