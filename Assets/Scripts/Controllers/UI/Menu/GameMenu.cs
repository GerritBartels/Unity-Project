using Controllers.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers.UI.Menu
{
    /// <summary>
    /// <c>GameMenu</c> defines UI events for the main game. This includes a pause, settings and game over menu.
    /// </summary>
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject gameOverMenuUI;
        [SerializeField] private GameObject playerResourcesUI;
        [SerializeField] private GameObject playerAbilitiesUI;
        [SerializeField] private GameObject settingsMenuUi;
        [SerializeField] private GameObject player;

        private static bool _gameIsPaused = false;
        private Camera _cam;
        private static readonly int Dead = Animator.StringToHash("dead");
        private PlayerController _playerController;
        private Animator _animator;

        private void Awake()
        {
            _playerController = player.GetComponent<PlayerController>();
            Cursor.visible = false;
            _cam = Camera.main;
            _animator = _cam.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && player.GetComponent<PlayerController>().PlayerModel.IsAlive)
            {
                if (_gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void OnApplicationQuit()
        {
            // Delete player stats and level
            PlayerPrefs.DeleteKey("Strength");
            PlayerPrefs.DeleteKey("Agility");
            PlayerPrefs.DeleteKey("Intelligence");
            PlayerPrefs.DeleteKey("Level");
            Debug.Log("Application ending after " + Time.time + " seconds");
        }

        /// <summary>
        /// <c>Pause</c> activates the pause menu and pauses the game by freezing time.
        /// Also deactivates the player's resource and ability UI.
        /// </summary>
        private void Pause()
        {
            // Disable PlayerController script to avoid casting abilities through Key events triggered during the pause screen
            _playerController.enabled = false;
            Cursor.visible = true;
            playerAbilitiesUI.SetActive(false);
            playerResourcesUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _gameIsPaused = true;
        }

        /// <summary>
        /// <c>Resume</c> deactivates the pause menu and resumes the game by unfreezing time.
        /// It is also hooked up to the "Resume" button and activates the player's resource and ability UI.
        /// </summary>
        public void Resume()
        {
            _playerController.enabled = true;
            Cursor.visible = false;
            pauseMenuUI.SetActive(false);
            settingsMenuUi.SetActive(false);
            playerAbilitiesUI.SetActive(true);
            playerResourcesUI.SetActive(true);
            Time.timeScale = 1f;
            _gameIsPaused = false;
        }

        /// <summary>
        /// <c>LoadMenu</c> is hooked up to the "Menu" button which, when clicked, loads the previous scene.
        /// </summary>
        public void LoadMenu()
        {
            _gameIsPaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        /// <summary>
        /// <c>QuitGame</c> is hooked up to the "Quit" button which, when clicked, closes the application.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quit!");
            Application.Quit();
        }

        /// <summary>
        /// <c>GameOver</c> activates the game over menu and triggers the camera's game over animation.
        /// Also deactivates the player's resource and ability UI.
        /// </summary>
        public void GameOver()
        {
            Cursor.visible = true;
            playerAbilitiesUI.SetActive(false);
            playerResourcesUI.SetActive(false);
            gameOverMenuUI.SetActive(true);
            _animator.SetTrigger(Dead);
            // Destroy all remaining enemies
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies == null) return;
            foreach (var enemy in enemies)
            {
                Destroy(enemy);
            }
        }

        /// <summary>
        /// <c>Restart</c> is hooked up to the "Restart" button which, when clicked, restarts the level.
        /// </summary>
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}