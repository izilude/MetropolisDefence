using System;
using Assets.RTSCore.Game;
using Assets.RTSCore.Misc;
using UnityEngine;

namespace Assets.RTSCore.GameEvents
{
    [Serializable]
    public class GameEvent : MonoBehaviour
    {
        public string Name;
        public string Message;
        public Texture2D InternalPicture;
        public float Duration;
        public Level.Level CurrentLevel;

        CountdownEvent timer;

        public virtual void Start()
        {
            timer = new CountdownEvent(Duration);
            ApplyEventEffects();
        }

        public virtual void Update()
        {
            timer.Update(GameTimeManager.DeltaTime);
        }

        public bool IsExpired
        {
            get
            {
                return timer.IsExpired;
            }
        }

        public virtual string Outcome
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual void ApplyEventEffects() { }

        public virtual void RemoveEventEffects() { }
    }
}
