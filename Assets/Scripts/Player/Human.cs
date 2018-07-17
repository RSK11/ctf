using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Human : AI {
    ThrdCam cam;
    public List<double> ins = new List<double>();
    private List<List<double>> inList = new List<List<double>>();
    private List<List<double>> expList = new List<List<double>>();
    int count = 3;
    float timer = 0f;

    // Use this for initialization
    public override void Init(int team, Material mat, CTFSim ctf)
    {
        SetupGPS();
        SetupMov();
        SetTeam(team);
        sim = ctf;
        meshBody.material = mat;
        brain = new NeuralNet(8, 3, 1, 3, .3);
        goalCheck = sim.goals[(team + 1) % 2].transform.position;
    }

    public override void InitBrain(int team, Material mat, CTFSim ctf, NeuralNet net)
    {
        SetupGPS();
        SetupMov();
        SetTeam(team);
        sim = ctf;
        meshBody.material = mat;
        brain = net;
        goalCheck = sim.goals[(team + 1) % 2].transform.position;
    }

    public NeuralNet GenSets(int times)
    {
        bool remove = false;

        for (int i = 0; i < inList.Count; i++)
        {
            remove = false;
            RoundInputVector(i, 0);
            RoundInputVector(i, 3);
            RoundInputVector(i, 6);
            for (int j = 0; j < i; j++)
            {
                if (inList[i].SequenceEqual(inList[j]))
                {
                    remove = true;
                    break;
                }
            }
            if (remove)
            {
                inList.RemoveAt(i);
                expList.RemoveAt(i);
                i--;
            }
        }

        for (int k = 0; k < times; k++)
        {
            for (int l = 0; l < inList.Count; l++)
            {
                brain.Train(inList[l], expList[l]);
            }
        }

        return brain;
    }

    private void RoundInputVector(int i, int j)
    {
        inList[i][j] = Mathf.Round((float)inList[i][j] * 100) / 100f;
        inList[i][j+1] = Mathf.Round((float)inList[i][j+1] * 100f) / 100f;
    }

    public void SetCam(ThrdCam camera)
    {
        cam = camera;
    }

    public override void Look()
    {
        FakeLook();
        List<double> dirs = new List<double>();
        dirs.Add((Vector3.SignedAngle(body.transform.forward,cam.Forward(), Vector3.up) + 180f) / 360f);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dirs.Add(1);
        }
        else
        {
            dirs.Add(0);
        }
        if (Input.GetAxis("Fire1") > .8f)
        {
            dirs.Add(1);
        }
        else
        {
            dirs.Add(0);
        }
        directions = dirs;
        timer += Time.deltaTime * 4;
        if (timer > count)
        {
            inList.Add(ins);
            expList.Add(directions);
            count++;
        }
    }

    public void FakeLook()
    {
        ins = new List<double>();
        double enemyAngle = 0;
        double enemyDist = 1;
        double enemyHasFlag = 0;
        double haveFlag = 0;
        Vector3 ray = new Vector3();
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (Collider hit in hits)
        {
            ray = hit.transform.position - transform.position;
            if (Vector3.Angle(body.transform.forward, ray) < sightAngle)
            {
                if (hit.CompareTag("Contestant"))
                {
                    CTFPlayer player = hit.gameObject.GetComponent<CTFPlayer>();
                    if (player.team != team && ((ray.magnitude / sightRadius < enemyDist && enemyHasFlag == 0) || player.flag))
                    {
                        AngDist(hit.transform.position, out enemyAngle, out enemyDist);
                        if (player.flag)
                        {
                            enemyHasFlag = 1;
                        }
                    }
                }
                else if (hit.CompareTag("Flag"))
                {
                    if (hit.gameObject.GetComponent<FlagScript>().team != team)
                    {
                        goalCheck = hit.transform.position;
                    }
                }
            }
        }

        if (flag)
        {
            haveFlag = 1;
        }

        ins.Add(enemyAngle);
        ins.Add(enemyDist);
        ins.Add(enemyHasFlag);
        AngDist(goalCheck, out enemyAngle, out enemyDist);
        ins.Add(enemyAngle);
        ins.Add(enemyDist);
        ins.Add(haveFlag);
        AngDist(sim.goals[team].transform.position, out enemyAngle, out enemyDist);
        ins.Add(enemyAngle);
        ins.Add(enemyDist);
    }
}
