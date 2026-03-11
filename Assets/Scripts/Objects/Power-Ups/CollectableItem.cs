using System.Collections;
using UnityEngine;

public sealed class CollectableContext
{
    public CollectableContext(CollectableUI source, Player player)
    {
        Source = source;
        Player = player;
        Shooting = player != null ? player.Shooting : null;
    }

    public CollectableUI Source { get; }
    public Player Player { get; }
    public Shooting Shooting { get; }
}

public abstract class CollectableItem : ScriptableObject
{
    [Header("Presentation")]
    [SerializeField] private string _displayName;
    [SerializeField] [TextArea] private string _description;

    public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
    public string Description => _description;

    public bool TryCollect(CollectableContext context)
    {
        if (!CanCollect(context))
        {
            return false;
        }

        OnCollect(context);
        return true;
    }

    protected virtual bool CanCollect(CollectableContext context)
    {
        if (context == null)
        {
            Debug.LogWarning($"{nameof(CollectableItem)} recibió un contexto nulo.", this);
            return false;
        }

        if (context.Player == null)
        {
            Debug.LogWarning($"{DisplayName} no puede aplicar el ítem porque no se encontró al jugador.", this);
            return false;
        }

        return true;
    }

    protected Coroutine StartRoutine(CollectableContext context, IEnumerator routine)
    {
        if (context?.Player == null || routine == null)
        {
            return null;
        }

        return context.Player.StartCoroutine(routine);
    }

    protected abstract void OnCollect(CollectableContext context);
}