using System;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class MapTest : MonoBehaviour {
        private void Start() {
            Debug.Log("1");
            gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}