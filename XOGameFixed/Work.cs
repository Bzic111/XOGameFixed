using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuSpace;

public abstract class Work
{
    public Menu.Runner[] AllRuns { get; }
    string[] Names { get; }
    public virtual string[] GetNames() => this.Names;
    public virtual Menu.Runner[] GetRunners() => AllRuns;
}