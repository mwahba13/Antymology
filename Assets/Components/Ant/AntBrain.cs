using UnityEngine;
using NeuralNet;

namespace Components.Ant
{
    /// <summary>
    /// Responsible for ant's behavior/AI
    /// </summary>
    public class AntBrain : MonoBehaviour
    {
        private static readonly int[] inputLayers = new int[]
        {
            2, 3, 4
        };

        private NeuralNet.NeuralNet nn;

        public void InitNeuralNet()
        {
            nn = new NeuralNet.NeuralNet(inputLayers);
        }

        public float[] RunNeuralNet(float[] input)
        {
            return nn.FeedForward(input);
        }
    }
}    



