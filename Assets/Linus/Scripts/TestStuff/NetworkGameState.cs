using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkGameState : NetworkBehaviour
{
    public static NetworkGameState Instance;
    [SerializeField] private bool useCountdown = false;

    [Header("NetworkVariables")]
    public NetworkVariable<bool> IsPaused = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> CountdownValue = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool isCountingDown = false;

    private void Awake()
    {
        Instance = this;
    }

    //-----------------PAUSE---------------
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestPauseServerRpc()
    {
        if (!IsPaused.Value && !isCountingDown)
        {
            StartCoroutine(PauseCountdownRoutine());
        }
    }

    private IEnumerator PauseCountdownRoutine()
    {
        if (useCountdown)
        {
            isCountingDown = true;
            CountdownValue.Value = 3;

            while (CountdownValue.Value > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                CountdownValue.Value -= 1;
            }

            IsPaused.Value = true;
            isCountingDown = false;
        }
        else
        {
            IsPaused.Value = true;
            isCountingDown = false;
        }
    }




    //-----------------Resume---------------
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestResumeServerRpc()
    {
        if (IsPaused.Value && !isCountingDown)
        {
            StartCoroutine(ResumeCountDownRoutine());
        }
    }
    private IEnumerator ResumeCountDownRoutine()
    {
        if (useCountdown)
        {
            isCountingDown = true;
            CountdownValue.Value = 3;

            while (CountdownValue.Value > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                CountdownValue.Value -= 1;
            }
            IsPaused.Value = false;
            isCountingDown = false;
        }
        else
        {
            IsPaused.Value = false;
            isCountingDown = false;
        }
        
    }
}
