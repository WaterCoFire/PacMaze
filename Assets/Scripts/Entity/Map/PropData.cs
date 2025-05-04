using System.Collections.Generic;
using UnityEngine;

namespace Entity.Map {
    public struct PropData {
        public readonly Dictionary<Vector3, GameObject> PropOnTiles; // Prop on every tile

        // FIXED counts of all the props
        public readonly Dictionary<string, int> FixedPropCounts;

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        public readonly Dictionary<string, int> TotalPropCounts;
        
        public PropData(Dictionary<Vector3, GameObject> propOnTiles, Dictionary<string, int> fixedPropCounts, Dictionary<string, int> totalPropCounts) {
            PropOnTiles = propOnTiles;
            FixedPropCounts = fixedPropCounts;
            TotalPropCounts = totalPropCounts;
        }
    }
}