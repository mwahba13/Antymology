using UnityEngine;

namespace Components
{
    [CreateAssetMenu(fileName = "SimSettings", menuName = "SimSettings", order = 0)]
    public class SimulationSettings : ScriptableObject
    {
        //Time in Seconds between simulation ticks
        public float SimulationTickInterval;

        //how many ticks until a new generation is created
        public int TicksUntilEvolution;

        //How likely mutation is to occur
        public float ProbabilityOfMutation;

        //Maximum percentage of original amount a gene can be mutated
        public float MutationPercentage;

    }
}