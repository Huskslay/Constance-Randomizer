using RandomizerCore.Classes.Storage.Items;

namespace RandomizerCore.Classes.Storage.Locations.Types.Progressive;

public interface IProgressiveLocation
{
    string GetFullName();

    ProgressiveItemType GetProgressiveType();
    int GetProgressiveIndex();
    AItem GetProgressiveItem();
}
