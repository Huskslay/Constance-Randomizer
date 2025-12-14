using Constance;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;

namespace FileHandler.Classes;

public static class GameDataActions
{
    public static UnityEvent<ConSaver, ConSaveStateId> OnFileSave { get; private set; } = new();
    public static UnityEvent<ConSaver, ConSaveStateId> OnFileDelete { get; private set; } = new();
}
