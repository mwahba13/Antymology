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
            2,3,5,7,9
        };

        private NeuralNet.NeuralNet nn;

        public void InitNeuralNet()
        {
            nn = new NeuralNet.NeuralNet(inputLayers);
        }
        
        /// <summary>
        /// Inputs (all between 0 and 1) : 
        /// 0 - Distance to queen
        /// 1 - Block underneath ant
        /// 2 - 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public float[] RunNeuralNet(
            float distToQueen,
            float blockUnder)
        {
            float[] nnInput = new[]
            {
                distToQueen, blockUnder
            };
            
            return nn.FeedForward(nnInput);
            
            
            
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

    }
}    



