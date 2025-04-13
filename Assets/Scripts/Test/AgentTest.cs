using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class AgentTest : MonoBehaviour {
        public Transform destination;

        private void Start() {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(destination.position);
        }
    }
}