using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Neuron {
    public double errorGrad = 0f;
    public List<double> weights = new List<double>();
    public double bias;

    public Neuron(int ins)
    {
        bias = Random.Range(-1f, 1f);
        for (int i = 0; i < ins; i++)
        {
            weights.Add(Random.Range(-1f, 1f));
        }
    }

    public void Write(StreamWriter sw)
    {
        foreach (double weight in weights)
        {
            sw.Write(weight + ",");
        }
        sw.WriteLine(bias + "," + errorGrad);
    }
}
