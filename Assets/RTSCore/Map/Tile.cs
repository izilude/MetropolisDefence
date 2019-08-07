using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.Map
{
    public enum TerrainType { None, Plain, Forest, Mountain, Water, Desert, Jungle, Road }

    public class Tile : MonoBehaviour
    {
        public void SetSprite(Texture2D texture)
        {
            var mat = this.GetComponent<Renderer>().material;
            mat.mainTexture = texture;
        }

        public int Index
        {
            get { return (int)transform.localPosition.x*1000000 + (int)transform.localPosition.z; }
        }

        public float SpeedFactor
        {
            get
            {
                switch (Type)
                {
                    case TerrainType.Road:
                        return 2;
                    default:
                        return 1;
                }
            }    
        }

        public bool Accessible;
        public bool Buildable;

        public TerrainType Type;

        public void Start()
        {
        }

        protected void Update()
        {

            //if (!Buildable)
            //{
            //    this.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            //}
            //if (!Accessible)
            //{
            //    this.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
            //}
        }

        public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			if (o is Tile) {
				Tile tile = o as Tile;

				if (tile.transform.position.x == this.transform.position.x &&
				    tile.transform.position.z == this.transform.position.z) {
					return true;
				}
			}

			return false;
		}

		public override string ToString ()
		{
			return string.Format ("[Tile: type={0}, YPosition={1}, XPosition={2}]", Type, transform.position.x, transform.position.z);
		}
    }
}
