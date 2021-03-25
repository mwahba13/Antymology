using UnityEngine;
using NeuralNet;

namespace Components.Ant
{
    /// <summary>
    /// Responsible for ant's behavior/AI
    /// </summary>
    public class AntBrain : MonoBehaviour
    {

        private float _fitnessFunction;
        
        private static readonly int[] inputLayers = new int[]
        {
            3,5,7,9
        };
        [SerializeField]
        private NeuralNet.NeuralNet nn;

        public void InitNeuralNet()
        {
            nn = new NeuralNet.NeuralNet(inputLayers);
        }
        

        public float[] RunAntNeuralNet(

            float blocksDug,
            float mulchEaten,
            float healthDonate)
        {
            float[] nnInput = new[]
            {
                blocksDug,mulchEaten,healthDonate
            };
            
            return nn.FeedForward(nnInput);
            
        }

        public float[] RunQueenNeuralNet(
            float blocksBuilt,
            float mulchEaten,
            float numNeighbours
        )
        {
            float[] nnInput = new[]
            {
                blocksBuilt,
            };

            return nn.FeedForward(nnInput);
        }



        public void MutateWeights(float mutatePercent)
        {
            //choose random number of weights to mutate
            int numToMutate = Random.Range(1, nn.GetWeights().Length);

            int counter = 0;
            while (counter < numToMutate)
            {
                int i = Random.Range(0, nn.weights.Length);
                int j = Random.Range(0, nn.weights[i].Length);
                int k = Random.Range(0, nn.weights[i][j].Length);

                float value = nn.weights[i][j][k];

                nn.weights[i][j][k] = Random.Range(value - mutatePercent, value + mutatePercent);

                counter++;

            }
            

        }
    
        //todo: fitness function
        public float UpdateFitnessFunction()
        {
            return 1.0f;
        }

        public float GetFitnessFunction()
        {
            return _fitnessFunction;
        }

        public float[][][] GetWeights()
        {
            return nn.GetWeights();
        }

        public void SetWeights(float[][][] newWeights)
        {
            nn.SetWeights(newWeights);
        }

    }
}    



