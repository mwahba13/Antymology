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


        private StreamWriter _writer;

        public void Start()
        {
            _writer = new StreamWriter(Application.dataPath + "\\NNData.csv");
            _writer.WriteLine("Generation,AntID");
        }

        public void LogAnt(int generation, AntBase ant)
        {
            
        }

        public void writeAntWeights()
        {
        }

        public void OnApplicationQuit()
        {
            _writer.Close();
        }
        
    }
}