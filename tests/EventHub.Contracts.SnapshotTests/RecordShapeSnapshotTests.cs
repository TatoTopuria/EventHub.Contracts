using System.Text;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Invariant §2.2: contract records are additive-only. Reordering, renaming, retyping, or removing
/// a positional member changes the wire layout. This snapshots the primary-constructor signature of
/// every contract so any such change shows up as a reviewable diff.
/// </summary>
public class RecordShapeSnapshotTests
{
    [Fact]
    public void Contract_record_shapes_are_additive_only()
    {
        var sb = new StringBuilder();
        foreach (var t in ContractTypes.All)
        {
            var parameters = ContractTypes.PrimaryConstructor(t).GetParameters();
            var rendered = string.Join(", ", parameters.Select(p => $"{ContractTypes.Pretty(p.ParameterType)} {p.Name}"));
            sb.AppendLine($"{t.Name}({rendered})");
        }

        Snapshot.Match(sb.ToString(), "record-shapes");
    }
}
