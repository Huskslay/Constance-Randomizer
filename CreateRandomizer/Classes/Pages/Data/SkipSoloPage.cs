using CheatMenu.Classes;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Skips;
using System;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Data;

public class SkipSoloPage : SoloGUIPage
{
    public override string Name => SkipData == null ? "null" : SkipData.skip.ToString();

    public SkipData SkipData { get; private set; }

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        windowRect.height = 600f;
        windowRect.width = 600f;
    }

    public void Open(SkipData skipData)
    {
        SkipData = skipData;
        Open();
    }
    public override void Open()
    {
        if (SkipData == null) return;
        base.Open();
    }

    public override void UpdateOpen()
    {
        base.UpdateOpen();
        if (SkipData == null) return;

        for (int i = 0; i < SkipData.neededItems.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Needed Items");
            if (GUILayout.Button("x", GUILayout.Width(50)))
            {
                SkipData.neededItems.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();

            Color bg = GUI.backgroundColor;
            GUILayout.BeginHorizontal();
            int index = 0;
            foreach (ItemEntries value in Enum.GetValues(typeof(ItemEntries)))
            {
                if (value == ItemEntries.None || value == ItemEntries.All) continue;
                if (index++ == 7)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    index = 0;
                }
                bool enabled = SkipData.neededItems[i].HasFlag(value);
                GUI.backgroundColor = enabled ? Color.green : Color.red;
                if (GUILayout.Button(value.ToString())) SkipData.neededItems[i] ^= value;
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = bg;

            GUIElements.Line();
        }
        if (GUILayout.Button("New entry")) SkipData.neededItems.Add(new());

        SkipDataHandler.SaveSkipData(SkipData, log: false);
    }

    public override void Close()
    {
        SkipData = null;
        base.Close();
    }
}
