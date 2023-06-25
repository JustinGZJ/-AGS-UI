using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
					
public class Program
{
    /// <summary>
    /// Connects to server and reads the current ServerState. 
    /// </summary>
    public static async Task Main()
    {
        // describe this client application.
        var clientDescription = new ApplicationDescription
        {
            ApplicationName = "Workstation.UaClient.FeatureTests",
            ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
            ApplicationType = ApplicationType.Client
        };

        // create a 'ClientSessionChannel', a client-side channel that opens a 'session' with the server.
        var channel = new ClientSessionChannel(
            clientDescription,
            null, // no x509 certificates
            new AnonymousIdentity(), // no user identity
            "opc.tcp://127.0.0.1:49320", // the public endpoint of a server at opcua.rocks.
            SecurityPolicyUris.None); // no encryption
        try
        {
            // try opening a session and reading a few nodes.
            await channel.OpenAsync();

            Console.WriteLine($"Opened session with endpoint '{channel.RemoteEndpoint.EndpointUrl}'.");
            Console.WriteLine($"SecurityPolicy: '{channel.RemoteEndpoint.SecurityPolicyUri}'.");
            Console.WriteLine($"SecurityMode: '{channel.RemoteEndpoint.SecurityMode}'.");
            Console.WriteLine($"UserIdentityToken: '{channel.UserIdentity}'.");

            // build a ReadRequest. See 'OPC UA Spec Part 4' paragraph 5.10.2
            var readRequest = new ReadRequest {
                // set the NodesToRead to an array of ReadValueIds.
                NodesToRead = new[] {
                    // construct a ReadValueId from a NodeId and AttributeId.
                    new ReadValueId {
                        // you can parse the nodeId from a string.
                        // e.g. NodeId.Parse("ns=2;s=Demo.Static.Scalar.Double")
                        NodeId = NodeId.Parse(VariableIds.Server_ServerStatus),
                        // variable class nodes have a Value attribute.
                        AttributeId = AttributeIds.Value
                    }
                }
            };
            // send the ReadRequest to the server.
            var readResult = await channel.ReadAsync(readRequest);

            // DataValue is a class containing value, timestamps and status code.
            // the 'Results' array returns DataValues, one for every ReadValueId.
            var serverStatus = readResult.Results[0].GetValueOrDefault<ServerStatusDataType>();

            Console.WriteLine("\nServer status:");
            Console.WriteLine("  ProductName: {0}", serverStatus.BuildInfo.ProductName);
            Console.WriteLine("  SoftwareVersion: {0}", serverStatus.BuildInfo.SoftwareVersion);
            Console.WriteLine("  ManufacturerName: {0}", serverStatus.BuildInfo.ManufacturerName);
            Console.WriteLine("  State: {0}", serverStatus.State);
            Console.WriteLine("  CurrentTime: {0}", serverStatus.CurrentTime);

            Console.WriteLine($"\nClosing session '{channel.SessionId}'.");
            await channel.CloseAsync();
        }
        catch(Exception ex)
        {
		 	 await channel.AbortAsync();
            Console.WriteLine(ex.Message);
        }
    }
}

// Server status:
//   ProductName: open62541 OPC UA Server
//   SoftwareVersion: 1.0.0-rc5-52-g04067153-dirty
//   ManufacturerName: open62541
//   State: Running
