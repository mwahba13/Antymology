using System;
using System.Collections.Generic;
using Antymology.Terrain;
using Components.Ant;
using Components.Terrain.Blocks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
        public Logger _logger;


        //whether to intialize ants with random weights, or last best weights in JSON file
        public bool startWithRandomWeights;

        [SerializeField]
        private float _tickTimer;
        private float worldHeight;
        private float worldDimensions;
        
        private int _generation = 0;
        private int _totalTicks = 0;
        [SerializeField]
        private int _ticksUntilEvolution;

        private UI _ui;
        private List<GameObject> _antList = new List<GameObject>();
        private List<Vector3> _nestBlockList = new List<Vector3>();
        private AntBase _queen;
        
        
        //weights for fitness funciton calculations
        public float _donateHealthToQueenWeight = 10.0f;
        public float _distToQueenWeight = -1.0f;
        public float _healthWeight = 1.0f;
        public float _blockBuildWeight = 10.0f;
        public float _queenHealthWeight = 5.0f;
        public float _distFromStartWeight = 1.0f;
        
        

        #endregion

        #region Methods

        private void Start()
        {
            _ui = GetComponent<UI>();
            
            _tickTimer = 0.0f;
            _ticksUntilEvolution = simSettings.TicksUntilEvolution;

            Debug.Log(Application.dataPath);
            _logger = GetComponent<Logger>();


        }


        private void Update()
        {
            
            _tickTimer -= Time.deltaTime;
            if (_tickTimer < 0.0f)
            {
                TickAnts();
                _tickTimer = simSettings.SimulationTickInterval;
                _totalTicks++;

                _ticksUntilEvolution--;
                if(_ticksUntilEvolution <= 0)
                    Antvolution();
                
            }
            
        }


        
        public void GenerateAnts(int numAnts,int dimensions,int height)
        {
            simSettings.NumberOfAnts = numAnts;
            worldHeight = height;
            worldDimensions = dimensions;
            RandomAntGenerator(numAnts,dimensions,height);

            //set an arbitrary best weight so we dont get null errors
            _logger.SetBestWeight(_antList[0].GetComponent<AntBase>().GetWeights());
            
            //sets weights (if we arent creating new ones)
            if (!startWithRandomWeights && _logger.TryReadWeight())
            {
                foreach (var ant in _antList)
                {
                    ant.GetComponent<AntBase>().SetWeight(_logger.ReadBestWeight());
                }
            }
            


            //PerlinNoiseAntGenerator(NumAnts,dimensions,height);
            //SpawnQueen(height,dimensions);
        }

        private void Antvolution()
        {
            //we get the top two fitness values and do some reproducing
            AntBase topAnt = null;
            AntBase secondAnt = null;

            float bestFit = -1;
            

            
            //get top ant
            foreach (var ant in _antList)
            {
                AntBase antBase = ant.GetComponent<AntBase>();
                float topAntFitness = antBase.CalculateFitnessFunction();
                if (topAntFitness > bestFit)
                {
                    bestFit = topAntFitness;
                    topAnt = antBase;
                    _ui.SetTopAntField(topAntFitness);
                }
            }
            

            bestFit = -1;
            //get second top ant
            foreach (var ant in _antList)
            {
                AntBase antBase = ant.GetComponent<AntBase>();
                float secAntFit = antBase.CalculateFitnessFunction();
                if (secAntFit > bestFit && !antBase.Equals(topAnt))
                {
                    bestFit = secAntFit;
                    secondAnt = antBase;
                }
            }
            
            _logger.WriteAntvolution(topAnt,secondAnt);

            float[][][] childWeights = SexualHealing(topAnt, secondAnt);
  

            //set new weights of the children ants and mutate them
            foreach (var antObj in _antList)
            {
                AntBase ant = antObj.GetComponent<AntBase>();
                
                ant.SetWeight(childWeights);
                //ant.ResetStats();
                if(Random.Range(0.0f,1.0f) < simSettings.ProbabilityOfMutation)
                    ant.MutateWeights(simSettings.MutationPercentage);
                //_logger.LogAnt(_generation,ant);
            }
            
            
            //create a bunch of new ants to keep teh bloodlines goin
            RandomPeasantGenerator(simSettings.NumberOfAnts-_antList.Count,childWeights);

            //write downt hte best weight so we can keep iterating on it.
            //simSettings.bestWeight = childWeights;
            _logger.WriteBestWeights(childWeights);


            //childWeights = SexualHealing(_queen, _antList[Random.Range(0, _antList.Count - 1)].GetComponent<AntBase>());
            
            if(_queen)
                MutateQueenWeights();
            
            _ticksUntilEvolution = simSettings.TicksUntilEvolution;
            _generation++;
            
            _ui.SetGenerationField(_generation);
        }


        private void ResetSimulation(Vector3 QueenTrans)
        {
            _generation = 0;
            
            //reset world
            DestroyAllNestBlocks();
            
            //make sure queen is not standing on top of old block positions
            
            Destroy(_queen.gameObject);

            SpawnAnt((int) QueenTrans.x, (int) QueenTrans.y, (int)worldHeight, true, 420);
            MutateQueenWeights();
           
            
            
            
            foreach(var ant in _antList)
                ant.GetComponent<AntBase>().ResetStats();
            _queen.ResetStats();

            
            //create new ants to replace the dead ones
            
            
        }
        

        private void MutateQueenWeights()
        {
            if (_queen)
            {
                AntBase queenBase = _queen.GetComponent<AntBase>();
            
                float mutationRange = queenBase.CalculateFitnessFunction();
                _ui.SetQueenFitField(mutationRange);
                mutationRange /= 100.0f;
            
                queenBase.MutateWeights(mutationRange);   
            }
            

            

        }

        /*When I get that feeling
         *I want sexual healing
         *Sexual healing, oh baby
         *Makes me feel so fine
         */
        private float[][][] SexualHealing(AntBase Mommy, AntBase Daddy)
        {
            float[][][] outWeights = Mommy.GetWeights();
            float[][][] mommyWeights = Mommy.GetWeights();
            float[][][] daddyWeights = Daddy.GetWeights();

            for (int i = 0; i < mommyWeights.Length; i++)
            {
                for (int j = 0; j < mommyWeights[i].Length; j++)
                {
                    for (int k = 0; k < mommyWeights[i][j].Length; k++)
                    {
                        float insert;
                        float randFloat = Random.Range(0.0f, 1.0f);
                        //crossover
                        if (randFloat > 0.5f)
                            insert = mommyWeights[i][j][k];
                        else
                            insert = daddyWeights[i][j][k];
/*
                        if (randFloat > simSettings.ProbabilityOfMutation)
                            insert = Random.Range(insert - (insert * simSettings.MutationPercentage),
                                insert + (insert * simSettings.MutationPercentage));
*/                      
                        outWeights[i][j][k] = insert;

                    }
                }
            }
            
            //return crossovered genome
            return outWeights;
        }

        private void TickAnts()
        {
            
            foreach (var antObj in _antList)
            {
                AntBase ant = antObj.GetComponent<AntBase>();
                if (ant)
                {
                    if(ant.GetHealth() <= 0.0f)
                        KillAnt(ant);
                    if(ant)
                        ant.Tick();                    
                }

            }

            if (_queen)
            {
                if(_queen.GetHealth() <= 0.0f)
                    KillAnt(_queen);
                if(_queen)
                    _queen.Tick();                
            }


            
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

                if (SpawnAnt(tempX, tempZ, height, false,i))
                    i++;
            }
            
            //spawn queen
            bool isQueenSpawned = false;

            while (!isQueenSpawned)
            {
                int tempX = Random.Range(1, dimensions - 1);
                int tempZ = Random.Range(1, dimensions - 1);

                if (SpawnAnt(tempX, tempZ, height, true,420))
                    isQueenSpawned = true;
            }
            
        }

        private void RandomPeasantGenerator(int numAnts,float[][][] childWeights)
        {
            int i = 0;
            //spawn peasant ants
            while (i < numAnts - 1)
            {
                int tempX = Random.Range(1, (int)worldDimensions - 1);
                int tempZ = Random.Range(1, (int)worldDimensions - 1);
                
                

                if (SpawnAnt(tempX, tempZ,(int) worldHeight, false,i,childWeights))
                    i++;
            }
        }

        private void RandomQueenGenerator(float[][][] childWeights)
        {
            bool isQueenSpawned = false;

            while (!isQueenSpawned)
            {
                int tempX = Random.Range(1, (int)worldDimensions - 1);
                int tempZ = Random.Range(1, (int)worldDimensions - 1);

                if (SpawnAnt(tempX, tempZ, (int)worldHeight, true,420,childWeights))
                    isQueenSpawned = true;
            }
        }

        /// <summary>
        /// Spawns ants in map based on perlin noise. One ant randomly chosen to be queen 
        /// </summary>
        /// <param name="numAnts"></param>
        private void PerlinNoiseAntGenerator(int numAnts,int dimensions,int height,int antID)
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
                           
                            if (SpawnAnt(x-randInt1, z-randInt2, height,false,antID))
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

        public float GetWorldHeight()
        {
            return worldHeight;
            
        }

        public float GetWorldDimension()
        {
            return worldDimensions;
        }
        
        
        public void AddNestBlockToList(Vector3 transform)
        {
            _nestBlockList.Add(transform);
            _ui.SetNestBlockField(_nestBlockList.Count);
        }

        public void DestroyAllNestBlocks()
        {
            AbstractBlock airBlock = new AirBlock();
            
            foreach (Vector3 block in _nestBlockList)
            {
                WorldManager.Instance.SetBlock((int)block.x,(int)block.y,(int)block.z,airBlock);
            }
            
            _nestBlockList.Clear();
            
            _ui.SetNestBlockField(0.0f);
        }
        
     

        public float GetQueenHealth()
        {
            if (!_queen)
                return 0.0f;
            return _queen.GetHealth();
        }
        
        public Vector3 GetQueenLocation()
        {
            if(!_queen)
                return Vector3.zero;
            return _queen.transform.position;
            
        }

        public void LogAntActions(AntBase ant, float[] nnOut, EAction action)
        {
            _logger.LogAntWeights(_totalTicks,ant,nnOut,action);
        }


        /// <summary>
        /// Kills ant
        /// </summary>
        /// <param name="deadAnt"></param>
        /// <returns></returns>
        public void KillAnt(AntBase deadAnt)
        {
            if (deadAnt.GetIsQueen())
            {
                Debug.Log("THE QUEEN IS DEAD; LONG LIVE THE QUEEN");
                ResetSimulation(deadAnt.transform.position);
            }
            
            _antList.Remove(deadAnt.gameObject);
            Destroy(deadAnt.gameObject);
        }
        
        /// <summary>
        /// spawns ant in map on top of block at x,z coords
        /// returns false if space already occupied
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool SpawnAnt(int x, int z, int height, bool isQueen,int antID)
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
                obj.name = "Queen" + antID;
                _queen = obj.GetComponent<AntBase>();
                _queen.SetAntID(antID);
                _queen.SetIsQueen(true);
                _queen.SetInitPos(boxMarch);
                return true;
            }
            else
            {
                //add ant to list
                GameObject newObj = Instantiate<GameObject>(AntPrefab,boxMarch,
                    Quaternion.identity);

                newObj.name = "Ant" + antID;
                if (!newObj)
                    return false;
                AntBase antBase = newObj.GetComponent<AntBase>();
                antBase.SetInitPos(boxMarch);
                antBase.SetAntID(antID);
                _antList.Add(antBase.gameObject);
              
            
                return true;
            }

            return false;


        }
        
        private bool SpawnAnt(int x, int z, int height, bool isQueen,int antID,float[][][] childWeights)
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
                obj.name = "Queen" + antID;
                _queen = obj.GetComponent<AntBase>();
                _queen.SetWeight(childWeights);
                _queen.SetAntID(antID);
                _queen.SetIsQueen(true);
                _queen.SetInitPos(boxMarch);
                return true;
            }
            else
            {
                //add ant to list
                GameObject newObj = Instantiate<GameObject>(AntPrefab,boxMarch,
                    Quaternion.identity);

                newObj.name = "Ant" + antID;
                if (!newObj)
                    return false;
                AntBase antBase = newObj.GetComponent<AntBase>();
                antBase.SetInitPos(boxMarch);
                antBase.SetWeight(childWeights);
                antBase.SetAntID(antID);
                _antList.Add(antBase.gameObject);
              
            
                return true;
            }

            return false;


        }

        #endregion
    }
}