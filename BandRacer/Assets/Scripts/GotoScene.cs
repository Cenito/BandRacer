using UnityEngine;
using System.Collections;

public class GotoScene : MonoBehaviour
{
    public string NameOfRaceScene;
    public string NameOfIntroScene;

    public void GotoRaceScene()
    {
        StartCoroutine(GotoRaceSceneWithFadeSound());
    }

    private IEnumerator GotoRaceSceneWithFadeSound()
    {
        var soundSource = GetComponent<AudioSource>();
        while (soundSource.volume > 0.05)
        {
            yield return new WaitForEndOfFrame();
            soundSource.volume = Mathf.Lerp(soundSource.volume, 0f, Time.deltaTime);
        }

        Application.LoadLevel(NameOfRaceScene);
        yield return null;
    }

    public void GotoIntroScene()
    {
        Application.LoadLevel(NameOfIntroScene);
    }
}
