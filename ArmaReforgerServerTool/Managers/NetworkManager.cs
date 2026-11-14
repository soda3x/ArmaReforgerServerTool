/******************************************************************************
 * File Name:    NetworkManager.cs
 * Project:      Longbow
 * Description:  This file contains the singleton NetworkManager class
 *               responsible for networking specific operations.
 * 
 * Authors:      Kye Seyhun
 ******************************************************************************/

using Longbow.Utils;
using Open.Nat;
using ReforgerServerApp.Utils;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace ReforgerServerApp.Managers
{
  /// <summary>
  /// This class manages the application's Networking related logic
  /// </summary>
  internal class NetworkManager
  {
    private static NetworkManager? m_instance;
    private static readonly int INFINITE_LIFETIME = 0;

    public bool useUPnP { get; set; }

    private NetworkManager()
    {
      // Disable no-args constructor to enforce singleton pattern
    }

    public static NetworkManager GetInstance()
    {
      m_instance ??= new NetworkManager();
      return m_instance;
    }

    /// <summary>
    /// Configure Universal Plug and Play for a list of devices and ports
    /// </summary>
    /// <param name="mappings">to configure UPnP for</param>
    public async Task ConfigurePortMappings(List<(string ipAddress, int port)> mappings)
    {
      if (!useUPnP)
      {
        Log.Information("NetworkManager - UPnP is not being used.");
        return;
      }

      try
      {
        var discoverer = new NatDiscoverer();
        var device = await discoverer.DiscoverDeviceAsync();

        foreach (var mapping in mappings)
        {
          string ipAddr = mapping.ipAddress;
          int port = mapping.port;

          // Convert string IP address to IPAddress type
          if (IPAddress.TryParse(ipAddr, out IPAddress ip))
          {
            // Create port mapping for the specified IP address
            var natMapping = new Mapping(Protocol.Tcp, ip, port, port, INFINITE_LIFETIME, $"Mapping for {ipAddr}:{port}");
            try
            {
              await device.CreatePortMapAsync(natMapping);
              Log.Information("NetworkManager - Opened UPnP port mapping {ipAddr}:{port}", ipAddr, port);
            }
            catch (Exception ex)
            {
              Log.Error("NetworkManager - Failed to map {ipAddr}:{port} - {ex}", ipAddr, port, ex.Message);
            }
          }
          else
          {
            Log.Error("NetworkManager - Failed to convert {ipAddr} to IP Address. UPnP will not be configured for port {port}", ipAddr, port);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("NetworkManager - Failed to configure UPnP: {msg}", ex.Message);
        if (ex.GetType() == typeof(NatDeviceNotFoundException))
        {
          Utilities.DisplayErrorMessage(Strings.Get(Constants.NETWORK_MANAGER_UPNP_FAILED_STR), Strings.Get(Constants.NETWORK_MANAGER_UPNP_NAT_NOT_FOUND_STR));
        }

        if (ex.GetType() == typeof(SocketException))
        {
          Utilities.DisplayErrorMessage(Strings.Get(Constants.NETWORK_MANAGER_UPNP_FAILED_STR), Strings.Get(Constants.NETWORK_MANAGER_UPNP_GENERIC_FAILED_STR));
        }
      }
    }

    /// <summary>
    /// Remove any created UPnP mappings, given a list
    /// </summary>
    /// <param name="mappings">to remove UPnP for</param>
    public async Task RemovePortMappings(List<(string ipAddress, int port)> mappings)
    {
      if (!useUPnP)
      {
        Log.Information("NetworkManager - UPnP is not being used.");
        return;
      }

      try
      {
        var discoverer = new NatDiscoverer();
        var device = await discoverer.DiscoverDeviceAsync();

        Console.WriteLine("Device found: " + device);

        foreach (var mapping in mappings)
        {
          string ipAddr = mapping.ipAddress;
          int port = mapping.port;

          // Convert string IP address to IPAddress type
          if (IPAddress.TryParse(ipAddr, out IPAddress ip))
          {
            // Create port mapping for the specified IP address
            var natMapping = new Mapping(Protocol.Tcp, ip, port, port, INFINITE_LIFETIME, $"Mapping for {ipAddr}:{port}");
            await device.DeletePortMapAsync(natMapping);

            Log.Information("NetworkManager - Removed UPnP port mapping {ipAddr}:{port}", ipAddr, port);
          }
          else
          {
            Log.Error("NetworkManager - Failed to convert {ipAddr} to IP Address. UPnP will not be configured for port {port}", ipAddr, port);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("NetworkManager - An exception occurred while attempting to remove UPnP port mappings: {msg}", ex.Message);
      }
    }
  }
}
