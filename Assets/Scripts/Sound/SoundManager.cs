using System.Collections.Generic;
using UnityEngine;

namespace Sound {
    /**
     * This script manages all musics & sound effects in PacMaze
     */
    public class SoundManager : MonoBehaviour {
        // Singleton instance
        public static SoundManager Instance { get; private set; }

        // Audio source of the game
        public AudioSource audioSource;

        /* All sounds */
        // Background musics
        public AudioClip homePageBackgroundMusic;
        public AudioClip gameBackgroundMusic;

        // Sound effects
        public AudioClip clickSound;
        public AudioClip playerWinSound;
        public AudioClip playerLoseSound;
        public AudioClip eatDotSound;
        public AudioClip pickUpGoodPropSound;
        public AudioClip pickUpBadPropSound;
        public AudioClip deployNiceBombSound;
        public AudioClip niceBombExplodeSound;
        public AudioClip ghostronCaughtSound;
        public AudioClip eventTriggeredSound;

        // Store all sound effects (not including two background musics)
        private List<AudioClip> _allSounds;

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject); // SoundManager should exist in all scenes
            } else {
                Destroy(gameObject);
            }
            
            // Store all sounds to the list
            _allSounds = new List<AudioClip>();
            _allSounds.Add(clickSound);
            _allSounds.Add(playerWinSound);
            _allSounds.Add(playerLoseSound);
            _allSounds.Add(eatDotSound);
            _allSounds.Add(pickUpGoodPropSound);
            _allSounds.Add(pickUpBadPropSound);
            _allSounds.Add(deployNiceBombSound);
            _allSounds.Add(niceBombExplodeSound);
            _allSounds.Add(ghostronCaughtSound);
            _allSounds.Add(eventTriggeredSound);
        }

        /* Play audio functions */
        // Play background music
        // Param - forGame: true if to play game background music, false if to play menu background music
        public void PlayBackgroundMusic(bool forGame) {
            // Play the correct music clip
            AudioClip clipToPlay = forGame ? gameBackgroundMusic : homePageBackgroundMusic;

            if (audioSource.clip != clipToPlay) {
                // Play if the current playing clip is not the expected clip
                audioSource.clip = clipToPlay;
                audioSource.loop = true;
                audioSource.Play();
            } else if (!audioSource.isPlaying) {
                // If currently not playing, play the clip as well
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        // Play a sound once
        // Param - soundIndex:
        // 0 - Click
        // 1 - Player win
        // 2 - Player lose
        // 3 - Eat dot
        // 4 - Pick up good prop
        // 5 - Pick up bad prop
        // 6 - Deploy Nice Bomb
        // 7 - Nice Bomb explode
        // 8 - Ghostron caught
        // 9 - Event triggered
        public void PlaySoundOnce(int soundIndex) {
            audioSource.PlayOneShot(_allSounds[soundIndex]);
        }
    }
}