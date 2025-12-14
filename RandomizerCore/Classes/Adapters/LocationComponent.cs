using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomizerCore.Classes.Adapters;

public class LocationComponent : MonoBehaviour
{
    public ALocation Location { get; private set; }

    public void Set(ALocation location)
    {
        Location = location;
    }
}
