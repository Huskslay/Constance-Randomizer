using System.Collections.Generic;

namespace Randomizer.Classes.Random.Generation;

public class RandomSearchHolder<T, T2>
{
    private readonly Queue<T> open;
    private readonly Queue<T> closed;
    private readonly List<T2> found;

    private readonly List<T> future;
    private List<T> oldFuture;

    public RandomSearchHolder(T start1, T start2)
    {
        open = new();
        open.Enqueue(start1);
        open.Enqueue(start2);
        closed = new();
        found = [];

        future = [];
        oldFuture = [];
    }


    public bool NotEmpty() { return open.Count > 0; }
    public bool SkipSearch(T subject)
    {
        return closed.Contains(subject) || open.Contains(subject);
    }


    public T PopOpen() { return open.Dequeue(); }


    public void AddToOpen(List<T> subjects)
    {
        foreach (T subject in subjects)
        {
            if (!closed.Contains(subject) && !open.Contains(subject) && !future.Contains(subject))
                open.Enqueue(subject);
        }
    }
    public void AddToClosed(T subject)
    {
        if (!future.Contains(subject)) closed.Enqueue(subject);

        // If the open is empty, check that the old and new futures are different, and if so, move futures into open to check them again
        if (open.Count == 0)
        {
            bool different = future.Count != oldFuture.Count;
            List<T> newOldFuture = [];
            while (future.Count > 0)
            {
                if (!oldFuture.Contains(future[0])) different = true;
                open.Enqueue(future[0]);
                newOldFuture.Add(future[0]);
                future.RemoveAt(0);
            }
            oldFuture = newOldFuture;
            if (!different) open.Clear();
        }
    }


    public void AddToFuture(T subject) { if (!future.Contains(subject)) future.Add(subject); }

    public void AddToFound(T2 subject) { found.Add(subject); }
    public bool FoundContains(T2 subject) { return found.Contains(subject); }
    public List<T2> GetFound() { return found; }


    public void Reset()
    { closed.Clear(); }
}
