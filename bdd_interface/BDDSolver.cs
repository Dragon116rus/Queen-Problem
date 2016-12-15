using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bdd_interface
{
    class BDDSolver : Solver
    {
        public override Tuple<Process,Thread> proccess(int n)
        {
            Process proc;
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "cudd.exe";
            procInfo.Arguments = n.ToString();
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc = Process.Start(procInfo);
            Thread thread = new Thread(new ThreadStart(checkResults));
            thread.Start();
            return new Tuple<Process,Thread>(proc,thread);
        }
        public delegate void ResultHandler(int countOfSolve, string pathOfSolve);
        public event ResultHandler gettedCount;

        private void checkResults()
        {
            
            string nameOfResult = "bdd_sol.txt";
            string nameOfSolve = "bdd_count.txt";
            pathOfSolve = nameOfSolve;
            int countOfSolve=0;
            while (true)
            {
                if (File.Exists(nameOfResult))
                {
                    using (StreamReader sr=new StreamReader(nameOfSolve))
                    {
                        var s=sr.ReadLine();
                        if (int.TryParse(s,out countOfSolve))
                        {
                            gettedCount(countOfSolve, pathOfSolve);
                            File.Delete(nameOfResult);
                            return;
                        }
                    }
                }

            }
        }
    }
}
