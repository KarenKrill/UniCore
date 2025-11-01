#nullable enable

using System;

namespace KarenKrill.UniCore.Diagnostics.Abstractions
{
    public interface IDiagnosticsProvider
    {
        PerfomanceInfo PerfomanceInfo { get; }

        event Action<PerfomanceInfo>? PerfomanceInfoChanged;
    }
}
