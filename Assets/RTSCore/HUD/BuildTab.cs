using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.HUD 
{
	[Serializable]
	public class BuildTab
	{
		public Texture2D Image;

		public Texture2D HoverImage;

		public List<GameObject> InfrastructurePrefabList;
	}
}

