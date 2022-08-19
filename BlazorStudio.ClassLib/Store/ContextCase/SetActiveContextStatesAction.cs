using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;

namespace BlazorStudio.ClassLib.Store.ContextCase;

public record SetActiveContextStatesAction(ImmutableList<ContextRecord> ActiveContextStates);