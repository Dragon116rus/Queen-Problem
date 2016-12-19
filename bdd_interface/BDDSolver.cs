using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        public delegate void ResultHandler(int countOfSolve, string pathOfSolve, int n);
        public event ResultHandler gettedCount;



       
       
    }
}
