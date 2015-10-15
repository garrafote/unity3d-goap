using System;
using System.Runtime.Remoting.Messaging;
using System.Text;
using PathFinder;
using System.Collections.Generic;

namespace GOAP
{
    public class ActionPlanner
    {
        public class Action
        {
            public int Cost;
            public WorldState Preconditions;
            public WorldState Postconditions;
            public string Name;
            public ActionPlanner Planner;

            public void SetPrecondition(string atom, bool? value)
            {
                var atomIndex = Planner.StringToAtom(atom);

                Preconditions[atomIndex] = value;
            }


            public void SetPostcondition(string atom, bool? value)
            {
                var atomIndex = Planner.StringToAtom(atom);

                Postconditions[atomIndex] = value;
            }
        }

        public const short MaxAtoms = sizeof(long) * 8;
        public const short MaxActions = sizeof(long) * 8;
        
        public HashSet<Action> Actions = new HashSet<Action>();
        public List<string> Atoms = new List<string>(); 

        public void Clear()
        {
            Actions.Clear();
            Atoms.Clear();
        }

        public Action CreateAction(string name, int cost = 1)
        {
            var action = new Action {
                Cost = cost,
                Preconditions = default(WorldState),
                Postconditions = default(WorldState),
                Name = name,
                Planner = this,
            };

            Actions.Add(action);

            return action;
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

        public WorldState PerformAction(Action goapAction, WorldState from)
        {
            var result = new WorldState();
            
            var postcondition = goapAction.Postconditions;
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

                sb.AppendLine(pre.ToString(Atoms, "{0} == {1}", "\n", "", ""));
                sb.AppendLine(post.ToString(Atoms, "{0} := {1}", "\n", "", ""));

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
