using System;
using System.Collections.Generic;
using System.Linq;
using Antymology.Terrain;
using Components.Terrain.Blocks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components.Ant
{
    /// <summary>
    /// Ant base class. holds base information and responsible for high level function.
    /// </summary>
    public class AntBase : MonoBehaviour
    {

        #region fields
        
        
        
        [SerializeField]
        private AntSettings _antSettings;

        private AntBody _antBody;
        private AntBrain _antBrain;

        private float[] _decisionWeights;
        private float _timer;
        private float _health;
        
        private bool _isQueen;

        //initial position of the ant - will use this for fitness function
        private Vector3 _initPos;
 
        #endregion
        

        #region methods
        private void Start()
        {
            
            _antBody = gameObject.AddComponent<AntBody>() as AntBody;
            _antBody._antBase = this;
            
            
            
            
            _antBrain = gameObject.AddComponent<AntBrain>() as AntBrain;
            _antBrain.InitNeuralNet();
            
            _health = _antSettings.initalHealth;
            _timer = _antSettings.timeStep;
            
            
        }

        public void Tick()
        {
            //first we need to get the inputs we need for the NN
            //get list of possible movements
            List<EAction> actionList = _antBody.GetValidMoveList(_isQueen);

            //todo: all nn inputs
           float[] nnOut = _antBrain.RunNeuralNet((
                            SimulationManager.Instance.GetQueenLocation() - transform.position).magnitude,
                            (float)_antBody.GetBlockUnderneath()
                            );

            
            DecideAction(nnOut,actionList);
            //RandomMovement();
        }

        public void Evolve()
        {
            _antBrain.UpdateFitnessFunction();
        }
        
        
        private void RandomMovement()
        {
            List<EAction> list = _antBody.GetValidMoveList(false);
            if(list.Count != 0)
                _antBody.ProcessAction(list[Random.Range(0, list.Count)]);
        }

        /// <summary>
        /// takes input from NN and decides next action
        /// </summary>
        private void DecideAction(float[] nnOut,List<EAction> actionList)
        {
            
            //Debug.Log("Ant: " + this.name);
            //Debug.Log("NN out");
            /*
            foreach (var nn in nnOut)
            {
                Debug.Log(nn);
            }
            
            Debug.Log("Action List");
            foreach (var action in actionList)
            {
                Debug.Log(action);
            }
            */            
            for (int i = 0; i < 9; i++)
            {
                //index of hte max value - index corresponds to action
                int indexMax = Array.IndexOf(nnOut, nnOut.Max());
                EAction tryAction = (EAction) indexMax;

                if (actionList.Contains(tryAction))
                {
                    Debug.Log("Ant: " + this.name + "Decision made: " + tryAction);
                    ;
                    _antBody.ProcessAction(tryAction);
                    break;
                }
                //if we cant do the highest rated action - reduce the value so we dont see it again
                else
                {
                    nnOut[indexMax] -= 1.0f;
                }
                
            }

            _antBrain.UpdateFitnessFunction();


        }
        
        private void DepleteHealthOnTick()
        {
            float deplete = _antSettings.healthDecreaseAmount;
            if (_antBody.GetBlockUnderneath() == BlockType.Acidic)
                deplete *= 2.0f;
            _health -= deplete;
        }


    

        #endregion


        #region NNInputs

        

        #endregion


        #region helpers

        public void DecrementHealth(bool isDoubled)
        {
            _health += _antSettings.healthDecreaseAmount;
        }

        public void IncrementHealth()
        {
            _health += _antSettings.healthIncreaseAmount;
            Mathf.Clamp(_health, 0, _antSettings.initalHealth);
        }

        public void SetHealth(float newH)
        {
            _health = newH;
        }

        public float GetHealth()
        {
            return _health;
            
        }

        public float[][][] GetWeights()
        {
            return _antBrain.GetWeights();
        }

        public float GetFitnessFunction()
        {
            return _antBrain.GetFitnessFunction();
        }

        //returns a list of valid moves

        public void SetIsQueen(bool b)
        {
            _isQueen = b;
        }

        public void SetInitPos(Vector3 trans)
        {
            _initPos = trans;   
        }

        #endregion
    }
}