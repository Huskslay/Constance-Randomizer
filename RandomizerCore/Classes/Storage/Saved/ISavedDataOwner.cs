namespace RandomizerCore.Classes.Storage.Saved;

public interface ISavedDataOwner<T> where T : SavedData
{
    string GetFullName();
    T GetSavedData();
    void SetSavedData(T savedData);
}

