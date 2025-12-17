using RandomizerCore.Classes.Storage.Locations;
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
