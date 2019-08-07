using System;
using System.Collections.Generic;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Level;
using Assets.RTSCore.Styles;
using Assets.RTSCore.WorldObject.Buildings;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public class HUDProductionManager : HUDWindow
    {
        public ProductionBuilding SelectedBuilding;

        public HUDProductionManager(GUISkin background) : base(background) { }

        public override bool Show
        {
            get
            {
                if (SelectedBuilding == null)
                {
                    _show = false;
                }

                return base.Show;
            }

            set
            {
                base.Show = value;
            }
        }

        protected override void DrawInternal(Rect windowArea)
        {
            DrawItemList(windowArea, SelectedBuilding);
        }

        private void DrawItemList(Rect drawingArea, ProductionBuilding SelectedBuilding)
        {
            List<int> columns = GetColumnsWidths(drawingArea);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;
            style.alignment = TextAnchor.MiddleLeft;
            int y = (int)drawingArea.y + HUDStyles.INFO_TEXT_SPACING;

            DrawHeader(drawingArea, y, style, columns);
            y += (int)(1.25 * HUDStyles.INFO_TEXT_SPACING);

            List<string> items = SelectedBuilding.ProducibleItems;
            foreach (string itemName in items)
            {
                List<ItemConversion> conversions = Game.Game.Instance.Configuration.RequestConversions(itemName);
                foreach (ItemConversion conversion in conversions)
                {
                    Item item = SelectedBuilding.MyInventory.GetItem(itemName);

                    bool newFlag = true;
                    ItemBuildingFlags flags = null;
                    foreach (ItemBuildingFlags flag in SelectedBuilding.ItemFlags)
                    {
                        if (flag.Name == itemName)
                        {
                            flags = flag;
                            newFlag = false;
                            break;
                        }
                    }
                    if (newFlag)
                    {
                        flags = new ItemBuildingFlags();
                        flags.Name = itemName;
                        SelectedBuilding.ItemFlags.Add(flags);
                    }

                    int max = (int)(SelectedBuilding.MyInventory.Capacity * item.PercentOfCapacity);
                    DrawItemLine(drawingArea, y, style, columns, SelectedBuilding, conversion);
                    y += (int)(1.25f * HUDStyles.INFO_TEXT_SPACING);
                }
            }
        }

        private List<int> GetColumnsWidths(Rect drawingArea)
        {
            List<int> columns = new List<int>();

            int delta = 10;

            int remainingWidth = (int)drawingArea.width - 2 * delta;

            int nameColumnWidth = remainingWidth / 3;
            remainingWidth -= nameColumnWidth;

            int buttonColumnWidth = remainingWidth / 6;
            int amountProducedColumnWidth = remainingWidth / 10;
            int timeToProduce = remainingWidth / 10;

            remainingWidth = remainingWidth - buttonColumnWidth - amountProducedColumnWidth - timeToProduce;
            int neededItemsColumnsWidth = remainingWidth;

            columns.Add(delta);
            columns.Add(buttonColumnWidth);
            columns.Add(amountProducedColumnWidth);
            columns.Add(nameColumnWidth);
            columns.Add(timeToProduce);
            columns.Add(neededItemsColumnsWidth);

            return columns;
        }

        private void DrawHeader(Rect drawingArea, int y, GUIStyle style, List<int> columns)
        {
            int x = (int)drawingArea.x + columns[0];
            x += columns[1];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), "Amount" , style);
            x += columns[2];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), "Item", style);
            x += columns[3];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), "Time", style);
            x += columns[4];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), "Items Needed", style);
        }

        private void DrawItemLine(
            Rect drawingArea,
            int y,
            GUIStyle style,
            List<int> columns,
            ProductionBuilding selectedBuilding,
            ItemConversion conversion)
        {
            int buttonTargetWidth = 60;

            int x = (int)drawingArea.x + columns[0];

            string buttonText;
            if (conversion.Equals(selectedBuilding.CurrentConversion))
            {
                buttonText = "Active";
            }
            else
            {
                buttonText = "Select";
            }

            int buttonWidth = columns[1] < buttonTargetWidth ? columns[1] : buttonTargetWidth;
            if (GUI.Button(new Rect(x, y, buttonWidth, HUDStyles.INFO_TEXT_SPACING), buttonText))
            {
                selectedBuilding.CurrentConversion = conversion;
            }

            x += columns[1];

            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), conversion.AmtProd.ToString(), style);
            x += columns[2];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), conversion.ItemProduced, style);
            x += columns[3];
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), String.Format("{0}s", conversion.BuildTime), style);
            x += columns[4];
            string line = string.Empty;
            foreach(Item itemName in conversion.ItemsNeedForConversion())
            {
                if (line != string.Empty) { line += ", "; }
                line += string.Format("{0} {1}", itemName.Amount, itemName.Name);
            }

            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), line, style);
        }
    }
}
