using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    class Command
    {
        internal static Command GetCommand(string[] args)
        {
            if (args.Length==0) {return new Command(); }
            var returnVar = new Command { Name = args[0] };
            if (args.Length==1)
            {
                returnVar.Instructions = new Instruction[0];
                return returnVar;
            }
            var instructionPositions = new List<int>(args.Length-1);
            int i=1;
            for (;i<args.Length;i++)
            {
                if (args[i][0] == '-')
                {
                    instructionPositions.Add(i);
                }
            }
            if (!instructionPositions.Any())
            {
                returnVar.Instructions = new Instruction[] { new Instruction { OptionValues =  args }};
            }
            instructionPositions.Add(i);
            int firsti;
            int lasti = instructionPositions.Count - 1;
            if (instructionPositions[0]==1)
            {
                returnVar.Instructions = new Instruction[lasti];
                firsti = 0;
            }
            else
            {
                returnVar.Instructions = new Instruction[instructionPositions.Count];
                returnVar.Instructions[0] = new Instruction
                {
                    OptionValues = args.SubArray(0, instructionPositions[0] - 1)
                };
                firsti = 1;
            }
            for (i = firsti; i < lasti; i++)
            {
                int ipos = i - firsti;
                returnVar.Instructions[i] = new Instruction
                {
                    OptionName = args[instructionPositions[ipos]].Substring(1),
                    OptionValues = args.SubArray(instructionPositions[ipos]+1,instructionPositions[ipos + 1] - 1)
                };
            }
            return returnVar;
        }
        internal string Name { get; set; }
        internal IList<Instruction> Instructions { get; set; }
    }
    class Instruction
    {
        internal string OptionName { get; set; }
        internal string[] OptionValues { get; set; }
    }
}
