using System.Collections.Generic;
using System;
using UnityEngine;
using Random = System.Random;

namespace NeuralNet
{
    //https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef
    public class NeuralNet
    {
        private int[] layers = null;
        private float[][] neurons = null;
        private float[][] biases = null;
        [SerializeField]
        public float[][][] weights = null;
        private int[] activations = null;

        public float fitness = 0;

        private Random _random = new Random();
        
        
        public NeuralNet(int[] layers)
        {
            this.layers = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = layers[i];
            }
            
            InitNeurons();
            InitBiases();
            InitWeights();
        }



        private void InitNeurons()
        {
            List<float[]> neuronList = new List<float[]>();
            
            for(int i = 0; i < layers.Length; i++)
            {
                neuronList.Add(new float[layers[i]]);
            }

            neurons = neuronList.ToArray();
        }

        private void InitBiases()
        {
            List<float[]> biasList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                float[] bias = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                {
                    bias[j] = (float)_random.NextDouble() - 0.5f;
                }
                
                biasList.Add(bias);
            }

            biases = biasList.ToArray();
        }
        

        private void InitWeights()
        {
            List<float[][]> weightsList = new List<float[][]>();

            for (int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightList = new List<float[]>();

                int neuronsInPrevLayer = layers[i - 1];

                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPrevLayer];

                    for (int k = 0; k < neuronsInPrevLayer; k++)
                    {
                        neuronWeights[k] = (float)_random.NextDouble() - 0.5f;
                    }
                    
                    layerWeightList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightList.ToArray());
                
            }
            
            
            weights = weightsList.ToArray();

        }



        public float[] FeedForward(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }

            for (int i = 1; i < layers.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float value = 0.25f;
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k];
                    }

                    neurons[i][j] = (float)Math.Atan(value + biases[i][j]);
                }
            }

            //return last/output layer
            return neurons[neurons.Length - 1];
        }


        public float[][][] GetWeights()
        {
            return weights;
        }
        
        
        public void SetWeights(float[][][] newWeight)
        {
            weights = newWeight;
        }



    }
}