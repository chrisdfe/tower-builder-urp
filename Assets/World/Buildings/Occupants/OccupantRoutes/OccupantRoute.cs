using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantRoute
    {
        public List<OccupantRouteNode> path = new();

        public OccupantRoute(List<OccupantRouteNode> path)
        {
            this.path = path;
        }
    }
}
