using System;
using UnityEngine;

namespace Components.Ant
{
    
    
    
    
    /// <summary>
    /// Enum that describes the actions an ant can take
    /// </summary>
    public enum EAction
    {
        //relative to global axis
        ForwardMove,BackMove,RightMove,LeftMove,Eat,Dig
    }
    
    
    /// <summary>
    /// Responsible for locomotion/effector function.
    /// </summary>
    public class AntBody : MonoBehaviour
    {
        #region fields

        #endregion


        #region methods
        /// <summary>
        /// returns true if ant can move in particular direction
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool ProcessAction(EAction action)
        {

            float XDir = 0.0f;
            float YDir = 0.0f;
            float ZDir = 0.0f;

            switch (action)
            {
                case EAction.ForwardMove:
                    ZDir = 1.0f;
                    break;
                case EAction.BackMove:
                    ZDir = -1.0f;
                    break;
                case EAction.RightMove:
                    XDir = 1.0f;
                    break;
                case EAction.LeftMove:
                    XDir = -1.0f;
                    break;
            }

            
            
            Vector3 pos = transform.position;


            bool straightHit = Physics.Raycast(pos, new Vector3(XDir, 0.0f, ZDir),1.0f);
            bool downHit = Physics.Raycast(pos, new Vector3(XDir, -.75f, ZDir),1.0f);
            bool furtherDownHit = Physics.Raycast(pos + new Vector3(XDir, -1.0f, ZDir), new Vector3(0.0f, -1.0f), 1.0f);
            bool upHit = Physics.Raycast(pos, new Vector3(XDir, 1.25f, ZDir),1.0f);


            if (straightHit && upHit)
                return false;

            if (!downHit && !furtherDownHit)
                return false;

            if (straightHit && !upHit)
                YDir = 1.0f;
            
            //cast ray to see if we can drop down
            if (furtherDownHit && !downHit)
                YDir = -1.0f;


            transform.Translate(new Vector3(XDir,YDir,ZDir));



            return true;

        }



        #endregion

        #region helpers
        
        /// <summary>
        /// returns true if ant has a way forward in given position, without actually moving ant
        /// </summary>
        /// <returns></returns>
        public bool CanMoveForward()
        {
            Vector3 pos = transform.position;
            bool straightHit = Physics.Raycast(pos, new Vector3(0.0f, 0.0f, 1.0f),1.0f);
            bool downHit = Physics.Raycast(pos, new Vector3(0.0f, -.75f, 1.0f),1.0f);
            bool upHit = Physics.Raycast(pos, new Vector3(0.0f, 1.25f, 1.0f),1.0f);
            Debug.DrawRay(pos,Vector3.forward*5.0f,Color.blue);
            //can climb on top of block
            if (straightHit)
            {
                if (upHit)
                    return false;
                else
                {
                    return true;
                }
            }
            
            //can move directly forward
            if (downHit && !straightHit)
            {
                return true;
            }
            
            //cam drop down below
            if (Physics.Raycast(pos + Vector3.forward + Vector3.down, new Vector3(0.0f, -1.0f), 1.0f))
            {
                return true;
            }
            

            return false;
        }

        

        #endregion
    }
}