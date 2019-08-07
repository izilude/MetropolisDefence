using System;
using System.Collections.Generic;
using Assets.RTSCore.Inventory;
using System.Xml.Serialization;

namespace Assets.RTSCore.Level
{
	[Serializable]
	public class ItemConversion
	{
		public string ItemProduced { get; set; }

        public int AmtProd { get; set; }

        public int FuelValue { get; set; }

        public int BuildTime { get; set; }

	    public List<Item> ItemsNeedForConversion()
	    {
	        var list = new List<Item>();
            if (!String.IsNullOrEmpty(Req1)) list.Add(new Item(Req1, Amt1));
	        if (!String.IsNullOrEmpty(Req2)) list.Add(new Item(Req2, Amt2));
	        if (!String.IsNullOrEmpty(Req3)) list.Add(new Item(Req3, Amt3));

	        return list;
	    }

        [XmlElement("Requirement1")]
	    public string Req1 { get; set; }

        public int Amt1 { get; set; }

	    [XmlElement("Requirement2")]
        public string Req2 { get; set; }

        public int Amt2 { get; set; }

	    [XmlElement("Requirement3")]
        public string Req3 { get; set; }

        public int Amt3 { get; set; }

        public string Building { get; set; }
    }
}

