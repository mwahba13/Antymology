using System;
using System.Collections.Generic;
using Antymology.Terrain;
using Components.Ant;
using Components.Terrain.Blocks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components
{
    /// <summary>
    /// For handling the simulation specific aspects.
    /// i.e. ticks, and generational learning
    /// </summary>
    public class SimulationManager : Singleton<SimulationManager>
    {

        #region fields

        public SimulationSettings simSettings;
        public GameObject AntPrefab;

        private float _tickTimer;

        private List<AntBase> antList = new List<AntBase>();

        #endregion

        #region Methods

        private void Start()
        {
            _tickTimer = 0.0f;
        }


        private void Update()
        {
            _tickTimer -= Time.deltaTime;
            if (_tickTimer < 0.0f)
            {
                _tickTimer = simSettings.SimulationTickInterval;
            }
            
        }

        public void GenerateAnts(int NumAnts,int dimensions,int height)
        {
            
            PerlinNoiseAntGenerator(NumAnts,dimensions,height);
        }


        private void TickAnts()
        {
            foreach (var ant in antList)
            {
                ant.Tick();
            }
        }

        

        #endregion


        #region AntGenerators
        
        /// <summary>
        /// Spawns ants in map based on perlin noise 
        /// </summary>
        /// <param name="numAnts"></param>
        private void PerlinNoiseAntGenerator(int numAnts,int dimensions,int height)
        {
            int numAntsSpawned = 0;
            //split board into 4 quadrants
            int quadrantSize = (int)Mathf.Floor(dimensions/4);

                for(int x = 1; x < dimensions-1; x++)
                {
                    for (int z = 1; z < dimensions-1; z++)
                    {
                        float tempX = x;
                        float tempZ = z;
                        float rand1 = Random.Range(0.0f, 1.0f);
                        float rand2 = Random.Range(0.0f, 1.0f);
                    
                        //randomly scatter ants across quadrants
                    
                        //normalize locations before doing perlin
                        tempX /=dimensions-1;
                        tempZ /=dimensions-1;
                    

                        float perlin = Mathf.PerlinNoise(tempX+rand1, tempZ+rand2);
                        if (perlin >= 0.85f)
                        {
                            int randInt1 = Random.Range(0, 3)*quadrantSize;
                            int randInt2 = Random.Range(0, 3)*quadrantSize;
                            if (SpawnAnt(x-randInt1, z-randInt2, height))
                                numAntsSpawned++;
                            if (numAntsSpawned >= numAnts)
                                break;


                        }
                        if (numAntsSpawned >= numAnts)
                            break;
                
                    }
                    
                }
                
                
                //if that first pass didnt place all the ants, spawn the rest randomly
                while (numAntsSpawned < numAnts)
                {
                    Debug.Log("While loop");
                    int tempX = Random.Range(5, dimensions-4);
                    int tempZ = Random.Range(5, dimensions-4);


                    SpawnAnt(tempX ,tempZ ,height);
                    numAntsSpawned++;

                }
                
                Debug.Log("Ant size: " + antList.Count);
            

        }

        #endregion


        #region helpers

        /// <summary>
        /// spawns ant in map on top of block at x,z coords
        /// returns false if space already occupied
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool SpawnAnt(int x, int z, int height)
        {
            //we start at top of box and work our way down until we hit a block
            Vector3 boxMarch = new Vector3(x, height-1, z);
            while (WorldManager.Instance.GetBlockType(boxMarch) == BlockType.Air)
            {
                boxMarch.y--;
                
            }
            //incrememnt to get empty spot above ground
            boxMarch.y++;

            Ray antCheckRay = new Ray(boxMarch, Vector3.up);
            RaycastHit antCheckHit;

            if(Physics.SphereCast(antCheckRay, 1.0f, out antCheckHit))
                if (antCheckHit.collider.tag.Equals("Ant"))
                    return false;
            
            
            //check if already ant on spot

            //add ant to list
            GameObject newObj = Instantiate<GameObject>(AntPrefab,boxMarch,
                Quaternion.identity);

            if (!newObj)
                return false;
            antList.Add(newObj.GetComponent<AntBase>());
            
            return true;
        }

        #endregion
    }
}