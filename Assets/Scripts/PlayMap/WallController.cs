using Entity.Map;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace PlayMap {
    public class WallController : MonoBehaviour {
        public GameObject[] horizontalWalls1;
        public GameObject[] horizontalWalls2;
        public GameObject[] horizontalWalls3;
        public GameObject[] horizontalWalls4;
        public GameObject[] horizontalWalls5;
        public GameObject[] horizontalWalls6;
        public GameObject[] horizontalWalls7;
        public GameObject[] horizontalWalls8;
        public GameObject[] horizontalWalls9;
        public GameObject[] horizontalWalls10;

        public GameObject[] verticalWalls1;
        public GameObject[] verticalWalls2;
        public GameObject[] verticalWalls3;
        public GameObject[] verticalWalls4;
        public GameObject[] verticalWalls5;
        public GameObject[] verticalWalls6;
        public GameObject[] verticalWalls7;
        public GameObject[] verticalWalls8;
        public GameObject[] verticalWalls9;
        public GameObject[] verticalWalls10;
        public GameObject[] verticalWalls11;

        public GameObject mapFloor;

        /**
         * Sets the walls of the map.
         * Then bakes the map using NavMesh.
         */
        public void InitWalls(WallData wallData) {
            for (int column = 0; column < 11; column++) {
                horizontalWalls1[column].SetActive(wallData.HorizontalWallStatus[0, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls2[column].SetActive(wallData.HorizontalWallStatus[1, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls3[column].SetActive(wallData.HorizontalWallStatus[2, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls4[column].SetActive(wallData.HorizontalWallStatus[3, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls5[column].SetActive(wallData.HorizontalWallStatus[4, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls6[column].SetActive(wallData.HorizontalWallStatus[5, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls7[column].SetActive(wallData.HorizontalWallStatus[6, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls8[column].SetActive(wallData.HorizontalWallStatus[7, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls9[column].SetActive(wallData.HorizontalWallStatus[8, column]);
            }

            for (int column = 0; column < 11; column++) {
                horizontalWalls10[column].SetActive(wallData.HorizontalWallStatus[9, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls1[column].SetActive(wallData.VerticalWallStatus[0, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls2[column].SetActive(wallData.VerticalWallStatus[1, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls3[column].SetActive(wallData.VerticalWallStatus[2, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls4[column].SetActive(wallData.VerticalWallStatus[3, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls5[column].SetActive(wallData.VerticalWallStatus[4, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls6[column].SetActive(wallData.VerticalWallStatus[5, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls7[column].SetActive(wallData.VerticalWallStatus[6, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls8[column].SetActive(wallData.VerticalWallStatus[7, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls9[column].SetActive(wallData.VerticalWallStatus[8, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls10[column].SetActive(wallData.VerticalWallStatus[9, column]);
            }

            for (int column = 0; column < 10; column++) {
                verticalWalls11[column].SetActive(wallData.VerticalWallStatus[10, column]);
            }

            // Use collider data to build navigation mesh
            mapFloor.GetComponent<NavMeshSurface>().useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            mapFloor.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}