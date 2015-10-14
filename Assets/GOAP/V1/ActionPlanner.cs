using System;
using System.Runtime.Remoting.Messaging;
using System.Text;
using PathFinder;
using System.Collections.Generic;

namespace GOAP.V1
{
    public class ActionPlanner
    {
        public const short MaxAtoms = sizeof(long) * 8;
        public const short MaxActions = sizeof(long) * 8;
        
        //private string[] atomNames = new string[MaxActions];
        //private string[] actionNames = new string[MaxActions];

        // create action / atom classes?
        private WorldState[] preconditions = new WorldState[MaxActions];
        private WorldState[] postconditions = new WorldState[MaxActions];
        private int[] costs = new int[MaxActions];

        public void Clear()
        {

            // numAtoms = 0;
            // numActions = 0;

            // clear names?

            for (var i = 0; i < MaxActions; i++)
            {
                preconditions[i].Clear();
                postconditions[i].Clear();
                costs[i] = 0;
            }

        }

        public bool SetPrecondition(Enum actionIndex, Enum atomIndex, bool value)
        {
            return SetPrecondition(Convert.ToInt16(actionIndex), Convert.ToInt16(atomIndex), value);
        }

        public bool SetPostcondition(Enum actionIndex, Enum atomIndex, bool value)
        {
            return SetPostcondition(Convert.ToInt16(actionIndex), Convert.ToInt16(atomIndex), value);
        }

        public bool SetCost(Enum actionIndex, int cost)
        {
            return SetCost(Convert.ToInt16(actionIndex), cost);
        }

        public bool SetPrecondition(short actionIndex, short atomIndex, bool value)
        {
            if (costs[actionIndex] == 0)
                costs[actionIndex] = 1;
            return preconditions[actionIndex].Set(atomIndex, value);
        }

        public bool SetPostcondition(short actionIndex, short atomIndex, bool value)
        {
            if (costs[actionIndex] == 0)
                costs[actionIndex] = 1;
            return postconditions[actionIndex].Set(atomIndex, value);
        }

        public bool SetCost(short actionIndex, int cost)
        {
            costs[actionIndex] = cost;

            return true;
        }

        public IEnumerable<NodeConnection> GetPossibleTransitions(INode from)
        {
            var wsFrom = ((WorldStateNode)from).State;

            var result = new List<NodeConnection>();

            for (short i = 0; i < 10; i++)
            {
                var pre = preconditions[i];
                var mask = pre.Mask;

                var met = (pre.Values & mask) == (wsFrom.Values & mask);
                if (met)
                {
                    var neighbour = PerformAction(i, wsFrom);

                    if (wsFrom.Match(neighbour))
                    {
                        continue;
                    }

                    var connection = new NodeConnection();
                    connection.From = from;
                    connection.To = new WorldStateNode(this, neighbour);
                    connection.Cost = costs[i];

                    result.Add(connection);
                }
            }

            return result;
        }

        public WorldState PerformAction(short actionIndex, WorldState from)
        {
            var result = new WorldState();
            
            var postcondition = postconditions[actionIndex];
            var affected = postcondition.Mask;
            var unaffected = affected ^ -1L;

            result.Values = (from.Values & unaffected) | (postcondition.Values & affected);
            result.Mask = from.Mask | postcondition.Mask; 

            return result;
        }

        public string ToString<TActions, TAtoms>()
        {
            var sb = new StringBuilder();

            var tActions = typeof (TActions);
            var actions = Enum.GetNames(tActions);
            
            var tAtoms = typeof (TAtoms);
            var atoms = Enum.GetNames(tAtoms);

            foreach (var actionName in actions)
            {
                var actionIndex = Convert.ToInt16(Enum.Parse(tActions, actionName));

                sb.AppendFormat("{0} [cost: {1}]:\n", actionName, costs[actionIndex]);

                var pre = preconditions[actionIndex];
                var post = postconditions[actionIndex];

                sb.AppendLine(pre.ToString<TAtoms>("{0} == {1}\n"));
                sb.AppendLine(post.ToString<TAtoms>("{0} := {1}\n"));

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString<GOAPTestV1.Actions, GOAPTestV1.Atoms>();
        }
    }
}
