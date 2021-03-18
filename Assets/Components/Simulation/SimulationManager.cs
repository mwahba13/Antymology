﻿using System;
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
        public GameObject QueenPrefab;

        [SerializeField]
        private float _tickTimer;
        [SerializeField]
        private int _generation = 0;
        [SerializeField]
        private int _ticksUntilEvolution;
        
        private List<AntBase> _antList = new List<AntBase>();
        private AntBase _queen;

        #endregion

        #region Methods

        private void Start()
        {
            _tickTimer = 0.0f;
            _ticksUntilEvolution = simSettings.TicksUntilEvolution;
        }


        private void Update()
        {
            
            _tickTimer -= Time.deltaTime;
            if (_tickTimer < 0.0f)
            {
                TickAnts();
                _tickTimer = simSettings.SimulationTickInterval;

                _ticksUntilEvolution--;
                if(_ticksUntilEvolution <= 0)
                    Antvolution();
                


            }
            
        }

        public void GenerateAnts(int numAnts,int dimensions,int height)
        {
            RandomAntGenerator(numAnts,dimensions,height);
            //PerlinNoiseAntGenerator(NumAnts,dimensions,height);
            //SpawnQueen(height,dimensions);
        }

        //TODO: ant learning
        private void Antvolution()
        {
            //we get the top two fitness values and do some reproducing
            AntBase topAnt = null;
            AntBase secondAnt = null;

            float bestFit = 0;
            
            //get top ant
            foreach (var ant in _antList)
                if (ant.GetFitnessFunction() > bestFit)
                    topAnt = ant;

            bestFit = 0;
            //get second top ant
            foreach (var ant in _antList)
            {
                if (ant.GetFitnessFunction() > bestFit && !ant.Equals(topAnt))
                    secondAnt = ant;
            }

            float[][][] childWeights = SexualHealing(topAnt, secondAnt);
            
            _ticksUntilEvolution = simSettings.TicksUntilEvolution;
            _generation++;
        }

        //todo: mutation
        /*When I get that feeling
         *I want sexual healing
         *Sexual healing, oh baby
         *Makes me feel so fine
         */
        private float[][][] SexualHealing(AntBase Mommy, AntBase Daddy)
        {
            float[][][] outWeights = null;
            float[][][] mommyWeights = Mommy.GetWeights();
            float[][][] daddyWeights = Mommy.GetWeights();

            for (int i = 0; i < mommyWeights.Length; i++)
            {
                for (int j = 0; j < mommyWeights[i].Length; j++)
                {
                    for (int k = 0; k < mommyWeights[i][k].Length; k++)
                    {
                        float insert;
                        float randFloat = Random.Range(0.0f, 1.0f);
                        //crossover
                        if (randFloat > 0.5f)
                            insert = mommyWeights[i][j][k];
                        else
                            insert = daddyWeights[i][j][k];

                        if (randFloat > simSettings.ProbabilityOfMutation)
                            insert = Random.Range(insert - (insert * simSettings.MutationPercentage),
                                insert + (insert * simSettings.MutationPercentage));
                        
                        outWeights[i][j][k] = insert;

                    }
                }
            }
            
            return outWeights;
        }

        private void TickAnts()
        {
            
            foreach (var ant in _antList)
            {
                ant.Tick();
                
            }
            
            _queen.Tick();
            
        }

        

        #endregion


        
        
        
        
        
        #region AntGenerators

        private void RandomAntGenerator(int numAnts, int dimensions, int height)
        {
            int i = 0;
            //spawn peasant ants
            while (i < numAnts - 1)
            {
                int tempX = Random.Range(1, dimensions - 1);
                int tempZ = Random.Range(1, dimensions - 1);

                if (SpawnAnt(tempX, tempZ, height, false))
                    i++;
            }
            
            //spawn queen
            bool isQueenSpawned = false;

            while (!isQueenSpawned)
            {
                int tempX = Random.Range(1, dimensions - 1);
                int tempZ = Random.Range(1, dimensions - 1);

                if (SpawnAnt(tempX, tempZ, height, true))
                    isQueenSpawned = true;
            }
            
        }

        /// <summary>
        /// Spawns ants in map based on perlin noise. One ant randomly chosen to be queen 
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
                           
                            if (SpawnAnt(x-randInt1, z-randInt2, height,false))
                                numAntsSpawned++;
                            if (numAntsSpawned >= numAnts)
                                break;


                        }
                        if (numAntsSpawned >= numAnts)
                            break;
                
                    }
                    
                }


        }


        
        #endregion


        #region helpers

        public Vector3 GetQueenLocation()
        {
            return _queen.transform.position;
            
        }
        
        
        /// <summary>
        /// spawns the queen somewhere randomly
        /// </summary>
        private void SpawnQueen(int height,int dimensions)
        {
            bool isSpawned = false;

            while (!isSpawned)
            {
                int tempX = Random.Range(2, dimensions - 3);
                int tempZ = Random.Range(2, dimensions - 3);

                if (SpawnAnt(tempX, tempZ, height, true))
                    isSpawned = true;
            }
        }

        //TODO: kill ant
        /// <summary>
        /// Kills ant
        /// </summary>
        /// <param name="deadAnt"></param>
        /// <returns></returns>
        public bool KillAnt(AntBase deadAnt)
        {
            return true;
        }
        
        /// <summary>
        /// spawns ant in map on top of block at x,z coords
        /// returns false if space already occupied
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool SpawnAnt(int x, int z, int height, bool isQueen)
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

            //check if space already occupied
            if(Physics.SphereCast(antCheckRay, 1.0f, out antCheckHit))
                if (antCheckHit.collider.tag.Equals("Ant") || antCheckHit.collider.tag.Equals("Queen"))
                    return false;

            if (isQueen)
            {
                GameObject obj = Instantiate<GameObject>(QueenPrefab, boxMarch, Quaternion.identity);
                _queen = obj.GetComponent<AntBase>();
                _queen.SetIsQueen(true);
                _queen.SetInitPos(boxMarch);
                return true;
            }
            else
            {
                //add ant to list
                GameObject newObj = Instantiate<GameObject>(AntPrefab,boxMarch,
                    Quaternion.identity);

                if (!newObj)
                    return false;
                AntBase antBase = newObj.GetComponent<AntBase>();
                antBase.SetInitPos(boxMarch);
                
                _antList.Add(antBase);
              
            
                return true;
            }

            return false;


        }

        #endregion
    }
}