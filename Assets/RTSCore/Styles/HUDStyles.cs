using UnityEngine;

namespace Assets.RTSCore.Styles
{
	public static class HUDStyles
	{
		public static int LEFTRIGHT_BAR_HEIGHT = 243;
		public static int LEFTRIGHT_BAR_WIDTH = (int)(220*1.32);
		public static int BOTTOM_BAR_HEIGHT = 112;
		public static int DECO_CORNER_WIDTH = 50;

		public static int TOP_BAR_HEIGHT = 40;
        public static int TOP_BAR_GALACTICMARKET_WIDTH = 150;
        public static int TOP_BAR_GALACTICMARKET_HEIGHT = 30;
        public static int TOP_BAR_XSPACING = 20;
        public static int TOP_BAR_MONEY_WIDTH = 200;
		public static int TOP_BAR_MONEY_FONTSIZE = 20;
		public static int RESOURCE_BAR_HEIGHT = 40;

		public static Color TEXT_COLOR = new Color(0,240,255);
		
		public static int MINIMAP_BAR_WIDTH = 256, MINIMAP_BAR_HEIGHT = 256, MINIMAP_BAR_VSHIFT = -90, MINIMAP_BAR_HSHIFT = -5;
		public static int SELECTION_NAME_HEIGHT = 12;
		
		public static int BUILD_IMAGE_WIDTH = 48; 
		public static int BUILD_IMAGE_HEIGHT = 48;
		public static int BUILD_IMAGE_X_DELTA = 18;
		public static int BUILD_IMAGE_Y_DELTA = 18;
		public static int BUILD_IMAGE_X_SHIFT = 10;
		public static int BUILD_IMAGE_Y_SHIFT = 28 + 23;
		public static int BUILD_IMAGE_PER_ROW = 3;

        public static int BUILD_MENU_Y_OFFSET = 40;


        public static int ACTION_AREA_WIDTH = 200;
		public static int ACTION_AREA_STARTX = 20;
		
		public static int INFO_AREA_WIDTH = 200;
		public static int INFO_AREA_STARTX = (Screen.width - INFO_AREA_WIDTH) / 2;
		public static int INFO_TEXT_START = 20;
		public static int INFO_TEXT_SPACING = 22;
		public static int INFO_TEXT_WIDTH = 200;
        public static int PROGRESS_BAR_WIDTH = 200;

        public static int ResourceIconWidth = 32, ResourceIconHeight = 32;
		public static int ResourceTextWidth = 105, ResourceTextHeight = 32;
	}
}

