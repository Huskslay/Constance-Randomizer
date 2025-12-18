using RandomizerCore.Classes.State;
using TMPro;
using UnityEngine;

namespace RandomizerItemDisplay.Class;

public class TrackerText(TextMeshProUGUI text)
{
    private readonly TextMeshProUGUI text = text;

    private static readonly float fadeScale = 0.5f;

    private float timeUntilFade = 15f;
    private readonly float bumpHeight = 40f;

    public void Begin(RandomStateElement element)
    {
        text.text = $"Got: {element.dest.GetDisplayItemName()}";
        text.gameObject.SetActive(true);
    }

    public void Bump()
    {
        text.transform.localPosition += new Vector3(0, bumpHeight, 0);
    }

    public bool Update()
    {
        if (timeUntilFade > 0)
        {
            timeUntilFade -= Time.deltaTime;
            return false;
        }

        text.color = new(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * fadeScale);
        if (text.color.a <= 0)
        {
            Plugin.Destroy(text.gameObject);
            return true;
        }
        return false;
    }
}