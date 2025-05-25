using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayMap.UI {
    /**
     * Manages the UI during game.
     */
    public class GamePlayUI : MonoBehaviour {
        /* Score Display */
        public TMP_Text scoreText; // Score text

        /* Super Bonus */
        public GameObject superBonusPrompt; // Prompt when Super Bonus is triggered

        /* Nice Bomb UI */
        // Displayed when the player has nice bombs (This button cannot be actually clicked)
        public Button niceBombButton;

        public TMP_Text niceBombNumText; // Inside the button, shows the number of nice bombs the player currently has
        public TMP_Text cooldownPromptText; // Inside the button, shows the cooldown status
        public TMP_Text useNiceBombKeyPrompt; // Prompt indicating which key to press to use the nice bomb
        public TMP_Text deployNiceBombKeyPrompt; // Prompt indicating which key to press to deploy the nice bomb

        /* Info Panel */
        public GameObject infoPanel; // The panel container
        public GameObject messagePrefab; // The prefab for each message prefab

        private const float DisplayDuration = 5f; // The display duration for each message
        private const float FadeOutDuration = 0.5f; // The fade out duration for each message

        /* Other UI Pages */
        public GameObject pausePage; // Pause page
        public GameObject winPage; // Game over page: player wins
        public GameObject losePage; // Game over page: player loses

        // Nice bomb operation KeyCodes
        private KeyCode _useNiceBombKeyCode; // Use (default: E)
        private KeyCode _deployNiceBombKeyCode; // Deploy (default: F)

        // Singleton instance
        public static GamePlayUI Instance { get; private set; }

        // AWAKE FUNCTION
        private void Awake() {
            Debug.Log("GamePlayUI AWAKE");
            // Set singleton instance
            Instance = this;
        }

        // START FUNCTION
        // UI initialisation
        private void Start() {
            Debug.Log("GamePlayUI START");

            // Reset score UI
            scoreText.text = "Score: 0";
            scoreText.color = Color.white;
            superBonusPrompt.SetActive(false);

            // Reset Nice Bomb operation UI
            niceBombNumText.text = "0";
            cooldownPromptText.gameObject.SetActive(false);
            niceBombButton.gameObject.SetActive(false);

            // Set status of other pages
            pausePage.SetActive(false);
            winPage.SetActive(false);
            losePage.SetActive(false);
            
            // TEST ONLY
            // StartCoroutine(TestMessages());
        }

        /**
         * Display a new message in info panel (bottom left).
         */
        public void NewInfo(string message, Color textColor) {
            // Instantiate the message prefab
            GameObject newMessageObj = Instantiate(messagePrefab, infoPanel.transform);
            newMessageObj.transform.SetAsLastSibling(); // Make sure the new bottom is at the bottom

            // Set text content and color
            TextMeshProUGUI textComponent = newMessageObj.GetComponent<TextMeshProUGUI>();
            if (textComponent != null) {
                textComponent.text = message;
                textComponent.color = textColor;
            } else {
                Debug.LogError("Message prefab does not have a TextMeshProUGUI component!");
                Destroy(newMessageObj);
                return;
            }

            // Start a coroutine to handle the lifecycle of the message (display, fade, destroy)
            StartCoroutine(MessageLifecycleCoroutine(newMessageObj, textComponent));
        }

        /**
         * The coroutine of an info message lifecycle.
         */
        private IEnumerator MessageLifecycleCoroutine(GameObject messageObject, TextMeshProUGUI textComponent) {
            // Wait for a fixed amount of time for the display
            // (the fade-out time is subtracted, as the fade-out counts as part of the display)
            yield return new WaitForSeconds(DisplayDuration - FadeOutDuration);

            // Start fading out
            float timer = 0f;
            Color originalColor = textComponent.color;
            Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            while (timer < FadeOutDuration) {
                if (textComponent == null) yield break;

                timer += Time.deltaTime;
                textComponent.color = Color.Lerp(originalColor, transparentColor, timer / FadeOutDuration);
                yield return null; // Wait for the next frame
            }

            // Ensure complete transparent, and then destroy the message object
            if (textComponent != null) {
                textComponent.color = transparentColor;
            }

            if (messageObject != null) {
                Destroy(messageObject);
            }
        }

        /**
         * Update the key binding and prompt UI.
         * Let the player know which key to press to use/deploy Nice Bombs.
         */
        public void UpdateNiceBombOperationKeyPrompts(KeyCode useKeyCode, KeyCode deployKeyCode) {
            useNiceBombKeyPrompt.text = "Use    " + useKeyCode;
            deployNiceBombKeyPrompt.text = "Deploy " + deployKeyCode;
        }

        /**
         * Sets the appearance of Nice Bomb UI that indicates that Nice Bomb is on cooldown.
         * Called by PacboyPropOperation:
         * "true" should be passed when the player just uses/deploys a Nice Bomb.
         * "false" should be passed when the cooldown is over.
         */
        public void SetNiceBombCooldown(bool onCooldown) {
            cooldownPromptText.gameObject.SetActive(onCooldown);
            niceBombButton.interactable = !onCooldown;
        }

        /**
         * Displays/Hides the Nice Bomb prompt UI.
         * Called by PacboyPropOperation when Pacboy has 0 to 1 or 1 to 0 Nice Bomb.
         */
        public void SetNiceBombPrompt(bool on) {
            niceBombButton.gameObject.SetActive(on);
        }

        /**
         * Updates the number of Nice Bomb that Pacboy currently has.
         * Called by PacboyPropOperation everytime that number changes.
         */
        public void UpdateNiceBombNum(int num) {
            niceBombNumText.text = num.ToString();
        }

        /**
         * Updates the score text.
         * Called by PlayMapController everytime the player's score changes.
         */
        public void UpdateScoreText(int score) {
            scoreText.text = "Score: " + score;
        }

        /**
         * Sets on/off the Super Bonus event UI effect (yellow score text/prompt).
         * Called by PlayMapController when the Super Bonus event starts/ends.
         */
        public void SetSuperBonusEffect(bool on) {
            // Yellow (on) or White (off)
            scoreText.color = on ? Color.yellow : Color.white;
            superBonusPrompt.SetActive(on);
        }

        /**
         * Displays/Hides the pause page.
         * Called by PlayMapController when the game pauses/resumes.
         */
        public void SetPausePage(bool on) {
            pausePage.SetActive(on);
        }

        /**
         * Displays the win page.
         * Called by PlayMapController when the player wins.
         */
        public void DisplayWinPage() {
            winPage.SetActive(true);
            winPage.GetComponent<WinPage>().UpdateText();
        }

        /**
         * Displays the lose page.
         * Called by PlayMapController when the player loses.
         */
        public void DisplayLosePage() {
            losePage.SetActive(true);
        }

        /* TEST ONLY */
        private IEnumerator TestMessages() {
            yield return new WaitForSeconds(2f);
            NewInfo("Boom! A Ghostron stepped on the bomb you deployed and two Ghostrons are gone!", Color.green);
            yield return new WaitForSeconds(1.5f);
            NewInfo("2222 Warning Yellow", Color.yellow);
            yield return new WaitForSeconds(3f);
            NewInfo("3333 Serious Warning Red", Color.red);
        }
    }
}