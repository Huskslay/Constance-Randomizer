using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Randomizer.Classes.UI.Elements;

public class RandoButton : MonoBehaviour
{
    public string text;
    public TextMeshProUGUI tmp;
    public Button button;
    public Action onUpdate;

    public void PreInit(TextMeshProUGUI tmp, Button button)
    {
        this.tmp = tmp;
        this.button = button;

        button.onClick.RemoveAllListeners();
        gameObject.SetActive(false);

        Plugin.DestroyImmediate(button);
        this.button = gameObject.AddComponent<Button>();
    }
    public void Init(string text, Action<RandoButton> onClick, Action onUpdate = null)
    {
        name = text;
        this.text = text;
        this.onUpdate = onUpdate;

        button.onClick.AddListener(() => onClick?.Invoke(this));
        button.gameObject.SetActive(true);
    }

    private void Update()
    {
        tmp.text = text;
        onUpdate?.Invoke();
    }
}
