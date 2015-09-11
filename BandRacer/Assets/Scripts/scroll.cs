using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Collections;

public class scroll : MonoBehaviour
{
    public string intro;
    public float off;
    public int speed = 100;
    public float waveLength = 100f;
    public float waveHeight = 40f;
    public float yOffset = 0;
    public float fontSize = 20;

    private bool fadeOut;
    private float fadeOutAlph = 0;

    void Start()
    {
        intro.Reverse().ToString();
    }

    public void OnGUI()
    {
        off += Time.deltaTime * speed;
        float characterOffset = fontSize*1.2f;
        for (int i = 0; i < intro.Length; i++)
        {
            float roff = (intro.Length * -characterOffset) + (i * characterOffset + off);

            float alph = Mathf.Sin((roff / Screen.width) * 180 * Mathf.Deg2Rad);

            GUI.color = new Color(1, 1, 1, alph);

            float yPos = Mathf.Sin(roff / waveLength * 180 * Mathf.Deg2Rad) * waveHeight + yOffset;

            var position = new Rect(Screen.width - roff, yPos, Screen.width, characterOffset);
            
            GUI.Label(position, GetFormattedString(intro[intro.Length - 1 - i].ToString()));

            if (position.xMax < (-fontSize * intro.Length) && i == (intro.Length - 1))
            {
                off = 0f;
            }

            GUI.color = new Color(1, 1, 1, 1);
        }

        if (fadeOut)
        {
            //alph = Mathf.Lerp(alph, 0f, Time.deltaTime);
            fadeOutAlph = Mathf.Lerp(fadeOutAlph, 1f, Time.deltaTime);
            var color = new Color(0, 0, 0, fadeOutAlph);
            DrawQuad(new Rect(0, 0, Screen.width, Screen.height), color);
        }   
    }

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    private string GetFormattedString(string text)
    {
        return string.Format("<size={0}>{1}</size>", fontSize, text);
    }

    public void FadeOutText()
    {
        fadeOutAlph = 0;
        fadeOut = true;
    }

    public void FadeInText()
    {
        fadeOut = false;
    }
}
