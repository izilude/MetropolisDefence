using System;
using UnityEngine;

namespace Assets.RTSCore.Map
{
	[Serializable]
	public class MapFeatureOptions
	{
		public GameObject FeaturePrefab;

		public int NumberToGenerate;

		public Vector2 ClusterSizeInTiles;

		public Vector2 CenterOfCluster;
	}
}

