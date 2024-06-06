namespace MZ.Lock;

public interface ILocker
{
    ILock GetLock(string key);
}

internal class Locker : ILocker
{
    private static readonly object _lockDataLock = new object();

    internal Dictionary<string, LockData> _lockData = new();

    public ILock GetLock(string key)
    {
        LockData? lockData;

        lock (_lockDataLock)
        {
            if (!_lockData.TryGetValue(key, out lockData))
            {
                lockData = new LockData(key);
                _lockData.Add(key, lockData);
            }

            lockData.RefCount++;
        }

        return new Lock(lockData, this);
    }

    public void ReturnLock(Lock @lock)
    {
        lock (_lockDataLock)
        {
            @lock.LockData.RefCount--;
            if (@lock.LockData.RefCount == 0)
                _lockData.Remove(@lock.LockData.Key);
        }
    }
}