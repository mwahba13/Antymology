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
        public AntSettings _antSettings;


        private AntBody _antBody;
        private AntBrain _antBrain;

        [SerializeField]
        private float[] _decisionWeights;
        private float _timer;
        [SerializeField]
        private float _health;
        
        private bool _isQueen;
        private int _antID;
        
        //ant stats needed for NN
        public float healthDonated = 0.0f;
        public float blocksBuilt = 0.0f;
        public float blocksDug = 0.0f;
        public float mulchEaten = 0.0f;

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

            float[] nnOut;
            if (_isQueen)
                nnOut = _antBrain.RunQueenNeuralNet(
                    blocksBuilt,
                    mulchEaten,
                    _antBody.GetNeighbourCount()
                    );
            else
                nnOut = _antBrain.RunAntNeuralNet(
                                blocksDug,
                                _health,
                                healthDonated
                                );
            
            Debug.Log(" ");
            Debug.Log("Ant: " + this.GetInstanceID());
            Debug.Log("Fr: " + nnOut[0]);
            Debug.Log("Bck: "+nnOut[1]);
            Debug.Log("Rit: "+nnOut[2]);
            Debug.Log("Lft: "+nnOut[3]);
            Debug.Log("Eat: "+nnOut[4]);
            Debug.Log("Dig: "+nnOut[5]);
            Debug.Log("Hlth: "+nnOut[6]);
            Debug.Log("Bld: "+nnOut[7]);
            Debug.Log("Nth: "+nnOut[8]);
            
            
            
            DecideAction(nnOut,actionList);
            //DepleteHealthOnTick();
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

        public void ResetStats()
        {
         healthDonated = 0.0f;
         blocksBuilt = 0.0f;
         blocksDug = 0.0f;
        }

        public int GetAntID()
        {
            return _antID;
            
        }

        public void SetAntID(int newID)
        {
            _antID = newID;
        }
        
        
        public void DecrementHealth(bool isDoubled)
        {
            _health += _antSettings.healthDecreaseAmount;
        }

        public void DecrementHealth(float amnt)
        {
            _health -= amnt;
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

        //TODO: Implement mutation
        public void MutateWeights(float mutatePercentage)
        {
            _antBrain.MutateWeights(mutatePercentage);
        }

        public void SetWeight(float[][][] newWeights)
        {
            _antBrain.SetWeights(newWeights);
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