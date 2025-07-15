#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

public class RateUsIOS : IRateUsPlatform
{
    [DllImport("__Internal")]
    private extern void _RequestReview();

    public void RequestReview()
    {
        _RequestReview();
    }
}

#endif
