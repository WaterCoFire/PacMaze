using Entity.Map;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace PlayMap {
    /**
     * Responsible for wall initialisation when game starts.
     */
    public class WallGenerator : MonoBehaviour {
        /* All horizontal and vertical wall game objects */
        public GameObject[,] HorizontalWalls;
        public GameObject[,] VerticalWalls;

        public GameObject mapFloor; // The floor parent GameObject
        
        // START FUNCTION
        private void Start() {
            // Initialise the two wall game object arrays
            HorizontalWalls = new GameObject[10, 11];
            VerticalWalls = new GameObject[11, 10];

            // Find the Walls root game object
            GameObject wallsRoot = GameObject.Find("Walls");

            // Load horizontal walls
            for (int row = 0; row < 10; row++) {
                string hRowName = $"HWall_Row_{row + 1}";
                Transform hRow = wallsRoot.transform.Find(hRowName);
                if (hRow == null) {
                    Debug.LogError($"Error: Not found {hRowName}");
                    continue;
                }

                for (int col = 0; col < 11; col++) {
                    string hWallName = $"HWall_{col + 1}";
                    Transform hWall = hRow.Find(hWallName);
                    if (hWall == null) {
                        Debug.LogError($"Error: Not found {hRowName}/{hWallName}");
                        continue;
                    }

                    HorizontalWalls[row, col] = hWall.gameObject;
                }
            }

            // Load vertical walls
            for (int col = 0; col < 10; col++) {
                string vColName = $"VWall_Column_{col + 1}";
                Transform vCol = wallsRoot.transform.Find(vColName);
                if (vCol == null) {
                    Debug.LogError($"Error: Not found {vColName}");
                    continue;
                }

                for (int row = 0; row < 11; row++) {
                    string vWallName = $"VWall_{row + 1}";
                    Transform vWall = vCol.Find(vWallName);
                    if (vWall == null) {
                        Debug.LogError($"Error: Not found {vColName}/{vWallName}");
                        continue;
                    }

                    VerticalWalls[row, col] = vWall.gameObject;
                }
            }
        }

        /**
         * Initialise walls, called when starting a game.
         * Sets the walls of the map.
         * Then bakes the map using NavMesh.
         */
        public void InitWalls(WallData wallData) {
            // Horizontal walls
            for (int row = 0; row < 10; row++) {
                for (int column = 0; column < 11; column++) {
                    HorizontalWalls[row, column].SetActive(wallData.HorizontalWallStatus[row, column]);
                }
            }
           
            // Vertical walls
            for (int row = 0; row < 11; row++) {
                for (int column = 0; column < 10; column++) {
                    VerticalWalls[row, column].SetActive(wallData.VerticalWallStatus[row, column]);
                }
            }

            // Use collider data to build navigation mesh
            NavMeshSurface navMeshSurface = mapFloor.GetComponent<NavMeshSurface>();

            if (navMeshSurface == null) {
                Debug.LogError("Floor has no NavMeshSurface component!");
                return;
            }

            // Bake NavMesh
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.BuildNavMesh();
        }
    }
}