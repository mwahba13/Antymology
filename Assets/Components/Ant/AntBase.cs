using System;
using UnityEngine;

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
            _antBody.ProcessAction(EAction.BackMove);


        }

        private void OnDeath()
        {
            
        }
        

        #endregion


        #region helpers

        

        #endregion
    }
}