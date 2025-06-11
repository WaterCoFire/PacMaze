using System.Collections.Generic;
using Entity.Prop;
using UnityEngine;

namespace Entity.Map {
    /**
     * Struct for storing the prop (types, nums, positions) data of a map.
     */
    public struct PropData {
        public readonly Dictionary<Vector3, PropType> PropOnTiles; // Prop on every tile

        // FIXED counts of all the props
        public readonly Dictionary<PropType, int> FixedPropCounts;

        // TOTAL counts of all the props - including FIXED and RANDOM ones
        public readonly Dictionary<PropType, int> TotalPropCounts;
        
        // Constructor
        public PropData(Dictionary<Vector3, PropType> propOnTiles, Dictionary<PropType, int> fixedPropCounts, Dictionary<PropType, int> totalPropCounts) {
            PropOnTiles = propOnTiles;
            FixedPropCounts = fixedPropCounts;
            TotalPropCounts = totalPropCounts;
        }
    }
}