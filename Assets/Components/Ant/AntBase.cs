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
        private float _health;
        
        private bool _isQueen;
        private int _antID;
        
        //ant stats needed for NN
        public float healthDonatedToQueen = 0.0f;
        public float blocksBuilt = 0.0f;
        public float blocksDug = 0.0f;
        public float mulchEaten = 0.0f;

        //initial position of the ant - will use this for fitness function
        private Vector3 _initPos;
 
        #endregion
        

        #region methods

        private void Awake()
        {
            _antBody = gameObject.AddComponent<AntBody>() as AntBody;
            _antBody._antBase = this;
            
            
            
            
            _antBrain = gameObject.AddComponent<AntBrain>() as AntBrain;
            _antBrain.InitNeuralNet();
        }

        private void Start()
        {
            _health = _antSettings.initalHealth;

        }

        public void Tick()
        {
            //first we need to get the inputs we need for the NN
            //get list of possible movements
            List<EAction> actionList = _antBody.GetValidMoveList(_isQueen);

            
            float[] nnOut;
            if (_isQueen)
                nnOut = _antBrain.RunQueenNeuralNet(
                    _health,
                    blocksBuilt,
                    mulchEaten,
                    _antBody.GetNeighbourCount(),
                    _antBody.GetDistToNearestNeigh()
                );
            else
                nnOut = _antBrain.RunAntNeuralNet(
                _health,
                    blocksDug,
            _health,
                    healthDonatedToQueen, 
            (transform.position - SimulationManager.Instance.GetQueenLocation()).magnitude
                    );


            nnOut = AddBiases(nnOut);
            
            SimulationManager.Instance.LogAntActions(this,nnOut,DecideAction(nnOut,actionList));
            
            //DecideAction(nnOut,actionList);
            DepleteHealthOnTick();


            //RandomMovement();
        }


        //add biases to the output of the neural net to encourage certain behaviors in certain circumstances
        private float[] AddBiases(float[] weights)
        {
            //if ant has low health, they should eat
            if (_health < _antSettings.initalHealth / 2)
                weights[4] += 0.3f;

            //if queen has lots of health, they should build
            if (_isQueen && _health > _antSettings.initalHealth / 2)
                weights[7] += 0.5f;

            return weights;
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
        private EAction DecideAction(float[] nnOut,List<EAction> actionList)
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

                    
                    _antBody.ProcessAction(tryAction);
                    return tryAction;
                }
                //if we cant do the highest rated action - reduce the value so we dont see it again
                else
                {
                    nnOut[indexMax] -= 1.0f;
                }
                
            }

            return EAction.Nothing;

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
            _health = _antSettings.initalHealth;
            healthDonatedToQueen = 0.0f;
            blocksBuilt = 0.0f;
            blocksDug = 0.0f;
            mulchEaten = 0.0f;
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

        
        public void MutateWeights(float mutatePercentage)
        {
            _antBrain.MutateWeights(mutatePercentage);
        }

        public void SetWeight(float[][][] newWeights)
        {
            _antBrain.SetWeights(newWeights);
        }
        
        public float CalculateFitnessFunction()
        {
            return _antBrain.CalculateFitnessFunction(_isQueen,
                _health
                ,healthDonatedToQueen
                ,blocksBuilt
                ,_initPos);
        }

       

        //returns a list of valid moves

        public void SetIsQueen(bool b)
        {
            _isQueen = b;
        }

        public bool GetIsQueen()
        {
            return _isQueen;
        }

        public void SetInitPos(Vector3 trans)
        {
            _initPos = trans;   
        }

        #endregion
    }
}