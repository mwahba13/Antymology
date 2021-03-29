using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Components.Ant;
using UnityEngine;

namespace Components
{
    
    
    /// <summary>
    /// Class for writing values of our simulation to a csv file
    /// </summary>
    public class Logger : MonoBehaviour
    {

        private StreamReader _weightReader;

        private StreamWriter _antWriter;
        private StreamWriter _genWriter;
        private StreamWriter _weightWriter;

        private float[][][] _bestWeights;


        private List<float> _avgAntFitness = new List<float>();
        

        public void Start()
        {
            _genWriter = new StreamWriter(Application.dataPath + "\\GenerationalData.csv");
            _genWriter.WriteLine("Epoch,Generation,Top Ant Fitness, Queen Fitness, Blocks Built");

            _antWriter = new StreamWriter(Application.dataPath + "\\AntTracker.csv");
            _antWriter.WriteLine("Tick,AntID,Health,MvFwd,MvBck,MvRit,MvLft,Eat,Dig,Hlth,Bld,Nil,Action,Fitness");
        }
        

        public void LogAntWeights(int tick, AntBase ant, float[] nnOut, EAction action)
        {
            float antFitness = ant.CalculateFitnessFunction();
            _avgAntFitness.Add(antFitness);
            
            _antWriter.WriteLine(tick
                                 + "," + ant.GetAntID()
                                 + "," + ant.GetHealth()
                                 + "," + nnOut[0] 
                                 + "," + nnOut[1]
                                 + "," + nnOut[2]
                                 + "," + nnOut[3]
                                 + "," + nnOut[4]
                                 + "," + nnOut[5]
                                 + "," + nnOut[6]
                                 + "," + nnOut[7]
                                 + "," + nnOut[8]
                                 + "," + action.ToString()
                                 + "," + antFitness
                                 );
            
        }


        

        public void LogNewGeneration(AntBase topAnt, AntBase secAnt, AntBase queen, 
            float generation, float epoch, int blocksBuilt)
        {
            _antWriter.WriteLine(" ");
            _antWriter.WriteLine("ANTVOLUTION -- GENERATION: " + generation );
            _antWriter.WriteLine("Ant: " + topAnt.GetAntID() + " mated with " + "Ant: " + secAnt.GetAntID());
            _antWriter.WriteLine("Average Ant Fitness: " + _avgAntFitness.Average());
            _antWriter.WriteLine(" ");


            _genWriter.WriteLine(epoch
                       + "," + generation
                       + "," + topAnt.CalculateFitnessFunction()
                       + "," + queen.CalculateFitnessFunction()
                       + "," + blocksBuilt
                                );

        }
        
        public void LogNewEpoch(float epochNum,float genNum , int blocksBuilt )
        {
            _antWriter.WriteLine(" ");
            _antWriter.WriteLine("Hark! The sun has set on the formics, let the new dawn be a kinder one");
            _antWriter.WriteLine("Epoch number " + epochNum + " has ended after " + genNum + " generations");
            _antWriter.WriteLine(blocksBuilt + " Blocks have been built. Now they turn to dust...");
            _antWriter.WriteLine(" ");
            
            /*
            Debug.Log("Hark! The sun has set on the formics, let the new dawn be a kinder one");
            Debug.Log("Epoch number " + epochNum + " has ended.");
            Debug.Log(blocksBuilt + " Blocks have been built. Now they turn to dust...");
            */
        } 
        

        public void WriteBestWeights(float[][][] weights)
        {
            try
            {
                _weightWriter = new StreamWriter(Application.dataPath + "\\BestWeight.txt");
                _bestWeights = weights;
                for (int i = 0; i < weights.Length;i++)
                {
                    for (int j = 0; j < weights[i].Length; j++)
                    {
                        for (int k = 0; k < weights[i][j].Length; k++)
                        {
                            _weightWriter.WriteLine(weights[i][j][k]);
                        }
                    }
                }
                _weightWriter.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                
            }

        }

        public float[][][] ReadBestWeight()
        {
            _weightReader = new StreamReader(Application.dataPath + "\\BestWeight.txt");
            
            for (int i = 0; i < _bestWeights.Length;i++)
            {
                for (int j = 0; j < _bestWeights[i].Length; j++)
                {
                    for (int k = 0; k < _bestWeights[i][j].Length; k++)
                    {
                        float value = float.Parse(_weightReader.ReadLine());
                        _bestWeights[i][j][k] = value;
                    }
                }
            }
            
            _weightReader.Close();
            return _bestWeights;
        }



        public float[][][] GetBestWeight()
        {
            return _bestWeights;
        }

        public void SetBestWeight(float[][][] newWeight)
        {
            _bestWeights = newWeight;
        }

        public bool TryReadWeight()
        {

            try
            {
                _weightReader = new StreamReader(Application.dataPath + "\\BestWeight.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
            return true;
        }

        public void OnApplicationQuit()
        {
            _antWriter.Close();
            _genWriter.Close();

        }
        
    }
}