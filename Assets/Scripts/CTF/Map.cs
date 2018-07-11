using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public CTFSim sim;
    public List<GoalScript> goals;
    public ResultScript rs;

    // Use this for initialization
    void Start () {
        sim = FindObjectOfType<CTFSim>();

        if (sim == null)
        {
            sim = new CTFSim();
            sim.Init(3, 2f, "", true);
        }

        sim.LoadMap(this);
	}
}
