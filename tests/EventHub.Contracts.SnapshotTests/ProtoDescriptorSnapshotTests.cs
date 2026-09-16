using System.Text;
using BuildingBlocks.Grpc.Catalog;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Invariant §2.3: the catalog gRPC contract identity is the proto <c>package</c>, the service +
/// method names (which form the HTTP/2 path <c>/catalog.CatalogGrpc/GetEventDetails</c>), and the
/// message field numbers. This reads the compiled FileDescriptor and freezes all of it.
/// </summary>
public class ProtoDescriptorSnapshotTests
{
    [Fact]
    public void Catalog_grpc_descriptor_is_frozen()
    {
        var fd = CatalogReflection.Descriptor;
        var sb = new StringBuilder();
        sb.AppendLine($"package: {fd.Package}");

        foreach (var svc in fd.Services)
        {
            sb.AppendLine($"service {svc.FullName}");
            foreach (var m in svc.Methods)
                sb.AppendLine($"  rpc {m.Name}({m.InputType.FullName}) returns ({m.OutputType.FullName})  // /{svc.FullName}/{m.Name}");
        }

        foreach (var mt in fd.MessageTypes.OrderBy(m => m.Name, StringComparer.Ordinal))
        {
            sb.AppendLine($"message {mt.FullName}");
            foreach (var f in mt.Fields.InFieldNumberOrder())
                sb.AppendLine($"  {f.FieldNumber}: {f.Name} ({f.FieldType}{(f.IsRepeated ? ", repeated" : "")})");
        }

        Snapshot.Match(sb.ToString(), "catalog-proto");
    }
}
