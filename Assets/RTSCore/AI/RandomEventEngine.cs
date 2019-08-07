using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.GameEvents;
using UnityEngine;

namespace Assets.RTSCore.AI
{
    public class RandomEventEngine : MonoBehaviour
    {
        public List<GameEvent> ActiveGameEvents
        {
            get
            {
                return _activeGameEvents;
            }
        } private List<GameEvent> _activeGameEvents = new List<GameEvent>();

        public List<GameEvent> GameEventPrefabs = new List<GameEvent>();

        public float CooldownTime;
        public float TickTime;
        public float ProbabilityConstant;

        private float _timeSinceLastEvent;
        private float _timeSinceLastTick;

        // Use this for initialization
        void Start()
        {
            _timeSinceLastEvent = 0;
            _timeSinceLastTick = 0;
        }

        // Update is called once per frame
        void Update()
        {
            for(int i=ActiveGameEvents.Count-1;i>=0; i--)
            {
                if (ActiveGameEvents[i].IsExpired)
                {
                    ActiveGameEvents[i].RemoveEventEffects();
                    ActiveGameEvents.RemoveAt(i);
                }
            }

            _timeSinceLastEvent += GameTimeManager.DeltaTime;
            if (_timeSinceLastEvent < CooldownTime) { return; }

            _timeSinceLastTick += GameTimeManager.DeltaTime;
            if (_timeSinceLastTick > TickTime)
            {
                _timeSinceLastTick = 0;

                if (TryTriggerRandomEvent())
                {
                    _timeSinceLastEvent = 0;
                    _timeSinceLastTick = 0;
                }
            }
        }

        private bool TryTriggerRandomEvent()
        {
            float timeRatio = (_timeSinceLastEvent - CooldownTime)/TickTime;
            timeRatio = timeRatio / ProbabilityConstant;
            float prob = Mathf.Exp(-timeRatio);

            float val = Random.value;
            Debug.Log(string.Format("Rolled: {0} Prob: {1}", val, prob));

            if (val > prob)
            {
                TriggerRandomEvent();
                return true;
            }

            return false;
        }

        private void TriggerRandomEvent()
        {
            Debug.Log(string.Format("Random Event Triggered {0}", _timeSinceLastEvent));

            int cnt = GameEventPrefabs.Count;
            if (cnt == 0) { return; }

            int index = Mathf.FloorToInt(Random.value * cnt);
            if (index >= cnt) { index = cnt - 1; }

            GameEvent gameEvent = (GameEvent)Instantiate(GameEventPrefabs[index]);

            Game.Game.Instance.ActiveLevel.Hud.GameEventMessage.DisplayGameEventMessage(gameEvent);
            ActiveGameEvents.Add(gameEvent);
        }
    }
}