using Assets.RTSCore.Game;
using Assets.RTSCore.GameEvents;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public class HUDGameEventMessage : HUDWindow
    {
        private GameEvent _gameEvent;

        public HUDGameEventMessage(GUISkin background) : base(background) { }

        protected override void DrawInternal(Rect area)
        {
            float imageWidthHeight = 256;
            float padding = 20;
            float nameLabelHeight = 50;

            Rect innerImageArea = new Rect(area.x + padding, area.y + padding, imageWidthHeight, imageWidthHeight);
            GUI.DrawTexture(innerImageArea, _gameEvent.InternalPicture);

            float nameLabelWidth = area.width - (area.x + 2 * padding + imageWidthHeight);
            GUIStyle nameLabelStyle = new GUIStyle();
            nameLabelStyle.fontSize = 22;
            Rect namePosition = new Rect(area.x + 2*padding + imageWidthHeight, area.y + padding, nameLabelWidth, nameLabelHeight);
            GUI.Label(namePosition, _gameEvent.Name, nameLabelStyle);

            Rect messagePosition = new Rect(area.x + 2 * padding + imageWidthHeight, 
                area.y + 2*padding + nameLabelHeight, 
                nameLabelWidth, 
                nameLabelHeight);

            nameLabelStyle.wordWrap = true;
            GUI.Label(messagePosition, _gameEvent.Message, nameLabelStyle);

            Rect outcomePosition = new Rect(area.x + padding,
                area.y + 2 * padding + innerImageArea.height,
                area.width - 2*padding,
                nameLabelHeight);

            nameLabelStyle.wordWrap = true;
            nameLabelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(outcomePosition, _gameEvent.Outcome, nameLabelStyle);

            outcomePosition.y = area.height - padding - outcomePosition.height;
            outcomePosition.width = area.width/6;
            outcomePosition.x = (area.width - outcomePosition.width) / 2;
            if (GUI.Button(outcomePosition, "Ok"))
            {
                Show = false;
            }
        }

        public override bool Show
        {
            get
            {
                return base.Show;
            }

            set
            {
                GameTimeManager.Paused = value;
                base.Show = value;
            }
        }

        public void DisplayGameEventMessage(GameEvent gameEvent)
        {
            _gameEvent = gameEvent;
            Show = true;
        } 
    }
}
