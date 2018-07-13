using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NeuralNet {

    public int inputCount;
    public int outputCount;
    public int hiddenCount;
    public int neuronCount;

    public double alpha;

    private List<Layer> layers = new List<Layer>();

	public NeuralNet(int ins, int outs, int hid, int node, double a)
    {
        inputCount = ins;
        outputCount = outs;
        hiddenCount = hid;
        neuronCount = node;
        alpha = a;

        if (hid > 0)
        {
            // Input / First Hidden
            layers.Add(new SigmoidLayer(inputCount, neuronCount));
            // Hidden
            for (int i = 0; i < hiddenCount - 1; i++)
            {
                layers.Add(new SigmoidLayer(neuronCount, neuronCount));
            }
            //Output
            layers.Add(new SigmoidLayer(neuronCount, outs));
        }
        else
        {
            layers.Add(new SigmoidLayer(inputCount, outs));
        }
    }

    public List<double> Run(List<double> ins)
    {
        layers[0].Calculate(new List<double>(ins));

        for (int i = 1; i < hiddenCount + 1; i++)
        {
            layers[i].Calculate(new List<double>(layers[i - 1].outputs));
        }

        return layers[hiddenCount].outputs;
    }

    public List<double> Train(List<double> ins, List<double> outs)
    {
        Run(ins);
        UpdateWeights(outs);
        return layers[hiddenCount].outputs;
    }

    public void UpdateWeights(List<double> expected)
    {
        layers[hiddenCount].Update(expected, alpha);
        for (int i = hiddenCount - 1; i >= 0; i--)
        {
            layers[i].Update(alpha, ErrorSums(i));
        }
    }

    private List<double> ErrorSums(int layer)
    {
        List<double> sums = new List<double>();
        double sum = 0;
        for (int neu = 0; neu < layers[layer].neuronCount; neu++)
        {
            sum = 0;
            for (int i = 0; i < layers[layer + 1].neuronCount; i++)
            {
                sum += layers[layer + 1].neurons[i].errorGrad * layers[layer + 1].neurons[i].weights[neu];
            }
            sums.Add(sum);
        }
        return sums;
    }

    public void Write(string file)
    {
        StreamWriter writer = new StreamWriter(file, true);
        foreach (Layer lay in layers)
        {
            lay.Write(writer);
        }
        writer.Close();
    }

    public class TanHLayer : Layer
    {
        public TanHLayer(int ins, int count) : base(ins, count) { }

        public override double Activation(double res)
        {
            return (Mathf.Exp((float)res) - Mathf.Exp((float)-res)) / (Mathf.Exp((float)res) + Mathf.Exp((float)-res));
        }

        public override double Derivative(int ind)
        {
            return 1.0 - Mathf.Pow((float)outputs[ind], 2f);
        }
    }

    public class SigmoidLayer : Layer
    {
        public SigmoidLayer(int ins, int count) : base(ins, count) { }

        public override double Activation(double res)
        {
            return 1.0 / (1 + Mathf.Exp((float)-res));
        }

        public override double Derivative(int ind)
        {
            return outputs[ind] * (1.0 - outputs[ind]);
        }
    }

    public class LeakyRELULayer : Layer
    {
        public LeakyRELULayer(int ins, int count) : base(ins, count) { }

        public override double Activation(double res)
        {
            if (res < 0)
            {
                return .01 * res;
            }
            return res;
        }

        public override double Derivative(int ind)
        {
            if (outputs[ind] < 0)
            {
                return .01;
            }
            return 1;
        }
    }
}
