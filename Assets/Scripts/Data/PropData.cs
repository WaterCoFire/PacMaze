using System.Collections.Generic;
using UnityEngine;

namespace Data {
    public struct PropData {
        public Dictionary<Vector3, GameObject> PropOnTile; // Prop on every tile

        // FIXED counts of all the props
        public Dictionary<string, int> FixedPropCounts;

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        public Dictionary<string, int> TotalPropCounts;
        
        public PropData(Dictionary<Vector3, GameObject> propOnTile, Dictionary<string, int> fixedPropCounts, Dictionary<string, int> totalPropCounts) {
            PropOnTile = propOnTile;
            FixedPropCounts = fixedPropCounts;
            TotalPropCounts = totalPropCounts;
        }
    }
}