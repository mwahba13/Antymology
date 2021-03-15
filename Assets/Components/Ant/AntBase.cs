using System;
using System.Collections.Generic;
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

        private EAction[] actionArray = new EAction[]
        {
            EAction.ForwardMove,
            EAction.BackMove,
            EAction.RightMove,
            EAction.LeftMove
        };


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
            RandomMovement();
        }

        public void Update()
        {
            /*
            Collider[] hits =  Physics.OverlapSphere(transform.position, 1.0f);

            foreach (var hit in hits)
            {
                Debug.Log("HIt: " + hit.name);
            }
            */
        }

        private void MoveAnt()
        {
            
        }

        private void UpdateDecisionWeights()
        {
            ///put inputs into NN and update weihts
        }

        private void RandomMovement()
        {
            List<EAction> list = _antBody.GetValidMoveList(false);
            if(list.Count != 0)
                _antBody.ProcessAction(list[Random.Range(0, list.Count)]);
        }
        private void DepleteHealthOnTick()
        {
            float deplete = _antSettings.healthDecreaseAmount;
            if (_antBody.GetBlockUnderneath() == BlockType.Acidic)
                deplete *= 2.0f;
            _health -= deplete;
        }


    

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
    
        //returns a list of valid moves


        

        #endregion
    }
}