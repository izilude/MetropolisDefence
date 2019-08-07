using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.Services
{
    [Serializable]
    public class ServiceChart : MonoBehaviour
    {
        public List<Service> Services = new List<Service>();

        private void Start()
        {
        
        }

        private void Update()
        {
            
        }

        public Service GetService(string serviceName)
        {
            foreach (Service s in Services)
            {
                if (s.Name == serviceName)
                {
                    return s;
                }
            }

            return null;
        }
    }
}
