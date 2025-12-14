using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomizerCore.Classes.Adapters;

public class ManyLocationComponent : MonoBehaviour
{
    public List<ALocation> Locations { get; private set; }

    public void Set<T>(List<T> locations) where T : ALocation
    {
        Locations = locations.ConvertAll(x => (ALocation)x);
    }
}
