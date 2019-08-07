using UnityEngine;

namespace Assets.RTSCore.Map
{
	public static class MapGenerator
	{	
		private static int MAXFAILS = 100000;

		public static void GenerateNaturalResources(MapFeatureOptions options, Level.Level level) 
		{
			WorldObject.WorldObject wob = options.FeaturePrefab.GetComponent<WorldObject.WorldObject>();
			if (wob == null) 
			{
				Debug.Log("MapGenerator->GenerateNaturalResources: World Object component null!");
				return;
			}

			int fails = 0;
			for(int i=0;i<options.NumberToGenerate;i++) 
			{
				float x = options.CenterOfCluster.x;
				float z = options.CenterOfCluster.y;

				if (i != 0 || fails > 0) 
				{
					float randNum = UnityEngine.Random.Range(0.0f, 1.0f);
					x += options.ClusterSizeInTiles.x*(randNum - 0.5f);
					randNum = UnityEngine.Random.Range(0.0f, 1.0f);
					z += options.ClusterSizeInTiles.y*(randNum - 0.5f);
				}

				wob.transform.position = new Vector3(x, 0, z);

				if (!level.Map.SnapToGrid(wob) || !level.Map.IsBuildable(wob)) 
				{
					fails++;
					if (fails > MAXFAILS) 
					{
						return;
					}

					i--;
					continue;
				}

				fails = 0;
			    Game.Game.Instance.ActiveLevel.AddWorldObject(wob, true);
				level.Map.ChangeTileType(wob);
				level.Map.MakeOccupyingTilesUnavailable(wob);
            }
        }
	}
}

