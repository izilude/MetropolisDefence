using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.RTSCore.AI;
using Assets.RTSCore.Level;
using Assets.RTSCore.Map;

namespace Assets.RTSCore.CustomEngine
{
    public class EngineTaskGetPath : IEngineTask
    {
        public List<Tile> PossibleStartTiles;
        public List<Tile> PossibleEndTiles;

        public void Run()
        {
            List<Tile> bestPath = null;
            float bestScore = 0;

            foreach (var tile in PossibleStartTiles)
            {
                List<Tile> path;
                float score;
                AStarPathFinding.FindPathBetweenTwoTiles(tile, PossibleEndTiles, out path, out score);

                if (bestPath == null || score < bestScore)
                {
                    bestPath = path;
                    bestScore = score;
                }
            }
        }
    }
}
