using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.TechTree
{
    public class Technology
    {
        public string Name;
        public string Description;
        public Texture2D Icon;

        public bool Known;

        public List<string> PreRequisites = new List<string>();
    }
}
