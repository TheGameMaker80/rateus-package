using System;

[Serializable]
public class VersionRates
{
    public string version;
    public int rates;

    public VersionRates() { }

    public VersionRates(string ver, int rat)
    {
        version = ver;
        rates = rat;
    }
}
