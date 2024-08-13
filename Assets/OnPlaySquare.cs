
using Microsoft.MixedReality.Toolkit.Input;


#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif
public class OnPlaySquare : BaseEyeFocusHandler
{
    private bool IsFinished = true;
    private string log1, log2;

    protected override void OnEyeFocusStart()

    {

      
        IsFinished = false;
    }

    protected override void OnEyeFocusStop()
    {

        IsFinished = true;
}
    }







