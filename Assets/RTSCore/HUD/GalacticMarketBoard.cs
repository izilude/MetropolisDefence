using System;
using Assets.RTSCore.Economy;
using Assets.RTSCore.Styles;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    [Serializable]
    public class GalacticMarketBoard : MonoBehaviour
    {
        public GUISkin BackgroundSkin;
        private Rect _windowArea;

        public bool Show { get; set; }

        // Use this for initialization
        protected virtual void Start()
        {
        }

        // Update is called once per frame
        protected virtual void OnGUI()
        {
            if (!Show) { return; }

            _windowArea = new Rect(Screen.width / 4, Screen.height / 8, Screen.width / 2, Screen.height * 6/8);

            GUI.skin = BackgroundSkin;
            GUI.BeginGroup(_windowArea);

            Rect internalArea = new Rect(_windowArea);
            internalArea.x = 0;
            internalArea.y = 0;

            GUI.Box(internalArea, "");
            DrawItemList(internalArea);

            GUI.EndGroup();
        }

        private void DrawItemList(Rect drawingArea)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;
            style.alignment = TextAnchor.MiddleLeft;
            int y = (int)drawingArea.y + HUDStyles.INFO_TEXT_SPACING;

            DrawItemLine(drawingArea, y, style, "Name", "Exp", "Price", "Demand", false, null);
            y += (int)(1.25*HUDStyles.INFO_TEXT_SPACING);

            foreach (MarketItem item in Game.Game.Instance.Economy.GallacticMarketItems)
            {
                DrawItemLine(drawingArea, y, style, item);
                y += (int)(1.25f*HUDStyles.INFO_TEXT_SPACING);
            }
        }

        public bool MouseInside(Vector3 mousePos)
        {
            return _windowArea.Contains(mousePos) && Show;
        }

        private void DrawItemLine(Rect drawingArea, int y, GUIStyle style, MarketItem item)
        {
            DrawItemLine(drawingArea, y, style, item.Name, item.Amount.ToString(), item.Price.ToString(), item.DemandText, true, item);
        }

        private void DrawItemLine(
            Rect drawingArea, 
            int y, 
            GUIStyle style, 
            string itemName, 
            string amount, 
            string price,
            string demand,
            bool drawExportButton, 
            MarketItem item)
        {
            int delta = 10;
            int buttonTargetWidth = 50;

            int remainingWidth = (int)drawingArea.width - 2*delta;
            int nameColumnWidth = remainingWidth / 3;
            remainingWidth -= nameColumnWidth;
            int buttonColumnWidth = remainingWidth / 4 > buttonTargetWidth + delta ? buttonTargetWidth + delta : remainingWidth / 4;
            int priceColumnWidth = remainingWidth / 4;
            int demandColumnWidth = remainingWidth / 4;
            remainingWidth = remainingWidth - priceColumnWidth - buttonColumnWidth - demandColumnWidth;
            int amountColumnWidth = remainingWidth / 4;
            int dashColumnWidth = remainingWidth / 4;
            int maxColumnWidth = remainingWidth / 2;

            int buttonWidth = buttonColumnWidth < buttonTargetWidth ? buttonColumnWidth : buttonTargetWidth;

            int x = (int)drawingArea.x + delta;

            if (drawExportButton)
            {
                string buttonText;
                if (!item.Exporting)
                {
                    buttonText = "Keep";
                }
                else
                {
                    buttonText = "Export";
                }

                if (GUI.Button(new Rect(x, y, buttonWidth, HUDStyles.INFO_TEXT_SPACING), buttonText))
                {
                    item.Exporting = !item.Exporting;    
                }
            }

            x += buttonColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), itemName, style);
            x += nameColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), amount, style);
            x += amountColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), @"/", style);
            x += dashColumnWidth;
            //GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), annualMax, style);
            x += maxColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), price, style);
            x += priceColumnWidth;
            GUI.Label(new Rect(x, y, drawingArea.width, HUDStyles.INFO_TEXT_SPACING), demand, style);
        }
    }
}
