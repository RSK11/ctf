using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class Layer {
    public int neuronCount;
    public int inputCount;
    public List<Neuron> neurons = new List<Neuron>();
    public List<double> inputs;
    public List<double> outputs = new List<double>();

    public Layer(int ins, int count)
    {
        inputCount = ins;
        neuronCount = count;
        for (int i = 0; i < count; i++)
        {
            neurons.Add(new Neuron(ins));
        }
    }

    public void Calculate(List<double> ins)
    {
        inputs = ins;
        double outp = 0.0;
        outputs.Clear();
        for (int neu = 0; neu < neuronCount; neu++)
        {
            outp = 0.0;
            for (int ind = 0; ind < inputCount; ind++)
            {
                outp += neurons[neu].weights[ind] * ins[ind];
            }
            
            outputs.Add(Activation(outp + neurons[neu].bias));
        }
    }

    public void Update(List<double> expect, double alpha)
    {
        for (int neu = 0; neu < neuronCount; neu++)
        {
            neurons[neu].errorGrad = (outputs[neu] - expect[neu]) * Derivative(neu);
            for (int wei = 0; wei < inputCount; wei++)
            {
                neurons[neu].weights[wei] -= alpha * neurons[neu].errorGrad * inputs[wei];
            }
            neurons[neu].bias -= alpha * neurons[neu].errorGrad;
        }
    }

    public void Update(double alpha, List<double> esum)
    {
        for (int neu = 0; neu < neuronCount; neu++)
        {
            neurons[neu].errorGrad = Derivative(neu) * esum[neu];
            for (int wei = 0; wei < inputCount; wei++)
            {
                neurons[neu].weights[wei] -= alpha * neurons[neu].errorGrad * inputs[wei];
            }
            neurons[neu].bias -= alpha * neurons[neu].errorGrad;
        }
    }

    public abstract double Activation(double res);
    public abstract double Derivative(int ind);

    public void Write(StreamWriter sw)
    {
        foreach(Neuron neu in neurons)
        {
            neu.Write(sw);
        }
        sw.WriteLine();
    }
}
