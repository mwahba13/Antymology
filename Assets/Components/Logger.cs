using System;
using System.IO;
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
        private StreamWriter _weightWriter;

        private float[][][] _bestWeights;


        public void Start()
        {


            _antWriter = new StreamWriter(Application.dataPath + "\\AntTracker.csv");
            _antWriter.WriteLine("Tick,AntID,MvFwd,MvBck,MvRit,MvLft,Eat,Dig,Hlth,Bld,Nil,Action,Fitness");
        }
        

        public void LogAntWeights(int tick, AntBase ant, float[] nnOut, EAction action)
        {
            _antWriter.WriteLine(tick 
                                 + "," + ant.GetAntID() 
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
                                 + "," + ant.CalculateFitnessFunction()
                                 );
        }


        

        public void WriteAntvolution(AntBase topAnt, AntBase secAnt)
        {
            _antWriter.WriteLine(" ");
            _antWriter.WriteLine("ANTVOLUTION");
            _antWriter.WriteLine("Ant: " + topAnt.GetAntID() + " mated with " + "Ant: " + secAnt.GetAntID());
            _antWriter.WriteLine(" ");
            
        }


        public void WriteBestWeights(float[][][] weights)
        {
            _weightWriter = new StreamWriter(Application.dataPath + "\\BestWeight.txt");
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

        public float[][][] ReadBestWeight()
        {
            _weightReader = new StreamReader(Application.dataPath + "\\BestWeight.txt");
            
            for (int i = 0; i < _bestWeights.Length;i++)
            {
                for (int j = 0; j < _bestWeights[i].Length; j++)
                {
                    for (int k = 0; k < _bestWeights[i][j].Length; k++)
                    {
                        _bestWeights[i][j][k] = float.Parse(_weightReader.ReadLine());
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


        }
        
    }
}