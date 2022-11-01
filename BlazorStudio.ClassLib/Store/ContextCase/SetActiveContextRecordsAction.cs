using System.Collections.Immutable;
using BlazorStudio.ClassLib.Context;

namespace BlazorStudio.ClassLib.Store.ContextCase;

public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);