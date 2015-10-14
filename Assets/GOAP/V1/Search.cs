using PathFinder;
using System.Collections.Generic;

namespace GOAP.V1
{
    public class Graph : ISearchSpace
    {
        // This is our heuristic: estimate for remaining distance is the nr of mismatched atoms that matter.
        public static float PlannerHeuristic(INode from, INode to)
        {
            var wsFrom = ((WorldStateNode)from).State;
            var wsTo = ((WorldStateNode)to).State;

            var mask = wsTo.Mask;
            var diff = ((wsFrom.Values & mask) ^ (wsTo.Values & mask));

            var distance = 0;
            while (diff > 0)
            {
                // right most bit is 1
                if (diff % 2 == 1)
                {
                    distance++;
                }

                diff >>= 1;
            }

            return distance;
        }
    }


    public class WorldStateNode : INode
    {
        public readonly ActionPlanner Planner;
        public WorldState State;

        public WorldStateNode(ActionPlanner planner, WorldState state)
        {
            Planner = planner;
            State = state;
        }

        public float Weight { get { return 1; } }

        public IEnumerable<NodeConnection> Connections
        {
            get
            {
                return Planner.GetPossibleTransitions(this);
            }
        }

        public bool Equals(INode other)
        {
            var wsOther = ((WorldStateNode)other).State;
            return State.Match(wsOther);
        }
    }
}