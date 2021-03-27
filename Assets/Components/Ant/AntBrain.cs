using UnityEngine;
using NeuralNet;

namespace Components.Ant
{
    /// <summary>
    /// Responsible for ant's behavior/AI
    /// </summary>
    public class AntBrain : MonoBehaviour
    {



        private float _fitness;
        
        private static readonly int[] inputLayers = new int[]
        {
            5,5,7,9
        };
        [SerializeField]
        private NeuralNet.NeuralNet nn;

        public void InitNeuralNet()
        {
            nn = new NeuralNet.NeuralNet(inputLayers);
        }
        

        public float[] RunAntNeuralNet(
            float health,
            float blocksDug,
            float mulchEaten,
            float healthDonated,
            float distToQueen
            )
        {
            float[] nnInput = new[]
            {
                health,blocksDug,mulchEaten,healthDonated,distToQueen
            };
            
            return nn.FeedForward(nnInput);
            
        }

        public float[] RunQueenNeuralNet(
            float health,
            float blocksBuilt,
            float mulchEaten,
            float numNeighbours,
            float distToNearestNeigh 
        )
        {
            float[] nnInput = new[]
            {
                health,blocksBuilt,mulchEaten,numNeighbours,distToNearestNeigh
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

                nn.weights[i][j][k] = Mathf.Clamp( Random.Range(value - mutatePercent, value + mutatePercent)
                    ,-1.0f,1.0f);

                counter++;

            }
            

        }
    

        public float CalculateFitnessFunction(bool isQueen, float health, 
            float donatedHealth,float blocksBuilt)
        {
            float fitness = 0.0f;

            SimulationManager simManage = SimulationManager.Instance;

            fitness += health * simManage._healthWeight;

            if (isQueen)
            {
                fitness += blocksBuilt * simManage._blockBuildWeight;
            }

            else
            {
                fitness += donatedHealth * simManage._donateHealthToQueenWeight;
                fitness += (transform.position - simManage.GetQueenLocation()).magnitude * simManage._distToQueenWeight;
                fitness += simManage.GetQueenHealth() * simManage._queenHealthWeight;
            }

            fitness /= 100.0f;

            _fitness = fitness;
            return fitness;

        }

        public float GetFitnessFunction()
        {
            return _fitness;
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



