using UnityEngine;

namespace Assets.RTSCore.WorldObject
{
    public abstract class BuildableWorldObject : WorldObject
    {
        public int Cost;
        public Texture2D BuildImage;
        public Texture2D BuildHoverImage;
    }
}
