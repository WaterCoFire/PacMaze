using System;
using UnityEngine;

namespace Test {
    public class WallTest : MonoBehaviour {
        public GameObject[,] HorizontalWalls;
        public GameObject[,] VerticalWalls;

        private void Start() {
            // Initialise the two wall game object arrays
            HorizontalWalls = new GameObject[10, 11];
            VerticalWalls = new GameObject[11, 10];
            
            // Find the Walls root game object
            GameObject wallsRoot = GameObject.Find("Walls");
            Debug.Log(wallsRoot);
            
            // Load horizontal walls
            for (int row = 0; row < 10; row++) {
                string hRowName = $"HWall_Row_{row + 1}";
                Transform hRow = wallsRoot.transform.Find(hRowName);
                if (hRow == null) {
                    Debug.LogError($"Error: Not found {hRowName}");
                    continue;
                }

                for (int column = 0; column < 11; column++) {
                    string hWallName = $"HWall_{column + 1}";
                    Transform hWall = hRow.Find(hWallName);
                    if (hWall == null) {
                        Debug.LogError($"Error: Not found {hRowName}/{hWallName}");
                        continue;
                    }
                    
                    HorizontalWalls[row, column] = hWall.gameObject;
                }
            }

            // Load vertical walls
            for (int column = 0; column < 10; column++) {
                string vColName = $"VWall_Column_{column + 1}";
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

                    VerticalWalls[row, column] = vWall.gameObject;
                }
            }
        }
    }
}