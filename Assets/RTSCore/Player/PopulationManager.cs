using System.Collections.Generic;
using Assets.RTSCore.WorldObject.Buildings;

namespace Assets.RTSCore.Player
{
	public static class PopulationManager
	{
		public static void Run(List<Building> worldObjects) 
		{
			int populationToDistribute = TotalPopulation (worldObjects);

			foreach (Building wob in worldObjects) 
			{
				if (wob is House) { continue; }

				populationToDistribute -= wob.MaxPopulation;
				int pop = wob.MaxPopulation;
				if (populationToDistribute < 0) 
				{
					pop = pop + populationToDistribute;
                    populationToDistribute = 0;
				}

				wob.CurrentPopulation = pop;
			}
		}

		public static int TotalPopulation(List<Building> worldObjects)
		{
			int population = 0;
			foreach (Building wob in worldObjects) 
			{
				if (wob is House)
				{
					population += wob.CurrentPopulation;
				}
			}
				
			return population;
		}
	}
}

