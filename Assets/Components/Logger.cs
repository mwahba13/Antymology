using System;
using System.IO;
using Components.Ant;
using UnityEngine;

namespace Components
{
    
    
    //TODO: serialize best weights into a JSON
    /// <summary>
    /// Class for writing values of our simulation to a csv file
    /// </summary>
    public class Logger : MonoBehaviour
    {


        private StreamWriter _antWriter;
        private StreamWriter _eventWriter;

        public void Start()
        {
            //_eventWriter = new StreamWriter(Application.dataPath + "\\EventData.csv");
           // _eventWriter.WriteLine("Tick,Event");
            
            _antWriter = new StreamWriter(Application.dataPath + "\\AntTracker.csv");
            _antWriter.WriteLine("Tick,AntID,MvFwd,MvBck,MvRit,MvLft,Eat,Dig,Hlth,Bld,Nil,Action,Fitness");
        }

        public void LogAnt(int generation, AntBase ant)
        {
            //_antWriter.WriteLine(generation + "," + ant.GetAntID()+","+ant.GetFitnessFunction());
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

        public void WriteAntvolution()
        {
            _antWriter.WriteLine("ANTVOLUTION");
        }
        

        public void writeAntWeights()
        {
        }

        public void OnApplicationQuit()
        {
            _antWriter.Close();
        }
        
    }
}