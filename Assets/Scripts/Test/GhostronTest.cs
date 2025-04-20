using System;
using Entity.Ghostron;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class GhostronTest : MonoBehaviour {
        public GameObject obj;

        private void Start() {
            GetComponent<NavMeshAgent>().SetDestination(obj.transform.position);
        }

        private void Update() {
        }
    }
}