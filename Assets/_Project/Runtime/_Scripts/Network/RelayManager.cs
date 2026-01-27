using System;
using System.Threading.Tasks;
using Lumina.Essentials.Modules.Singleton;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    /// <summary>
    /// Creates a Relay allocation and returns the join code.
    /// </summary>
    /// <param name="maxPlayers"></param>
    /// <returns></returns>
    public async Task<string> CreateAllocation(int maxPlayers)
    {
        try
        {
            await InitUnityServices();

            var allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            var joinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
            
            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to create allocation: " + e.Message);
            
            // Look at inner exception for more details
            Debug.LogWarning(e.ToString());
            
            return null;
        }
    }
    
    /// <summary>
    /// Join an existing Relay allocation using the provided join code.
    /// </summary>
    /// <param name="joinCode"></param>
    /// <param name="onJoined"></param>
    /// <param name="onFailed"></param>
    public async void JoinAllocation(string joinCode, Action onJoined, Action onFailed)
    {
        try
        {
            await InitUnityServices();
            
            var allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);
            
            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (allocation != null)
                onJoined?.Invoke();
            else
                onFailed?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to join allocation: " + e.Message);
            onFailed?.Invoke();
        }
        
    }

    async Task InitUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}