using System;
using System.Collections.Generic;
using Assets.RTSCore.Level;
using UnityEngine;

namespace Assets.RTSCore.Map
{
    public class IslandMap : Map
    {
        public int NumberOfIslands;
        public int NumberOfSmallIsland;
        
        protected override void Start()
        {
        }

        public override void Initialize()
        {
            List<float> xCenters = new List<float>();
            List<float> zCenters = new List<float>();

            CreateIslands(xCenters, zCenters, NumberOfIslands, 0.85f);
            CreateIslands(xCenters, zCenters, NumberOfSmallIsland, 0.5f);

            base.Initialize(); 
        }

        private void CreateIslands(List<float> xCenters, List<float> zCenters, int n, float percent)
        {
            for (int i = 0; i < NumberOfIslands; i++)
            {
                float size = ((XTiles + YTiles) / 4) * percent;
                size = size / n;

                float xc = (XTiles - 2 * size) * UnityEngine.Random.value + size;
                float zc = (YTiles - 2 * size) * UnityEngine.Random.value + size;

                int cnt = 0;
                while (cnt < 1000)
                {
                    bool goodCenter = true;

                    for (int j = 0; j < xCenters.Count; j++)
                    {
                        float dis = (xc - xCenters[j]) * (xc - xCenters[j]) + (zc - zCenters[j]) * (zc - zCenters[j]);
                        dis = (float)Math.Sqrt(dis);
                        if (dis < size)
                        {
                            goodCenter = false;
                        }
                    }

                    cnt++;
                    if (goodCenter)
                    {
                        xCenters.Add(xc);
                        zCenters.Add(zc);
                        break;
                    }
                }

                CreateIsland(xc, zc, size);
            }
        }

        protected bool _mapGenerated;
        private void CreateIsland(float xCenter, float zCenter, float size)
        {
            float y = 0.0f;

            int tileCount = 0;
            for (int i = 0; i < XTiles; i++)
            {
                for (int j = 0; j < YTiles; j++)
                {
                    float randNum = UnityEngine.Random.Range(0.0f, 1.0f);

                    float xTile = (float)i * TileSize;
                    float zTile = (float)j * TileSize;
                    float deltaR = (xTile - xCenter) * (xTile - xCenter) + (zTile - zCenter) * (zTile - zCenter);
                    deltaR = (float)Math.Sqrt(deltaR);

                    float XUnc = 10;
                    float YUnc = 10;
                    float grayArea = (float)Math.Sqrt(XUnc * XUnc + YUnc * YUnc)/2; 

                    GameObject tilePrefab = null;
                    if (deltaR < size - grayArea)
                    {
                        tilePrefab = TilePrefab;
                    }
                    else if (deltaR > size + grayArea)
                    {
                        tilePrefab = TilePrefab;
                    }
                    else
                    {
                        float prob = ((size + grayArea) - deltaR) / (2 * grayArea);
                        TerrainType type = UnityEngine.Random.value < prob ? TerrainType.Plain : TerrainType.Water;
                        tilePrefab = TilePrefab;
                    }

                    if (tilePrefab == null)
                    {
                        return;
                    }

                    if (_mapGenerated)
                    {
                        int index = GetIndex(xTile, zTile);

                        Tile tempTile = tilePrefab.GetComponent<Tile>();
                        if (Tiles[index].Type == TerrainType.Water && tempTile.Type != TerrainType.Water)
                        {
                            Tiles[index] = SwapTile(Tiles[index], tempTile.Type);
                        }
                    }
                    else
                    {
                        Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                        Vector3 position = new Vector3(xTile, y, zTile);
                        GameObject newGameObject = (GameObject)Instantiate(tilePrefab, position, rotation);
                        newGameObject.transform.parent = this.transform;

                        Tile newTile = newGameObject.GetComponent<Tile>();

                        AddTile(tileCount, newTile);
                        tileCount++;
                    }
                }
            }

            ReplaceTileGroups(TerrainType.Water, TerrainType.Plain, 2);
            ReplaceTileGroups(TerrainType.Plain, TerrainType.Water, 2);

            _mapGenerated = true;
        }

    }
}
