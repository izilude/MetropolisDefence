using System.Collections.Generic;

namespace Assets.RTSCore.TechTree
{
    public class TechTree
    {
        private Dictionary<string, Technology> _technologies = new Dictionary<string, Technology>();

        public void CreateTechnology(string nameOfTech, Technology technology)
        {
            if (!_technologies.ContainsKey(nameOfTech))
            {
                _technologies.Add(nameOfTech, technology);
            }
        }

        public void CreateTechnologyDependence(string nameOfTechnology, string nameOfPrerequisiteTechnology)
        {
            if (_technologies.ContainsKey(nameOfTechnology) && _technologies.ContainsKey(nameOfPrerequisiteTechnology))
            {
                _technologies[nameOfTechnology].PreRequisites.Add(nameOfPrerequisiteTechnology);
            }
        }

        public Technology GetTechnology(string nameOfTechnology)
        {
            if (_technologies.ContainsKey(nameOfTechnology))
            {
                return _technologies[nameOfTechnology];
            }

            return null;
        }

        public void LearnTechnology(string nameOfTechnology)
        {
            if (_technologies.ContainsKey(nameOfTechnology))
            {
                _technologies[nameOfTechnology].Known = true;
            }
        }

        public bool IsTechnologyKnown(string nameOfTechnology)
        {
            if (_technologies.ContainsKey(nameOfTechnology))
            {
                return _technologies[nameOfTechnology].Known;
            }

            return false;
        }

        public bool CheckTechnologyDependencies(string nameOfTechnology)
        {
            foreach (string x in _technologies[nameOfTechnology].PreRequisites)
            {
                if (_technologies[x].Known == false)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
