using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplaySlider : MonoBehaviour {
    public string txt = ":";
    public string txtAfter = "";
    public Slider slide;
    public float scalar = 1f;
    private Text textUI;

    public void Awake()
    {
        textUI = GetComponent<Text>();
        SliderChanged();
    }

    public void SliderChanged()
    {
        textUI.text = txt + (Mathf.Round(10 *scalar * slide.value) / 10f) + txtAfter;
    }
}
