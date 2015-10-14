using PathFinder;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace GOAP
{
    public class GoapGraph : ISearchSpace
    {
        private ActionPlanner planner;

        public GoapGraph(ActionPlanner planner)
        {
            this.planner = planner;
        }


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

        public IEnumerable<NodeConnection> GetPossibleTransitions(INode from)
        {
            var wsFrom = ((WorldStateNode)from).State;

            var result = new List<NodeConnection>();

            foreach (var action in planner.Actions)
            {
                var pre = action.Preconditions;
                var mask = pre.Mask;

                var met = (pre.Values & mask) == (wsFrom.Values & mask);
                if (met)
                {
                    var neighbour = planner.PerformAction(action, wsFrom);

                    if (wsFrom.Match(neighbour))
                    {
                        continue;
                    }

                    var connection = new NodeConnection();
                    connection.From = from;
                    connection.To = new WorldStateNode(this, neighbour);
                    connection.Cost = action.Cost;

                    result.Add(connection);
                }
            }

            return result;
        }
    }


    public class WorldStateNode : INode
    {
        public readonly GoapGraph Graph;
        public WorldState State;

        public WorldStateNode(GoapGraph graph, WorldState state)
        {
            Graph = graph;
            State = state;
        }

        public float Weight { get { return 1; } }

        public IEnumerable<NodeConnection> Connections
        {
            get
            {
                return Graph.GetPossibleTransitions(this);
            }
        }

        public bool Equals(INode other)
        {
            var wsOther = ((WorldStateNode)other).State;
            return State.Match(wsOther);
        }
    }
}