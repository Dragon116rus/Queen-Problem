using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace bdd_interface
{
    class BDDSolver 
    {
        public delegate void ToCheckResult(int n);
        public Process proccess(int n)
        {
            Process proc;
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.WorkingDirectory = @"solvers";
            procInfo.FileName = @"cudd.exe";
            procInfo.Arguments = n.ToString();
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc = Process.Start(procInfo);
            
            return proc;
        }
        public delegate void ResultHandler(int countOfSolve, string pathOfSolve,int n);
        public event ResultHandler gettedCount;

      


        private void checkResults(int n)
        {
            
            string pathToSolves = "solvers/cudd_solves.txt";
            string pathToCountOfSolves = "solvers/cudd_countOfSolves.txt";
            string pathIsFinish = "solvers/cudd_finished.txt";
            int countOfSolve=-1;
            while (true)
            {
                if (File.Exists(pathIsFinish))
                {
                    using (StreamReader sr=new StreamReader(pathToCountOfSolves))
                    {
                        var s=sr.ReadLine();
                        if (int.TryParse(s,out countOfSolve))
                        {
                        //    gettedCount(countOfSolve, pathToSolves, (int)n);
                            Thread.Sleep(100);
                            File.Delete(pathIsFinish);
                            return;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
