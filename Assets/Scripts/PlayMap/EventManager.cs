using Entity.Pacboy;
using PlayMap.UI;
using Sound;
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
        private readonly float _eventTriggerProbability = 0.05f;
        private bool _eventInProcess;
        private float _eventTimer;
        private readonly float _eventDuration = 6.0f;
        private int _currentEventIndex;

        // Singleton instance
        public static EventManager Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        private void Start() {
            // Initialise params
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

            // No action if an event is currently in process
            if (_eventInProcess) return;

            // Generate a random number to see if a new event should happen
            int eventRand = Random.Range(1, 101);
            float eventMax = 100 * _eventTriggerProbability;
            if (eventRand > eventMax) {
                // Directly return if no event
                return;
            }

            // Event should happen
            // Play event triggered sound
            SoundManager.Instance.PlaySoundOnce(SoundType.EventTriggered);

            // Params init
            _eventTimer = 0f;
            _eventInProcess = true;
            GamePlayUI.Instance.NewInfo("Random Event!", Color.yellow);

            // Prompt the player

            // Probability of three events:
            // Super Bonus 50% (index: 1)
            // Crazy Party 40% (index: 2)
            // Air Wall 10% (index: 3)
            int typeRand = Random.Range(1, 101);

            if (typeRand <= 50) {
                // Super Bonus
                _currentEventIndex = 1;
                PlayMapController.Instance.SetSuperBonus(true);
                GamePlayUI.Instance.NewInfo("Super Bonus - Try getting more points!", Color.yellow);
            } else if (typeRand <= 90) {
                // Crazy Party
                _currentEventIndex = 2;
                GhostronManager.Instance.SetCrazyParty(true);
                PlayMapController.Instance.GetPacboy().GetComponent<PacboyMovement>().SetCrazyParty(true);
                GamePlayUI.Instance.NewInfo("Crazy Party - Enjoy the chaos!", Color.yellow);
            } else {
                // Air Wall
                _currentEventIndex = 3;
                PlayMapController.Instance.SetAirWall(true);
                GamePlayUI.Instance.NewInfo("Air Wall - Good luck finding your way!", Color.yellow);
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