using System;
using System.Runtime.Remoting.Messaging;
using System.Text;
using PathFinder;
using System.Collections.Generic;

namespace GOAP
{
    public class ActionPlanner
    {
        public const short MaxAtoms = sizeof(long) * 8;
        public const short MaxActions = sizeof(long) * 8;
        
        //private string[] atomNames = new string[MaxActions];
        //private string[] actionNames = new string[MaxActions];

        public HashSet<GoapAction> Actions = new HashSet<GoapAction>();
        public List<string> Atoms = new List<string>(); 

        public void Clear()
        {
            Actions.Clear();
            Atoms.Clear();
        }

        public short StringToAtom(string atom)
        {
            var atomIndex = Atoms.IndexOf(atom);
            if (atomIndex >= 0)
                return (short)atomIndex;

            atomIndex = Atoms.Count;
            Atoms.Add(atom);

            return (short)atomIndex;
        }

        public string AtomToString(short atomIndex)
        {
            return atomIndex < Atoms.Count ? Atoms[atomIndex] : null;
        }

        public bool? SetPrecondition(GoapAction goapAction, string atom, bool? value)
        {
            var atomIndex = StringToAtom(atom);

            return goapAction.Preconditions[atomIndex] = value;
        }

        public bool? SetPostcondition(GoapAction goapAction, string atom, bool? value)
        {
            var atomIndex = StringToAtom(atom);

            return goapAction.Preconditions[atomIndex] = value;
        }

    

        public WorldState PerformAction(GoapAction goapAction, WorldState from)
        {
            var result = new WorldState();
            
            var postcondition = goapAction.Preconditions;
            var affected = postcondition.Mask;
            var unaffected = affected ^ -1L;

            result.Values = (from.Values & unaffected) | (postcondition.Values & affected);
            result.Mask = from.Mask | postcondition.Mask; 

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var action in Actions)
            {
                sb.AppendFormat("{0} [cost: {1}]:\n", action.Name, action.Cost);

                var pre = action.Preconditions;
                var post = action.Postconditions;

                sb.AppendLine(pre.ToString(Atoms, "{0} == {1}\n"));
                sb.AppendLine(post.ToString(Atoms, "{0} := {1}\n"));

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
