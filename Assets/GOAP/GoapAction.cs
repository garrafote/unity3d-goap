using System.Security.Policy;

namespace GOAP
{
    public struct GoapAction
    {

        public int Cost;
        public WorldState Preconditions;
        public WorldState Postconditions;
        public string Name;


    }
}