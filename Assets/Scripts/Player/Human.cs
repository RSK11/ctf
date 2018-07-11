using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Human : AI {
    ThrdCam cam;
    public List<double> ins = new List<double>();
    private List<List<double>> doubleList = new List<List<double>>();

    // Use this for initialization
    public override void Init(int team, Material mat, CTFSim ctf)
    {
        SetupGPS();
        SetupMov();
        SetTeam(team);
        sim = ctf;
        meshBody.material = mat;
        brain = new NeuralNet(8, 5, 4, 4, .7);
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

    public NeuralNet GenSets()
    {
        List<double> inputs = new List<double>();
        foreach (List<double> dubs in doubleList)
        {
            if (dubs.Count > 5)
            {
                inputs = dubs;
            }
            else
            {
                brain.Train(inputs, dubs);
            }
        }
        return brain;
    }

    public void SetCam(ThrdCam camera)
    {
        cam = camera;
    }

    public override void Look()
    {
        FakeLook();
        List<double> dirs = new List<double>();
        dirs.Add(Input.GetAxis("Horizontal"));
        dirs.Add(Input.GetAxis("Vertical"));
        dirs.Add(1);
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
        doubleList.Add(ins);
        doubleList.Add(directions);
    }

    public void FakeLook()
    {
        ins.Clear();
        double enemyDir = 0;
        double enemyDist = sightRadius;
        double enemyHasFlag = 0;
        double haveFlag = 0;
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (Collider hit in hits)
        {
            Vector3 ray = hit.transform.position - transform.position;
            if (Vector3.Angle(cam.Forward(), ray) < sightAngle)
            {
                if (hit.CompareTag("Contestant"))
                {
                    CTFPlayer player = hit.gameObject.GetComponent<CTFPlayer>();
                    if (player.team != team && ((ray.magnitude < enemyDist && enemyHasFlag == 0) || player.flag))
                    {
                        enemyDir = Vector3.SignedAngle(cam.Forward(), ray, Vector3.up) / 180f;
                        enemyDist = ray.magnitude;
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

        ins.Add(enemyDir);
        ins.Add(enemyDist);
        ins.Add(enemyHasFlag);
        ins.Add(Vector3.SignedAngle(body.transform.forward, goalCheck - transform.position, Vector3.up) / 180f);
        ins.Add((goalCheck - transform.position).magnitude);
        ins.Add(haveFlag);
        ins.Add(Vector3.SignedAngle(body.transform.forward, sim.goals[team].transform.position - transform.position, Vector3.up) / 180f);
        ins.Add((sim.goals[team].transform.position - transform.position).magnitude);
    }

    public override Vector3 GetDirection()
    {
        Vector3 dir = cam.Forward() * (float)directions[1] + cam.Right() * (float)directions[0];

        // Ignore height and small movements
        dir.y = 0f;

        return dir.normalized;
    }
}
