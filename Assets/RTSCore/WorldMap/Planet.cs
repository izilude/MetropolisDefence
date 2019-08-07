using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.WorldMap
{
    public class Planet : MonoBehaviour {

        public float speed;
        private bool _mouseDown;

        private bool _autoRotate = true;

        public List<LandingSite> LandingSites = new List<LandingSite>();

        // Use this for initialization
        void Start () 
        {
	
        }

        public void ResumePlanetView()
        {
            _autoRotate = true;
            _mouseDown = false;
        }
	
        // Update is called once per frame
        void Update () 
        {
            if (_autoRotate) transform.RotateAround(new Vector3(0, 0, 0), new Vector3(-0.7f, 1, 0), 0.1f);

            if (Input.GetMouseButtonDown (0))
            {
                _mouseDown = true;		
            } 
            else if(Input.GetMouseButtonUp(0) )
            {
                _mouseDown = false;
            }

            if (_mouseDown)
            {
                _autoRotate = false;
                this.transform.Rotate (new Vector3 (Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0) * Time.deltaTime * speed);
            }
        }

        public void AddLandingSite(bool easy, int difficultyRating)
        {
            LandingSite landingSite;
            if (easy)
            {
                landingSite = Instantiate(Game.Game.Instance.WorldMap.EasyLandingSite);
            }
            else
            {
                landingSite = Instantiate(Game.Game.Instance.WorldMap.HardLandingSite);
            }

            landingSite.transform.parent = this.transform;
            landingSite.GenerateRandom(difficultyRating);
            landingSite.enabled = true;
            LandingSites.Add(landingSite);
        }
    }
}
