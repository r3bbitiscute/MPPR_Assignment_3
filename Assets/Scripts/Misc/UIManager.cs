using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Transform[] menuPanels;
    public float transitionDuration = 0.5f;
    public EasingFunctions easingFunctions;
    private AudioSource audioSource;
    public AudioClip popSFX;

    private bool isMenuOpen = false;

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
        Debug.Log("Return to Main Menu Button Pressed!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        ClosePanel(0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game Restarted!");
    }

    public void GameOverRestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
        Debug.Log("Game Restarted from Game Over!");
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
            {
                // If the menu is open, close it
                ClosePanel(0);
                Debug.Log("Escape button pressed to close the menu.");
            }
            else
            {
                Escape(0);
                Debug.Log("Escape button pressed!");
            }
            isMenuOpen = !isMenuOpen;
        }
    }
    /*
    Index
    - Escape = o
    */
    public void Escape(int panelIndex)
    {
        // Set panel active
        StartCoroutine(TogglePanel(panelIndex));
    }

    public void ClosePanel(int panelIndex)
    {
        // Close panel for the current panel
        StartCoroutine(ClosePanelEase(panelIndex));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panelIndex"></param>
    /// <returns></returns>
    private IEnumerator ClosePanelEase(int panelIndex)
    {
        // Reset time
        float time = 0f;

        // Set Timescale to 1
        Time.timeScale = 1f;

        // Play Sound
        PlaySound();

        Transform targetPanel = menuPanels[panelIndex];

        targetPanel.localScale = Vector3.one;

        // Scale from 100% to 0% 
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            // Quadratic Ease in function
            easingFunctions.EaseIn(t);

            //Calculate the scale using quadratic ease in function
            float currentScaleX = startScale.x + (endScale.x - startScale.x) * t;
            float currentScaleY = startScale.y + (endScale.y - startScale.y) * t;
            float currentScaleZ = startScale.z + (endScale.z - startScale.z) * t;

            // Set panel scale to new calculated values
            targetPanel.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);

            yield return null;
        }

        // Ensure the final scale is exactly as expected
        targetPanel.localScale = endScale;

        // Disable panel
        targetPanel.gameObject.SetActive(false);

        // Lock and hide the cursor when the menu is closed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panelIndex"></param>
    /// <returns></returns>
    private IEnumerator TogglePanel(int panelIndex)
    {
        // Unlock the cursor and make it visible when the menu is opened
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reset time
        float time = 0f;

        // Play sound effect
        PlaySound();

        //Close Panel
        foreach (var panel in menuPanels)
        {
            panel.gameObject.SetActive(false);
        }

        Transform targetPanel = menuPanels[panelIndex];
        targetPanel.gameObject.SetActive(true);

        // Scale from 0% to 100% 
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            // Quadratic Ease out function
            easingFunctions.EaseOut(t);

            //Calculate the scale using quadratic ease out function
            float currentScaleX = startScale.x + (endScale.x - startScale.x) * t;
            float currentScaleY = startScale.y + (endScale.y - startScale.y) * t;
            float currentScaleZ = startScale.z + (endScale.z - startScale.z) * t;

            // Set panel scale to new calculated values
            targetPanel.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);

            yield return null;
        }

        // Ensure the final scale is exactly as expected
        targetPanel.localScale = endScale;

        // Delay the pause until the animation starts
        yield return null;

        // Set Timescale to 0
        Time.timeScale = 0f;
    }

    private void PlaySound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(popSFX);
        }
    }
}
