using System.Collections.Generic;
using Assets.RTSCore.Level;
using Assets.RTSCore.Map;
using Assets.RTSCore.Misc;
using Assets.RTSCore.WorldObject;
using UnityEngine;

namespace Assets.RTSCore.AI 
{
	public static class PathFinding 
	{
		public static bool TryGetUnitPath(MovingWorldObject movingWorldObject, List<MonoBehaviour> gameObjects, out List<Tile> wayPoints)
		{
			wayPoints = new List<Tile>();

			movingWorldObject.State = UnitState.Stopped;
			
			List<Tile> tiles = Game.Game.Instance.ActiveLevel.Map.FindOccupyingTiles(movingWorldObject.transform);
		    Tile startTile = null;
		    float bestScore = 0;

		    List<Tile> bestPath = null;
            List<Tile> fullPath = new List<Tile>();
		    List<Tile> bestWayPoints = null;
 		    foreach (var tile in tiles)
		    {
		        bool success = false;
		        float score = 0;

                wayPoints.Clear();
                fullPath.Clear();

                //wayPoints.Add(tile);
                //fullPath.Add(tile);
		        foreach (MonoBehaviour gameObject in gameObjects)
		        {
		            List<Tile> path;
		            var legStartTile = wayPoints.Count == 0 ? tile : wayPoints[wayPoints.Count - 1];
                    success = TryGetUnitPathToTile(gameObject, legStartTile, out score, out path);
		            if (!success) break;

		            if (path.Count != 0)
		            {
		                fullPath.AddRange(path);
		                wayPoints.Add(path[path.Count - 1]);
		            }
		        }

		        if (success && (bestPath == null || score < bestScore))
		        {
		            bestScore = score;
		            bestPath = fullPath;
		            bestWayPoints = wayPoints;
		        }
            }

		    if (bestPath == null || bestWayPoints == null) return false;

            movingWorldObject.MovementQueue.AddRange(bestPath);
            movingWorldObject.WayPoints.Clear();
		    movingWorldObject.WayPoints.AddRange(bestWayPoints);

            return true;
		}

        public static bool TryGetPathToWorldObject(Vector3 SpawnPoint, WorldObject.WorldObject Headquarters, out List<Tile> finalPath)
        {
            Tile startTile = Game.Game.Instance.ActiveLevel.Map.FindClosestTile(SpawnPoint);
            float bestPathScore;
            return TryGetPathToGameObject(Headquarters, startTile, out finalPath, out bestPathScore);
        }


        public static bool DoesPathExistBetweenWorldObjects(
            WorldObject.WorldObject wob1, 
            WorldObject.WorldObject wob2, 
            out int pathlength,
            out float pathScore) 
		{
			pathlength = 0;

		    List<Tile> possibleStartTiles = Game.Game.Instance.ActiveLevel.Map.FindSurroundingTiles(wob1, true);
			foreach (Tile startTile in possibleStartTiles) 
			{
				List<Tile> bestPath;
				if (TryGetPathToGameObject(wob2, startTile, out bestPath, out pathScore)) 
				{
					pathlength = bestPath.Count;
					return true;
				}
			}

		    pathScore = 0;
			return false;
		}

		private static bool TryGetPathToGameObject(
            MonoBehaviour gameObject, 
            Tile startTile, 
            out List<Tile> bestPath,
            out float pathScore) 
		{
		    var possibleEndTiles = Game.Game.Instance.ActiveLevel.Map.FindSurroundingTiles(gameObject, false);

		    for (int i = possibleEndTiles.Count - 1; i >= 0; i--)
		    {
		        if (!possibleEndTiles[i].Accessible) possibleEndTiles.RemoveAt(i);
		    }

		    return AStarPathFinding.FindPathBetweenTwoTiles(startTile, possibleEndTiles, out bestPath, out pathScore);
		}

		private static bool TryGetUnitPathToTile (
            MonoBehaviour gameObject, 
            Tile startTile, 
            out float bestScore,
            out List<Tile> bestPath)
		{
			if (TryGetPathToGameObject(gameObject, startTile, out bestPath, out bestScore)) 
			{
                return true;
			}

		    bestScore = 0;
            return false;
		}

		private static List<Tile> OrderTilesByProximityToDestination(List<Tile> tiles, Tile endTile) 
		{
			List<Tile> orderedTiles = new List<Tile>();
			List<double> mags = new List<double>();
			foreach (Tile tile in tiles) 
			{
				Vector3 diff = tile.transform.position - endTile.transform.position;
				double mag = diff.magnitude;

				bool inserted = false;
				for(int i=0;i<orderedTiles.Count;i++) 
				{
					if (mag < mags[i]) 
					{
						orderedTiles.Insert(i, tile);
						mags.Insert(i, mag);
						inserted = true;
						break;
					}
				}

				if (!inserted) 
				{
					mags.Add(mag);
					orderedTiles.Add(tile);
				}
			}

			return orderedTiles;
		}

		private static bool FindPathBetweenTiles(Map.Map map, Tile startTile, Tile currentTile, Tile endTile, List<Tile> path, List<Tile> bestPath, bool requireRoads) 
		{
			if (currentTile.Equals(endTile) || bestPath.Count > 0) 
			{
				return true;
			}

			if (bestPath.Count != 0 && path.Count >= bestPath.Count) {return false;}

			List<Tile> tiles = map.FindSurroundingTiles(currentTile, false);
			tiles = OrderTilesByProximityToDestination(tiles, endTile);
			foreach (Tile tile in tiles)
			{
				if (!path.Contains(tile) && (tile.Type == TerrainType.Road || !requireRoads) && !tile.Equals(startTile)) 
				{
					path.Add (tile);
					bool pathFound = FindPathBetweenTiles(map, startTile, tile, endTile, path, bestPath, requireRoads);

					if (pathFound) 
					{
						if (bestPath.Count == 0) 
						{
							bestPath.Clear();
							foreach (Tile t in path) 
							{
								bestPath.Add(t);
							}
						} 
						else 
						{
							return true;
						}
					} 

					path.Remove(tile);
				}
			}

			return false;
		}
	}
}
