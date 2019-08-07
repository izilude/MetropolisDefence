using System.Runtime.InteropServices;
using Assets.RTSCore.Misc;

namespace Assets.RTSCore.WorldObject.Creep 
{
	public class Creep : MovingWorldObject 
	{
        public int ChallengeRating;
	    public int Penalty;
		
		// Update is called once per frame
		protected override void Update () 
		{
			base.Update();

		    if (State == UnitState.Finished)
		    {
		        Game.Game.Instance.ActivePlayer.ReducePlayerHealth(Penalty);
                Destroy(gameObject);
		    }
		}
	}
}
