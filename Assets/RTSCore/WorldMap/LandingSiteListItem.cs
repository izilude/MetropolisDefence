using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.RTSCore.WorldMap
{
    public static class LandingSiteListItem
    {
        public static void Draw(Rect area, LandingSite site)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = Color.white;

            if (GUI.Button(area, site.Background))
            {
                Game.Game.Instance.WorldMap.LandingSiteSelected(site);
            }

            Rect nameArea = new Rect(area.x, area.y, area.width * 1 / 2, area.height);
            Rect regionArea = new Rect(area.x + area.width * 1 / 2, area.y, area.width * 1 / 4, area.height);
            Rect costArea = new Rect(area.x + area.width * 3 / 4, area.y, area.width * 1 / 4, area.height);
            GUI.Label(nameArea, site.Name, style);
            GUI.Label(regionArea, site.Region, style);
            GUI.Label(costArea, site.CostToLand.ToString(), style);
        }

        public static void DrawHeader(Rect area)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = Color.white;

            Rect nameArea = new Rect(area.x, area.y, area.width * 1 / 2, area.height);
            Rect regionArea = new Rect(area.x + area.width * 1 / 2, area.y, area.width * 1 / 4, area.height);
            Rect costArea = new Rect(area.x + area.width * 3 / 4, area.y, area.width * 1 / 4, area.height);
            GUI.Label(nameArea, "Name", style);
            GUI.Label(regionArea, "Region", style);
            GUI.Label(costArea, "Cost", style);
        }
    }
}
