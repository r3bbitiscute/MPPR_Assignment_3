using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform[] menuPanels;
    public float transitionDuration = 0.5f;
    public EasingFunctions easingFunctions;

    public void GameScene()
    {
        //Play button to change scene
        SceneManager.LoadScene(1);
    }

    /*
    Index
    - Tutorial = 0
    - Credits = 1
    - Settings = 2
    */
    public void Tutorial(int panelIndex)
    {
        // Set panel active
        StartCoroutine(TogglePanel(panelIndex));
    }

    public void Credits(int panelIndex)
    {
        // Set panel active
        StartCoroutine(TogglePanel(panelIndex));
    }

    public void Settings(int panelIndex)
    {
        // Set panel active
        StartCoroutine(TogglePanel(panelIndex));
    }

    public void Quit()
    {
        // Quit
        Application.Quit();
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panelIndex"></param>
    /// <returns></returns>
    private IEnumerator TogglePanel(int panelIndex)
    {
        // Reset time
        float time = 0f;

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
    }
}
