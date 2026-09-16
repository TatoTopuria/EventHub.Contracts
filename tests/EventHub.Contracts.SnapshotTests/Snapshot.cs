using System.Runtime.CompilerServices;
using FluentAssertions;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Tiny approval-test helper. Compares an actual string against a committed golden file under
/// <c>Snapshots/</c> (resolved next to the calling source file via <see cref="CallerFilePathAttribute"/>).
///
/// To (re)generate goldens after an INTENTIONAL, additive contract change:
///   PowerShell:  $env:UPDATE_SNAPSHOTS='1'; dotnet test; Remove-Item Env:UPDATE_SNAPSHOTS
/// then review the diff and bump the EventHub.Contracts package version.
/// </summary>
internal static class Snapshot
{
    public static void Match(string actual, string name, [CallerFilePath] string callerPath = "")
    {
        var dir = Path.Combine(Path.GetDirectoryName(callerPath)!, "Snapshots");
        Directory.CreateDirectory(dir);
        var golden = Path.Combine(dir, name + ".verified.txt");

        // Normalize line endings + trailing whitespace so Windows/Linux runners agree.
        var normalized = actual.Replace("\r\n", "\n").TrimEnd() + "\n";
        var update = Environment.GetEnvironmentVariable("UPDATE_SNAPSHOTS") == "1";

        if (update)
        {
            File.WriteAllText(golden, normalized);
            return;
        }

        if (!File.Exists(golden))
        {
            File.WriteAllText(golden, normalized);
            Assert.Fail(
                $"Snapshot '{name}' had no baseline — created {golden}. " +
                "Review and commit it, then re-run. (Build is red on purpose so a missing baseline can't pass in CI.)");
        }

        var expected = File.ReadAllText(golden).Replace("\r\n", "\n");
        normalized.Should().Be(expected,
            $"the wire contract snapshot '{name}' changed. This is a CROSS-SERVICE WIRE-BREAKING change " +
            "unless it is intentional AND additive. If intentional, re-run with UPDATE_SNAPSHOTS=1 and bump EventHub.Contracts.");
    }
}
