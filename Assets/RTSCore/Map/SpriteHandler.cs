using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.RTSCore.Map
{
    public class SpriteHandler
    {
        private List<Texture2D> _imageTiles;

        public Texture2D Sprite
        {
            set
            {
                Split(value, 128, 128);
            }
        }

        public Texture2D GetTile(bool left, bool top, bool right, bool bottom, bool center)
        {
            int cnt = 0;
            cnt += left ? 1 : 0;
            cnt += top ? 1 : 0;
            cnt += right ? 1 : 0;
            cnt += bottom ? 1 : 0;

            int index = 0;
            switch (cnt)
            {
                case 0:
                    if (center) index = GetCenterSquare();
                    else index = GetPureSquare();
                    break;
                case 1:
                    index = GetSingleEdge();
                    break;
                case 2:
                    if (left && right) index = GetAcross();
                    else if (top && bottom) index = GetAcross();
                    else index = GetElbow();
                    break;
                case 3:
                    index = GetThreeEdge();
                    break;
                case 4:
                    index = GetFourEdge();
                    break;
            }

            return _imageTiles[index];
        }

        private int GetCenterSquare()
        {
            return 8;
        }

        private int GetPureSquare()
        {
            return UnityEngine.Random.Range(0,3);
        }

        private int GetSingleEdge()
        {
            return 8;
        }

        private int GetAcross()
        {
            return 9;
        }

        private int GetElbow()
        {
            return 16;
        }

        private int GetThreeEdge()
        {
            return 17;
        }


        private int GetFourEdge()
        {
            return 25;
        }

        public void Split(Texture2D image, int width, int height)
        {
            _imageTiles = new List<Texture2D>();

            bool perfectWidth = image.width % width == 0;
            bool perfectHeight = image.height % height == 0;

            int lastWidth = width;
            if (!perfectWidth)
            {
                lastWidth = image.width - ((image.width / width) * width);
            }

            int lastHeight = height;
            if (!perfectHeight)
            {
                lastHeight = image.height - ((image.height / height) * height);
            }

            int widthPartsCount = image.width / width + (perfectWidth ? 0 : 1);
            int heightPartsCount = image.height / height + (perfectHeight ? 0 : 1);

            for (int i = 0; i < heightPartsCount; i++)
            {
                for (int j = 0; j < widthPartsCount; j++)
                {
                    int tileWidth = i == widthPartsCount - 1 ? lastWidth : width;
                    int tileHeight = j == heightPartsCount - 1 ? lastHeight : height;

                    Texture2D g = new Texture2D(tileWidth, tileHeight);
                    g.SetPixels(image.GetPixels(j * width, i * height, tileWidth, tileHeight));
                    g.Apply();
                    _imageTiles.Add(g);
                }
            }
        }

    }
}
