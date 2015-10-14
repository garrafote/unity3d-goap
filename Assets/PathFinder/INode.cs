using System;
using System.Collections.Generic;

namespace PathFinder
{
    public interface INode : IEquatable<INode>
    {
        float Weight { get; }

        IEnumerable<NodeConnection> Connections { get; }
    }
}