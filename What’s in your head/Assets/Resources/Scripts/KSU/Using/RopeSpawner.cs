using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        //public List<GameObject> playerList;

        public GameObject NellaRopeAction;
        public GameObject SteadyRopeAction;

        public float detectingRange = 30f;
        public float interactableRange = 20f;

        public float ropeLength = 15f;

        public float rotationSpeed = 180f;

        public float swingSpeed = 30f;
        public float swingAngle = 60f;

        // Start is called before the first frame update
        void Start()
        {
            InitCollider();
        }

        void InitCollider()
        {
            transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        public void StartRopeAction(GameObject player)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        NellaRopeAction.GetComponent<Rope>().player = player;
                        NellaRopeAction.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        SteadyRopeAction.GetComponent<Rope>().player = player;
                        SteadyRopeAction.SetActive(true);
                    }
                    break;
            }
            //GameObject obj = Instantiate<GameObject>(ropeAction, transform);
            //obj.GetComponent<RopeAction>().spawner = this;
            //obj.GetComponent<RopeAction>().player = player;
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {

                case "Nella":
                    {
                        return NellaRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.forward * 5f);
        }
    }
}
