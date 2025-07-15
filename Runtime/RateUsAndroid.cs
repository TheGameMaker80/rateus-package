#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;

public class RateUsAndroid : IRateUsPlatform
{
    public void RequestReview()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using (var rateUs = new AndroidJavaObject("com.rateus.RateUs"))
            {
                rateUs.CallStatic("requestReview", activity);
            }
        }
    }
}
#endif