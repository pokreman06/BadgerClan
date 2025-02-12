

using ProtoBuf.Grpc.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace gRPC.shared;

[DataContract]
public class StyleRequest
{
    [DataMember(Order = 1)]
    public string Style;

    [DataMember(Order = 2)]
    public int StyleId { get; set; }
}
[DataContract]
public class StyleResult
{
    [DataMember(Order = 1)]
    public string Style { get; set; }
}
[ServiceContract]
public interface IStyle
{
    Task<StyleResult> GetStyleAsync();
    Task SendStyleAsync(StyleRequest styleResult);
}