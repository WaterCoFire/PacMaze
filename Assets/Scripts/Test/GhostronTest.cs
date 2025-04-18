using System;
using Entity.Ghostron;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class GhostronTest : MonoBehaviour {
        public Transform destination;
        public GameObject pacman;

        private Material _normalMaterial;
        public Material scaredMaterial;

        private void Start() {
            _normalMaterial = gameObject.GetComponent<SkinnedMeshRenderer>().material;

            gameObject.GetComponent<Ghostron>().SetPacman(pacman);
            gameObject.GetComponent<Ghostron>().SetGhostronParams(3f, 1f, 4f, 10f);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.P)) {
                gameObject.GetComponent<SkinnedMeshRenderer>().material = scaredMaterial;
            }

            if (Input.GetKeyDown(KeyCode.O)) {
                gameObject.GetComponent<SkinnedMeshRenderer>().material = _normalMaterial;
            }
        }
    }
}