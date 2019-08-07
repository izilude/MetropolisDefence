using System.Collections.Generic;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Styles;
using Assets.RTSCore.WorldObject.Buildings;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public class HUDInventoryManager : HUDWindow
    {
        public Building SelectedBuilding;

        public HUDInventoryManager(GUISkin background) : base(background)
        {

        }

        public override bool Show
        {
            get
            {
                if (SelectedBuilding == null) { _show = false; }
                return _show;
            }
            set
            {
                base.Show = value;
            }
        }

        protected override void DrawInternal(Rect area)
        { 
            DrawItemList(area, SelectedBuilding);
        }

        private void DrawItemList(Rect drawingArea, Building SelectedBuilding)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;
            style.alignment = TextAnchor.MiddleLeft;
            int y = (int)drawingArea.y + HUDStyles.INFO_TEXT_SPACING;

            DrawItemLine(drawingArea, y, style, "Name", "Amt", "Max", null, null, false);
            y += (int)(1.25 * HUDStyles.INFO_TEXT_SPACING);

            List<string> items = Game.Game.Instance.ActiveLevel.AllItems();
            foreach (string itemName in items)
            {
                Item item = SelectedBuilding.MyInventory.GetItem(itemName);
                if (item == null) continue;

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
                
                int max =(int)(SelectedBuilding.MyInventory.Capacity * item.PercentOfCapacity);
                DrawItemLine(drawingArea, y, style, itemName, item.Amount.ToString(), max.ToString(), item, flags, true);
                y += (int)(1.25f * HUDStyles.INFO_TEXT_SPACING);
            }
        }

        private void DrawItemLine(
            Rect drawingArea,
            int y,
            GUIStyle style, 
            string name,
            string currentAmount,
            string maxAmount,
            Item item,
            ItemBuildingFlags flags,
            bool drawButtons)
        {
            int delta = 10;
            int buttonTargetWidth = 60;

            int remainingWidth = (int)drawingArea.width - 2 * delta;

            int nameColumnWidth = remainingWidth / 3;
            remainingWidth -= nameColumnWidth;

            int buttonColumnWidth = remainingWidth / 3;
            int amountColumnWidth = remainingWidth / 10;
            int dashColumnWidth = remainingWidth / 10;
            int maxAmountColumnWidth = remainingWidth / 10;

            remainingWidth -= buttonColumnWidth;
            remainingWidth -= amountColumnWidth;
            remainingWidth -= dashColumnWidth;
            remainingWidth -= maxAmountColumnWidth;

            int buttonWidth = buttonColumnWidth < buttonTargetWidth ? buttonColumnWidth : buttonTargetWidth;

            int x = (int)drawingArea.x + delta;

            if (drawButtons)
            {
                DrawOnOffButton(y, ref flags.Keep, buttonWidth, x, "Keep", "Empty");
                x += buttonColumnWidth / 2;
                DrawOnOffButton(y, ref flags.Request, buttonWidth, x, "Request", "Refuse");
                x += buttonColumnWidth / 2;
            }
            else
            {
                x += buttonColumnWidth;
            }

            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), name, style);
            x += nameColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), currentAmount, style);
            x += amountColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), @"/", style);
            x += dashColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), maxAmount, style);
            x += maxAmountColumnWidth;

            if (drawButtons)
            {
                float percentButtonWidth = remainingWidth / 4 - 5;

                if (GUI.Button(new Rect(x, y, percentButtonWidth, HUDStyles.INFO_TEXT_SPACING), "1/4"))
                {
                    item.PercentOfCapacity = 0.25f;
                }
                x += remainingWidth / 4;

                if (GUI.Button(new Rect(x, y, percentButtonWidth, HUDStyles.INFO_TEXT_SPACING), "1/2"))
                {
                    item.PercentOfCapacity = 0.5f;
                }
                x += remainingWidth / 4;

                if (GUI.Button(new Rect(x, y, percentButtonWidth, HUDStyles.INFO_TEXT_SPACING), "3/4"))
                {
                    item.PercentOfCapacity = 0.75f;
                }
                x += remainingWidth / 4;

                if (GUI.Button(new Rect(x, y, percentButtonWidth, HUDStyles.INFO_TEXT_SPACING), "Max"))
                {
                    item.PercentOfCapacity = 1f;
                }
            }
        }

        private static void DrawOnOffButton(int y, ref bool onOff, int buttonWidth, int x, string onText, string offText)
        {
            string buttonText;
            if (onOff)
            {
                buttonText = onText;
            }
            else
            {
                buttonText = offText;
            }

            if (GUI.Button(new Rect(x, y, buttonWidth, HUDStyles.INFO_TEXT_SPACING), buttonText))
            {
                onOff = !onOff;
            }
        }
    }
}
