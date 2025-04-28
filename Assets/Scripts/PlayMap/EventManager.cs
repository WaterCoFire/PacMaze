using System;
using Entity.Pacboy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayMap {
    /**
     * Event Manager
     * In PacMaze there could be three types of events:
     * - Super Bonus (All scores awarded are doubled)
     * - Crazy Party (Pacboy and Ghostrons are way faster)
     * - Air Walls (Walls become invisible)
     * Every event lasts for ten seconds
     */
    public class EventManager : MonoBehaviour {
        private bool _eventEnabled;
        private readonly float _eventTriggerProbability = 0.5f;
        private bool _eventInProcess;
        private float _eventTimer;
        private readonly float _eventDuration = 10.0f;
        private int _currentEventIndex;

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
            _currentEventIndex = 0;
            _eventInProcess = false;
        }

        // UPDATE FUNCTION
        private void Update() {
            // No action if event disabled & event is not in process
            if (!_eventEnabled) return;
            if (!_eventInProcess) return;

            // Event is in process
            // Update timer
            _eventTimer += Time.deltaTime;

            // Check if the event time is over
            // If so, terminate the event effect
            if (_eventTimer >= _eventDuration) {
                switch (_currentEventIndex) {
                    case 1:
                        // Super Bonus
                        PlayMapController.Instance.SetSuperBonus(false);
                        break;
                    case 2:
                        // Crazy Party
                        GhostronManager.Instance.SetCrazyParty(false);
                        PlayMapController.Instance.GetPacboy().GetComponent<PacboyMovement>().SetCrazyParty(false);
                        break;
                    case 3:
                        // Air Wall
                        PlayMapController.Instance.SetAirWall(false);
                        break;
                    default:
                        Debug.LogError("Error: Invalid event index!");
                        break;
                }

                _currentEventIndex = 0;
                _eventInProcess = false;
                _eventTimer = 0f;
            }
        }

        /**
         * Action when a new dot is eaten.
         * Use random number logic to decide if an event is triggered
         * and if so, what event to trigger.
         */
        public void DotEaten() {
            // No action if event disabled
            if (!_eventEnabled) return;

            // No action if a event is currently in process
            if (_eventInProcess) return;

            // Generate a random number to see if a new event should happen
            int rand = Random.Range(1, 101);
            float eventMax = 100 * _eventTriggerProbability;
            if (rand > eventMax) return; // No event

            // Event should happen
            // Probability of three events:
            // Super Bonus 40% (index: 1)
            // Crazy Party 40% (index: 2)
            // Air Wall 20% (index: 3)
            rand = Random.Range(1, 101);

            if (rand <= 40) {
                // Super Bonus
                _currentEventIndex = 1;
                _eventTimer = 0f;
                _eventInProcess = true;
                PlayMapController.Instance.SetSuperBonus(true);
            } else if (rand <= 80) {
                // Crazy Party
                _currentEventIndex = 2;
                _eventTimer = 0f;
                _eventInProcess = true;
                GhostronManager.Instance.SetCrazyParty(true);
                PlayMapController.Instance.GetPacboy().GetComponent<PacboyMovement>().SetCrazyParty(true);
            } else {
                // Air Wall
                _currentEventIndex = 3;
                _eventTimer = 0f;
                _eventInProcess = true;
                PlayMapController.Instance.SetAirWall(true);
            }
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