using System;
using System.Collections.Generic;
using Antymology.Terrain;
using Components.Terrain.Blocks;
using UnityEngine;
using UnityEngine.Animations;

namespace Components.Ant
{
    
    
    
    
    /// <summary>
    /// Enum that describes the actions an ant can take
    /// </summary>
    public enum EAction
    {
        //relative to global axis
        ForwardMove,
        BackMove,
        RightMove,
        LeftMove,
        Eat,
        Dig ,
        GiveHealth,
        Build,
        Nothing,
        
        
    }
    
    
    /// <summary>
    /// Responsible for locomotion/effector functions.
    /// </summary>
    public class AntBody : MonoBehaviour
    {
        #region fields

        public AntBase _antBase;

        private List<GameObject> _neighbourList;

        #endregion
        
        #region Actions
        
        
        private void DigBlock()
        {
            AbstractBlock air = new AirBlock();
            WorldManager.Instance.SetBlock((int)transform.position.x,
                (int)transform.position.y - 1,(int)transform.position.z,air);
            //move ant down
            transform.Translate(Vector3.down);
        }

        private void EatMulch()
        {
            //remove block below ant
            DigBlock();
            
            _antBase.IncrementHealth();
            
        }


        private void GiveHealth(AntBase reciever)
        {
            
        }
        
        private void BuildBlock()
        {
            AbstractBlock nestBlock = new NestBlock();
            transform.Translate(Vector3.up);
            WorldManager.Instance.SetBlock((int)transform.position.x,
                (int)transform.position.y-1,(int)transform.position.z,nestBlock);
            
            _antBase.SetHealth(_antBase.GetHealth()/3);
        }
        
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
            float rotate = 0.0f;
            switch (action)
            {
                case EAction.ForwardMove:
                    ZDir = 1.0f;
                    break;
                case EAction.BackMove:
                    rotate = 180.0f;
                    ZDir = -1.0f;
                    break;
                case EAction.RightMove:
                    rotate = 90.0f;
                    XDir = 1.0f;
                    break;
                case EAction.LeftMove:
                    rotate = 270.0f;
                    XDir = -1.0f;
                    break;
                case EAction.Dig:
                    DigBlock();
                    return true;
                case EAction.GiveHealth:
                    break;
                case EAction.Eat:
                    EatMulch();
                    return true;
                case EAction.Build:
                    BuildBlock();
                    return true;
                case EAction.Nothing:
                    return true;
                
                
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
        
        //returns list of valid actions - called every tick!
        public List<EAction> GetValidMoveList(bool isQueen)
        {
            List<EAction> outList = new List<EAction>();

            BlockType blockUnder = GetBlockUnderneath();
            
            if(blockUnder == BlockType.Mulch)
                outList.Add(EAction.Eat);
            if(blockUnder != BlockType.Container)
                outList.Add(EAction.Dig);
            
            if(CanMove(EAction.ForwardMove))
                outList.Add(EAction.ForwardMove);
            
            if(CanMove(EAction.BackMove))
                outList.Add(EAction.BackMove);
            
            if(CanMove(EAction.LeftMove))
                outList.Add(EAction.LeftMove);
            
            if(CanMove(EAction.RightMove))
                outList.Add(EAction.RightMove);
            
            
            
            //give health functions
            //if(CanGiveHealth())
                //outList.Add(EAction.GiveHealth);
            
            if(isQueen)
                outList.Add(EAction.Build);
            
            outList.Add(EAction.Nothing);

            return outList;
        }
    
        /// <summary>
        /// Gets the block object directly underneath the ant
        /// </summary>
        /// <returns></returns>
        public BlockType GetBlockUnderneath()
        {

            Vector3Int pos = Vector3Int.zero;
            pos.x = (int)transform.position.x;
            pos.y = (int) transform.position.y;
            pos.z = (int) transform.position.z;
            
            AbstractBlock block =  WorldManager.Instance.GetBlock((int) pos.x, 
                (int) pos.y - 1, (int) pos.z);
            
            return (BlockHelper.GetBlockType(block));
        }
        
        //todo: functionality surrounding give health
        private bool CanGiveHealth()
        {
            Collider[] neighbours = Physics.OverlapSphere(transform.position, 1.0f);
            bool hasNeighbour = false;
            
            foreach (var hit in neighbours)
            {
                if (hit.tag.Equals("Ant"))
                {
                    hasNeighbour = true;
                    _neighbourList.Add(hit.gameObject);
                }
            }

            return hasNeighbour;
        }
        
        
        
        
        /// <summary>
        /// returns true if ant has a way forward in given position, without actually moving ant
        /// </summary>
        /// <returns></returns>
        private bool CanMove(EAction action)
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

            return true;

        }

       

        #endregion
    }
}