namespace MZ.Lock;

internal class LockData
{
    public LockData(string key)
    {
        Lock = new object();
        Key = key;
        RefCount = 0;
    }

    public object Lock;

    public string Key;

    public int RefCount;
}
