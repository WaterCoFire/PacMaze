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
        public AudioClip playerWinSound;
        public AudioClip playerLoseSound;
        public AudioClip eatDotSound;
        public AudioClip pickUpGoodPropSound;
        public AudioClip pickUpBadPropSound;
        public AudioClip deployNiceBombSound;
        public AudioClip niceBombExplodeSound;
        public AudioClip ghostronCaughtSound;
        public AudioClip eventTriggeredSound;
        public AudioClip clickSound;
        public AudioClip warningSound;

        // Store all sound effect types and corresponding sound clips (not including two background musics)
        private Dictionary<SoundType, AudioClip> _allSounds;

        // AWAKE FUNCTION
        private void Awake() {
            // Set singleton instance
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject); // SoundManager should exist in all scenes
            } else {
                Destroy(gameObject);
            }
            
            // Store all sounds to the dictionary
            _allSounds = new Dictionary<SoundType, AudioClip>();
            _allSounds.Add(SoundType.PlayerWin, playerWinSound);
            _allSounds.Add(SoundType.PlayerLose, playerLoseSound);
            _allSounds.Add(SoundType.EatDot, eatDotSound);
            _allSounds.Add(SoundType.PickUpGoodProp, pickUpGoodPropSound);
            _allSounds.Add(SoundType.PickUpBadProp, pickUpBadPropSound);
            _allSounds.Add(SoundType.DeployNiceBomb, deployNiceBombSound);
            _allSounds.Add(SoundType.NiceBombExplode, niceBombExplodeSound);
            _allSounds.Add(SoundType.GhostronCaught, ghostronCaughtSound);
            _allSounds.Add(SoundType.EventTriggered, eventTriggeredSound);
            _allSounds.Add(SoundType.Click, clickSound);
            _allSounds.Add(SoundType.Warning, warningSound);
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
        
        // Stop playing background music
        // Called when player wins/loses a game
        public void StopBackgroundMusic() {
            // Stop the music
            audioSource.Pause();
        }
        

        // Play a sound once
        // Param - the type of the sound to be played
        public void PlaySoundOnce(SoundType soundType) {
            audioSource.PlayOneShot(_allSounds[soundType]);
        }
    }
}