using System;
using System.Collections.Generic;
using Assets.RTSCore.Level;
using UnityEngine;

namespace Assets.RTSCore.Map
{
    public class Map : MonoBehaviour
    {
        public int XTiles;
        public int YTiles;
        public float TileSize;

        private SpriteHandler _grassRoad;
        public Texture2D GrassRoadSprite;

        public GameObject TilePrefab;
        protected Dictionary<int, Tile> Tiles = new Dictionary<int, Tile>();

        public void AddTile(int index, Tile newTile)
        {
            if (_grassRoad == null)
            {
                _grassRoad = new SpriteHandler();
                _grassRoad.Sprite = GrassRoadSprite;
            }

            newTile.SetSprite(_grassRoad.GetTile(false,false,false,false,false));
            Tiles.Add(index, newTile);
        }

        public bool MapReady;

        // Use this for initialization
        protected virtual void Start()
        {
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public virtual void Initialize()
        {
            foreach (MapFeatureOptions option in Game.Game.Instance.ActiveLevel.NaturalResources)
            {
                MapGenerator.GenerateNaturalResources(option, Game.Game.Instance.ActiveLevel);
            }

            MapReady = true;
        }

        public Vector3 GetRandomVector3()
        {
            int maxLoopCount = 10000;
            int count = 0;
            while (count < maxLoopCount)
            {
                int n = UnityEngine.Random.Range(0, XTiles);
                int m = UnityEngine.Random.Range(0, YTiles);

                int index = GetIndex(n, m);
                Tile tile;
                Tiles.TryGetValue(index, out tile);

                if (tile != null && tile.Buildable)
                {
                    return tile.transform.position;
                }

                count++;
            }

            return new Vector3();
        }

        public bool SnapToGrid(WorldObject.WorldObject worldObject) 
		{
			if (!worldObject) { return false; }

			//worldObject.transform.parent = this.transform;
			Tile tile = FindClosestTile(worldObject.transform.position);

			if (!tile) { return false; }

			float xoffset = 0;
			if ((int)worldObject.transform.localScale.x % 2 == 0) {
				xoffset += TileSize/2;
			}

			float zoffset = 0;
			if ((int)worldObject.transform.localScale.z % 2 == 0) {
				zoffset += TileSize/2;
			}

			worldObject.transform.position = new Vector3(tile.transform.position.x + xoffset,
			                                             tile.transform.position.y + tile.transform.localScale.y/2,
			                                             tile.transform.position.z + zoffset);

			return true;
		}

		public void MakeOccupyingTilesUnavailable(WorldObject.WorldObject worldObject) 
		{
			var tiles = FindOccupyingTiles(worldObject);

			foreach (Tile tile in tiles) 
			{
				tile.Accessible = tile.Type == TerrainType.Road;
				tile.Buildable = false;
			}
		}

		public List<Tile> FindOccupyingTiles(WorldObject.WorldObject worldObject) 
		{
			return FindOccupyingTiles(worldObject.transform);
		}

		public List<Tile> FindOccupyingTiles(Transform someTransform) 
		{
			List<Tile> tiles = new List<Tile>();
			
			float xoffset = 0;
			if ((int)someTransform.localScale.x % 2 == 0) 
			{
				xoffset += TileSize/2;
			}
			
			float zoffset = 0;
			if ((int)someTransform.localScale.z % 2 == 0) 
			{
				zoffset += TileSize/2;
			}
			
			int xwidth = (int)Math.Ceiling(someTransform.localScale.x);
			int zwidth = (int)Math.Ceiling(someTransform.localScale.z);
			
			int n = (int) (xwidth/TileSize);
			int m = (int) (zwidth/TileSize);
			
			Vector3 correctedObjectPosition = new Vector3(someTransform.position.x - xoffset,
			    someTransform.position.y,
			    someTransform.position.z - zoffset);
			
			Tile closestTile = FindClosestTile(correctedObjectPosition);

			if (!closestTile) {return tiles;}

			for(int i=0;i<n;i++) 
			{
				for(int j=0;j<m;j++) 
				{
					int x = (int)(closestTile.transform.position.x) + i;
					int z = (int)(closestTile.transform.position.z) + j;
					int index = GetIndex(x,z);
					
					Tile tile;
					if (Tiles.TryGetValue(index, out tile)) 
					{
						tiles.Add (tile);
					}
				}
			}
			
			return tiles;
		}

		public void ChangeTileType(WorldObject.WorldObject worldObject) 
		{
			if (worldObject.TerrainTypeToChangeTile == TerrainType.None) { return; }

			var tiles = FindOccupyingTiles(worldObject);
			foreach (Tile tile in tiles)
            {
				ChangeTileType(tile, worldObject.TerrainTypeToChangeTile);
			}
		}

        private void ChangeTileType(Tile tile, TerrainType type)
        {
            if (type == TerrainType.Road)
            {
                var surroundingTiles = FindSurroundingTiles(tile, false);
                bool left = false;
                bool right = false;
                bool top = false;
                bool bottom = false;
                bool center = false;
                foreach (var sTile in surroundingTiles)
                {
                    if (sTile.transform.localPosition.x < tile.transform.localPosition.x)
                    {
                        if (sTile.Type == TerrainType.Road) left = true;
                    }
                    else if (sTile.transform.localPosition.x > tile.transform.localPosition.x)
                    {
                        if (sTile.Type == TerrainType.Road) right = true;
                    }
                    else if (sTile.transform.localPosition.y < tile.transform.localPosition.y)
                    {
                        if (sTile.Type == TerrainType.Road) bottom = true;
                    }
                    else if (sTile.transform.localPosition.y > tile.transform.localPosition.y)
                    {
                        if (sTile.Type == TerrainType.Road) top = true;
                    }
                }
                tile.SetSprite(_grassRoad.GetTile(left, top, right, bottom, center));
            }

            tile.Type = type;
        }

		public List<Tile> FindSurroundingTiles(MonoBehaviour someGameObject, bool includeDiagonal ) 
		{
			if (someGameObject == null || someGameObject.transform == null) { return new List<Tile>(); }

			float xoffset = 0;
			if ((int)someGameObject.transform.localScale.x % 2 == 0) {
				xoffset += TileSize/2;
			}
			
			float zoffset = 0;
			if ((int)someGameObject.transform.localScale.z % 2 == 0) {
				zoffset += TileSize/2;
			}

			List<Tile> tiles = new List<Tile>();

			int xwidth = (int)Math.Ceiling(someGameObject.transform.localScale.x);
			int zwidth = (int)Math.Ceiling(someGameObject.transform.localScale.z);

			int n = (int) (xwidth/TileSize);
			int m = (int) (zwidth/TileSize);

			Vector3 correctedObjectPosition = new Vector3(someGameObject.transform.position.x - xoffset,
			    someGameObject.transform.position.y,
			    someGameObject.transform.position.z - zoffset);

			Tile closestTile = FindClosestTile(correctedObjectPosition);

			int istart = (int)Math.Ceiling((double) n/2);
			int jstart = (int)Math.Ceiling((double) m/2);
			int istop = istart + 1 - n % 2;
			int jstop = jstart + 1 - m % 2;

			for(int i=-istart;i<istop+1;i++) 
			{
				for(int j=-jstart;j<jstop+1;j++) 
				{
					if(
						!includeDiagonal &&
					  ( (i==-istart && j==-jstart) ||
					 	(i==-istart && j==jstop) ||
					 	(i==istop && j==-jstart) ||
					 	(i==istop && j==jstop) ) ) {
						continue;
					} else if ( i == -istart || i == istop) {
						;
					} else if (j == -jstart || j == jstop) {
						;
					} else {
						continue;
					}

					int x = (int)(closestTile.transform.position.x) + i;
					int z = (int)(closestTile.transform.position.z) + j;
					int index = GetIndex(x,z);

					Tile tile;
					if (Tiles.TryGetValue(index, out tile)) 
					{
						tiles.Add(tile);
					}
				}
			}

			return tiles;
		}

		public Tile FindClosestTile(Vector3 point) 
		{
			int index = GetIndex(point.x, point.z);
			Tile tile; 
			Tiles.TryGetValue(index, out tile);
			return tile;
		}

		public bool IsBuildable(WorldObject.WorldObject worldObject) 
		{
			var tiles = FindOccupyingTiles(worldObject);

			foreach (Tile tile in tiles) {
				if (!tile.Buildable) {
					return false;
				}
			}

			return true;
		}
        
        protected void ReplaceTileGroups(TerrainType typeToChange, TerrainType newType, int minimumNeighbors)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile tile = Tiles[i];

                if (tile.Type != typeToChange) { continue; }

                List<Tile> neighbors = GetNeighbors(tile);

                bool switchTile = true;
                int cnt = 0;
                foreach (Tile t in neighbors)
                {
                    if (t.Type == typeToChange)
                    {
                        cnt++;
                        if (cnt >= minimumNeighbors)
                        {
                            switchTile = false;
                            break;
                        }
                    }
                }

                if (!switchTile) {continue;}

                Tiles[i] = SwapTile(tile, newType);
            }
        }

        protected Tile SwapTile(Tile tileToReplace, TerrainType newType)
        {
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            Vector3 position = tileToReplace.transform.position;
            GameObject newGameObject = (GameObject)Instantiate(TilePrefab, position, rotation);
            newGameObject.transform.parent = this.transform;

            Tile newTile = newGameObject.GetComponent<Tile>();

            return newTile;
        }

        protected List<Tile> GetNeighbors(Tile tile) 
        {
            List<Tile> neighbors = new List<Tile>();

            float x = tile.transform.position.x;
            float z = tile.transform.position.z;

            int index = GetIndex(x + TileSize, z);
            if (index >= 0 && index < XTiles * YTiles)
            {
                neighbors.Add(Tiles[index]);
            }

            index = GetIndex(x - TileSize, z);
            if (index >= 0 && index < XTiles * YTiles)
            {
                neighbors.Add(Tiles[index]);
            }

            index = GetIndex(x, z + TileSize);
            if (index >= 0 && index < XTiles * YTiles)
            {
                neighbors.Add(Tiles[index]);
            }

            index = GetIndex(x, z - TileSize);
            if (index >= 0 && index < XTiles * YTiles)
            {
                neighbors.Add(Tiles[index]);
            }

            return neighbors;
        }

        protected int GetIndex(float x, float z) 
        {
			int xn = (int)Math.Round(x/TileSize);
			int zn = (int)Math.Round(z/TileSize);

			return GetIndex (xn, zn);
		}
		
		protected int GetIndex(int xn, int zn) 
		{
			return (int)((YTiles*xn + zn) / TileSize);
		}
		
		protected void CreateRandomMap()
        {
            float y = 0.0f;
            for (int i = 0; i < XTiles; i++)
            {
                float x = (float)i;

                for (int j = 0; j < YTiles; j++)
                {
                    float z = (float)j;

                    Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                    Vector3 position = new Vector3(x, y, z);
                    GameObject newGameObject = (GameObject)Instantiate(TilePrefab, position, rotation);

                    newGameObject.transform.parent = this.transform;

                    Tile newTile = newGameObject.GetComponent<Tile>();

                    int index = GetIndex(x,z);
                    AddTile(index, newTile);
                }
            }
        }
    }
}
