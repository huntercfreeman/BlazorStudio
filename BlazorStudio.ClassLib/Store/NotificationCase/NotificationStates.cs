using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.NotificationCase;

[FeatureState]
public record NotificationStates(ImmutableList<NotificationRecord> List)
{
    private NotificationStates() : this(ImmutableList<NotificationRecord>.Empty)
    {
    }
}