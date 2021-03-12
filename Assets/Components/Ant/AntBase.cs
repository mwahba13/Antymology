using System;
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
            _health = _antSettings.initalHealth;
            _antBody = gameObject.AddComponent<AntBody>() as AntBody;
            _timer = _antSettings.timeStep;
            
            
        }

        public void Tick()
        {
            RandomMovement();
            if(_health <= 0.0f)
                KillAnt();
            
        }

        private void Update()
        {
            /*
            _timer -= Time.deltaTime;
            if (_timer < 0.0f)
            {
                
                RandomMovement();
                
                //DepleteHealthOnTick();

                _timer = _antSettings.timeStep;
            }   
            */
            
        }
        
     

        private void RandomMovement()
        {

            if (Random.Range(0.0f, 1.0f) > 0.5)
            {
                if (GetBlockUnderneath() == BlockType.Mulch)
                {
                    EatMulch();
                }
                
            }
            else
            {
                while(!_antBody.ProcessAction(actionArray[Random.Range(0,3)]))
                    continue;
            }
            
            //move in random direction

            
            
        }
        private void DepleteHealthOnTick()
        {
            float deplete = _antSettings.healthDecreaseAmount;
            if (GetBlockUnderneath() == BlockType.Acidic)
                deplete *= 2.0f;
            _health -= deplete;
        }


    

        #endregion

        #region Actions 
        private void KillAnt()
        {
            
        }
        
        private void DigBlock()
        {
            AbstractBlock air = new AirBlock();
            WorldManager.Instance.SetBlock((int)transform.position.x,
                (int)transform.position.y - 1,(int)transform.position.z,air);
        }

        private void EatMulch()
        {
            Debug.Log("Eat mulch");

            //remove block below ant
            DigBlock();
            
            //move ant down
            transform.Translate(Vector3.down);

            _health += _antSettings.healthIncreaseAmount;
            
        }



        

        #endregion


        #region helpers
    
        /// <summary>
        /// Gets the block object directly underneath the ant
        /// </summary>
        /// <returns></returns>
        private BlockType GetBlockUnderneath()
        {

            Vector3 pos = transform.position;
            
            AbstractBlock block =  WorldManager.Instance.GetBlock((int) pos.x, 
                (int) pos.y - 1, (int) pos.z);
            
            return (BlockHelper.GetBlockType(block));
        }

        

        #endregion
    }
}