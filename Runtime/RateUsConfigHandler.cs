using System;
using UnityEngine;

public class RateUsConfigHandler
{
    protected string configJson = null;
    RateUsConfig config = null;

    public virtual void GetConfig(Action<bool, string, RateUsConfig> onComplete)
    {
        configJson ??= Resources.Load<TextAsset>("rate_us_config").text;

        bool succeed = true;
        string message = "Rate Us Configurations failed";

        try
        {
            if (string.IsNullOrEmpty(configJson))
                throw new Exception("Json string is empty");

            config = JsonUtility.FromJson<RateUsConfig>(configJson);
            message = "Rate Us Configurations succeed";
            succeed = true;
        }
        catch (Exception err)
        {
            message = err.Message;
            succeed = false;
        }

        onComplete(succeed, message, config);
    }

    public virtual RateUsConfig GetConfig()
    {
        return config;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(GetConfig());
    }
}
