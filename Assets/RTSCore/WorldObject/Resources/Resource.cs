using System;

namespace Assets.RTSCore.WorldObject.Resources 
{
	public class Resource : WorldObject
	{
		// Use this for initialization
		protected override void Start () 
		{
			base.Start();	
		}
		
		// Update is called once per frame
		protected override void Update ()
        {
			base.Update();
		}

        public override Information.Information GetInformation()
        {
            throw new NotImplementedException();
        }
    }
}
