using UnityEngine;

namespace Components
{
    [CreateAssetMenu(fileName = "SimSettings", menuName = "SimSettings", order = 0)]
    public class SimulationSettings : ScriptableObject
    {
        public float SimulationTickInterval;
    }
}