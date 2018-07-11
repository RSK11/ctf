using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Display a team's score
public class ScoreScript : MonoBehaviour {
    public Map map;
    // The team whose score will be displayed
    public int team = 0;
    // The team's name
    public string teamName = "";

    private Text tex;

	void Start () {
        // Resize the UI Element and Get the Text Object
        tex = GetComponent<Text>();
        float wid = Mathf.Min(Mathf.Max(0, Screen.width - 540) / 740, 1);
        tex.fontSize = 20 + Mathf.RoundToInt(40 * wid);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -(30 + 60 * wid), 0f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(100 + 150 * wid, 30 + 60 * wid);
    }
	
	void Update () {
        if (map.sim.loaded)
            tex.text = teamName + ": " + map.sim.scores[team];
	}
}
