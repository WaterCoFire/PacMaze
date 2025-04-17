using System;
using Entity.Ghost;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class GhostTest : MonoBehaviour {
        public Transform destination;
        public GameObject pacman;

        private void Start() {
            gameObject.GetComponent<Ghost>().SetPacman(pacman);
            gameObject.GetComponent<Ghost>().SetGhostParams(3f, 4f, 10f);
        }
    }
}