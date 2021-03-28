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


        private StreamWriter _antWriter;




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
        


        public void OnApplicationQuit()
        {
            _antWriter.Close();

        }
        
    }
}