using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    class Instruction
    {
        internal static IEnumerable<Instruction> GetInstructions(string[] args)
        {
            if (args.Length==0) {return new Instruction[0]; }
            var currentInstruction = new Instruction();
            var instructionPositions = new List<int>(args.Length);
            int i=0;
            for (;i<args.Length;i++)
            {
                if (args[i][0] == '-')
                {
                    instructionPositions.Add(i);
                }
            }
            if (!instructionPositions.Any())
            {
                return new Instruction[] { new Instruction { OptionValues = string.Join(" ", args) } };
            }
            instructionPositions.Add(i);
            Instruction[] returnVar;
            int firsti;
            int lasti = instructionPositions.Count - 1;
            if (instructionPositions[0]==0)
            {
                returnVar = new Instruction[lasti];
                firsti = 0;
            }
            else
            {
                returnVar = new Instruction[instructionPositions.Count];
                returnVar[0] = new Instruction
                {
                    OptionValues = string.Join(" ", args.SubArray(0, instructionPositions[0] - 1))
                };
                firsti = 1;
            }
            for (i = firsti; i < lasti; i++)
            {
                int ipos = i - firsti;
                returnVar[i] = new Instruction
                {
                    OptionName = args[instructionPositions[ipos]].Substring(1),
                    OptionValues = string.Join(" ", args.SubArray(instructionPositions[ipos]+1,instructionPositions[ipos + 1] - 1))
                };
            }
            return returnVar;
        }
        internal string OptionName { get; set; }
        internal string OptionValues { get; set; }
    }
}
