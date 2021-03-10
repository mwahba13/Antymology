using System;
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
            
            _timer = 0.0f;
        }


        private void Update()
        {
            
            _timer -= Time.deltaTime;
            if (_timer < 0.0f)
            {
                
                RandomMovement();
                
                //_health -= _antSettings.healthDecreaseAmount;
                if(_health <= 0.0f)
                    OnDeath();
                _timer = _antSettings.timeStep;
            }

            
        }

        private void RandomMovement()
        {
            while(!_antBody.ProcessAction(actionArray[Random.Range(0,3)]))
                continue;
            
        }

        private void OnDeath()
        {
            
        }
        

        #endregion


        #region helpers

        

        #endregion
    }
}