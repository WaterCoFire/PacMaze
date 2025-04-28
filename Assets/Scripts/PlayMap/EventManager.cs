using System;
using UnityEngine;

namespace PlayMap {
    /**
     * Event Manager
     * In PacMaze there could be three types of events:
     * - Air Walls (Walls become invisible)
     * - Super Bonus (All scores awarded are doubled)
     * - Crazy Party (Pacboy and Ghostrons are way faster)
     * Every event lasts for ten seconds
     */
    public class EventManager : MonoBehaviour {
        private bool _eventEnabled;
        private float _eventTriggerProbability = 0.1f;
        private bool _eventInProcess;

        // Singleton instance
        public static EventManager Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("EventManager AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            Debug.Log("EventManager START");
            _eventInProcess = false;
        }

        /**
         * Action when a new dot is eaten.
         * Use random number logic to decide if an event is triggered
         * and if so, what event to trigger.
         */
        public void DotEaten() {
            // No action if a event is currently in process
            if (_eventInProcess) return;
            
            // TODO Corresponding logic
            Debug.Log("Time for a new event");
        }

        /**
         * Sets the event status (enabled/disabled).
         * Called by PlayMapController when initialising a game.
         */
        public void SetEventStatus(bool eventEnabled) {
            _eventEnabled = eventEnabled;
        }
    }
}