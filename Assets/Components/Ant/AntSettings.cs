﻿using UnityEngine;

namespace Components.Ant
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class AntSettings : ScriptableObject
    {
        [Tooltip("Health that each ant starts at")]
        public float initalHealth;

        [Tooltip("Amount by which ants health decreases every timestep")] 
        public float healthDecreaseAmount;

        [Tooltip("Amount by which ants health increases after eating mulch")]
        public float healthIncreaseAmount;
        
        public float timeStep;

    }
}