using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bdd_interface
{
    public abstract class  Solver
    {
       
        public abstract Tuple<Process, Thread> proccess(int n);
    }
}
