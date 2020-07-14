using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class SpaceBuilder 
    {
		private static SpaceBuilder instance;

		/**
		 * Singleton of space builder to be implemented
		 */
		public static SpaceBuilder getInstance()
		{
			if (instance == null)
			{
				instance = new SpaceBuilder();
			}

			return instance;
		}

		public static SpaceBuilderInterface get()
		{
			return getInstance().getSpaceBuilderInterface();
		}

		/////////////////////////////////////////////////////////////////////////////


		private SpaceBuilderInterface spaceBuilderInterface;

		private SpaceBuilder()
		{
		}

		public SpaceBuilderInterface getSpaceBuilderInterface()
		{
			return spaceBuilderInterface;
		}

		private void setSpaceBuilderInterface(SpaceBuilderInterface spaceBuilderInterface)
		{
			this.spaceBuilderInterface = spaceBuilderInterface;
		}
	}
}
