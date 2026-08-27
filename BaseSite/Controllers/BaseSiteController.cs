using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Controllers;

public abstract class BaseSiteController : Controller
{
    protected LegacySession Session => new(HttpContext);
}

public sealed class LegacySession
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> Values = new();
    private readonly HttpContext context;

    public LegacySession(HttpContext context) => this.context = context;

    private ConcurrentDictionary<string, object> Store => Values.GetOrAdd(context.Session.Id, _ => new());

    public object this[string key]
    {
        get => Store.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value is null) Store.TryRemove(key, out _);
            else Store[key] = value;
        }
    }

    public void Clear() => Store.Clear();
}
