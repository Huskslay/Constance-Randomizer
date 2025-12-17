using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RandomizerItemDisplay.Class;

public static class Tracker
{
    private static Canvas canvas;
    private static TextMeshProUGUI locationTextPrefab;

    private static List<TrackerText> locationTexts;

    private static TextMeshProUGUI CreateText(float x, float y,
        HorizontalAlignmentOptions horizontal = HorizontalAlignmentOptions.Left,
        VerticalAlignmentOptions vertical = VerticalAlignmentOptions.Top)
    {
        TextMeshProUGUI textMeshProUGUI = new GameObject("Tracker Text").AddComponent<TextMeshProUGUI>();
        textMeshProUGUI.transform.SetParent(canvas.transform);
        textMeshProUGUI.text = "";

        textMeshProUGUI.horizontalAlignment = horizontal;
        textMeshProUGUI.verticalAlignment = vertical;
        textMeshProUGUI.fontSize = 30f;
        textMeshProUGUI.textWrappingMode = TextWrappingModes.NoWrap;

        textMeshProUGUI.margin = new Vector4(0f, 0f, 0f, 0f);
        textMeshProUGUI.rectTransform.anchorMin = Vector2.zero;
        textMeshProUGUI.rectTransform.anchorMax = Vector2.zero;
        textMeshProUGUI.transform.localPosition = new Vector3(x, y, 0f);

        return textMeshProUGUI;
    }

    public static void Init()
    {
        RandomState.onLocationGet.AddListener(OnLocationGet);
        locationTexts = [];

        canvas = new GameObject().AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = Plugin.FindFirstObjectByType<Camera>();
        canvas.sortingOrder = 100;
        canvas.transform.SetParent(Plugin.Transform);

        locationTextPrefab = CreateText(30, 0);
        locationTextPrefab.gameObject.SetActive(false);
    }

    private static void OnLocationGet(RandomStateElement element)
    {
        TextMeshProUGUI tmp = Plugin.Instantiate(locationTextPrefab, canvas.transform);
        tmp.transform.position = locationTextPrefab.transform.position;

        TrackerText trackerText = new(tmp);
        trackerText.Begin(element);

        foreach (TrackerText text in locationTexts) text.Bump();
        locationTexts.Add(trackerText);
    }

    public static void Update()
    {
        int index = 0;
        while (index < locationTexts.Count)
        {
            if (locationTexts[index].Update()) locationTexts.RemoveAt(index);
            else index++;
        }
    }
}
