using RandomizerCore.Classes.Storage.Transitions;
using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.UI.Selectable;

namespace Randomizer.Classes.Random.Generation;

public class RandomSearchHolder<T, T2>
{
    private readonly Queue<T> open;
    private readonly Queue<T> closed;
    private readonly List<T2> found;

    public RandomSearchHolder(T start1, T start2)
    {
        open = new();
        open.Enqueue(start1);
        open.Enqueue(start2);
        closed = new();
        found = [];
    }


    public bool NotEmpty() { return open.Count > 0; }
    public bool SkipSearch(T subject) { return closed.Contains(subject) || open.Contains(subject); }


    public T PopOpen() { return open.Dequeue(); }


    public void AddToOpen(List<T> transitions)
    { 
        foreach (T transition in transitions) {
            if (!closed.Contains(transition) && !open.Contains(transition)) open.Enqueue(transition);
        }
    }
    public void AddToClosed(T subject) { closed.Enqueue(subject); }


    public void AddToFound(T2 subject) { found.Add(subject); }
    public bool FoundContains(T2 subject) { return found.Contains(subject); }
    public List<T2> GetFound() { return found; }


    public void Reset()
    { closed.Clear(); }
}
