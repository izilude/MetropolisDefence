using System.Collections.Generic;
using System.Linq;
using Assets.RTSCore.Level;
using Assets.RTSCore.Map;
using UnityEngine;

namespace Assets.RTSCore.AI
{
    public static class AStarPathFinding
    {
        public static bool FindPathBetweenTwoTiles(
            Tile startTile, 
            List<Tile> possibleEndTiles, 
            out List<Tile> bestPath, 
            out float pathScore)
        {
            Dictionary<int, Tile> registeredTiles = new Dictionary<int, Tile>();

            bestPath = new List<Tile>();
            if (!IterateOnAStartAlgorithm(
                registeredTiles, 
                new List<TileScore>(), 
                bestPath, 
                new TileScore(), 
                startTile, 
                possibleEndTiles, 
                out pathScore))
            {
                pathScore = 0;
                return false;
            }

            return true;
        }

        private static bool IterateOnAStartAlgorithm(
            Dictionary<int, Tile> registeredTiles,
            List<TileScore> tilesToTry,
            List<Tile> finalPath,
            TileScore previousTileScore, 
            Tile currentTile, 
            List<Tile> possibleEndTiles,
            out float pathScore)
        {
            Tile tile;
            var neighbors = Game.Game.Instance.ActiveLevel.Map.FindSurroundingTiles(currentTile, false);

            if (possibleEndTiles.Any(x => x.Index == currentTile.Index))
            {
                foreach (int index in previousTileScore.PathByIndex)
                {
                    registeredTiles.TryGetValue(index, out tile);
                    finalPath.Add(tile);
                }
                pathScore = previousTileScore.Score;
                return true;
            }

            foreach (var neighbor in neighbors)
            {
                if (!registeredTiles.TryGetValue(neighbor.Index, out tile) && neighbor.Accessible)
                {
                    registeredTiles.Add(neighbor.Index, neighbor);
                    var newTileScore = new TileScore
                    {
                        Index = neighbor.Index,
                        XScore = previousTileScore.XScore + 1/neighbor.SpeedFactor,
                        YScore = (currentTile.transform.position - GetAveragePosition(possibleEndTiles)).magnitude/20.0f
                    };
                    newTileScore.PathByIndex.AddRange(previousTileScore.PathByIndex);
                    newTileScore.PathByIndex.Add(neighbor.Index);
                    tilesToTry.Add(newTileScore);
                }
            }

            if (tilesToTry.Count != 0)
            {
                var nextTile = GetNextTileToTry(tilesToTry);

                registeredTiles.TryGetValue(nextTile.Index, out tile);
                bool endTileFound = IterateOnAStartAlgorithm(
                    registeredTiles,
                    tilesToTry,
                    finalPath,
                    nextTile,
                    tile,
                    possibleEndTiles,
                    out pathScore);

                if (endTileFound)
                {
                    return true;
                }
            }

            pathScore = 0;
            return false;
        }

        private static Vector3 GetAveragePosition(List<Tile> endTiles)
        {
            Vector3 ave = new Vector3();
            foreach (var tile in endTiles)
            {
                ave += tile.transform.position;
            }
            ave = ave / endTiles.Count;
            return ave;
        }

        private static TileScore GetNextTileToTry(List<TileScore> tilesToTry)
        {
            float minScore = 0;
            TileScore best = null;
            foreach (var score in tilesToTry)
            {
                if (best == null || score.Score < minScore)
                {
                    best = score;
                    minScore = score.Score;
                }
            }

            tilesToTry.Remove(best);
            return best;
        }

        private class TileScore
        {
            public int Index;
            public float XScore;
            public float YScore;
            public float Score { get { return XScore + YScore; } }
            public List<int> PathByIndex = new List<int>();
        }
    }
}
