using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFadeInOut : MonoBehaviour
{
    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    private bool sceneStarting = true;      // Whether or not the scene is still fading in.

    void Update()
    {
        // If the scene is starting...
        if (sceneStarting)
            // ... call the StartScene function.
            StartScene();
    }

    void Awake()
    {
        var image = GetComponent<Image>();
        image.color = Color.black;
    }

    void FadeToClear()
    {
        var image = GetComponent<Image>();
        
        // Lerp the colour of the texture between itself and transparent.
        image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack()
    {
        var image = GetComponent<Image>();
        // Lerp the colour of the texture between itself and black.
        image.color = Color.Lerp(image.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    void StartScene()
    {
        var image = GetComponent<Image>();
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (image.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
            image.color = Color.clear;
            image.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
    }

    public void EndScene()
    {
        StartCoroutine(EndSceneLoop());
    }

    private IEnumerator EndSceneLoop()
    {
        var image = GetComponent<Image>();
        // Make sure the texture is enabled.
        image.enabled = true;

        // Start fading towards black.
        FadeToBlack();

        // If the screen is almost black...
        while (image.color.a <= 0.95f)
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(EndSceneLoop());
        }

        //Application.LoadLevel(1);
        yield return null;
    }
}
