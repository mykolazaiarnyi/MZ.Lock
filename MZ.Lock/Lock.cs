namespace MZ.Lock;

public interface ILock : IDisposable
{
    void Aquire();

    void Release();
}

internal class Lock : ILock
{
    internal readonly LockData LockData;
    private readonly Locker _locker;
    private LockState _state = LockState.Unused;
    private bool _disposed = false;

    public Lock(LockData lockData, Locker locker)
    {
        LockData = lockData;
        _locker = locker;
    }

    public void Aquire()
    {
        if (_disposed || _state != LockState.Unused)
            throw new InvalidOperationException();

        Monitor.Enter(LockData.Lock);
        _state = LockState.Aquired;
    }

    public void Release()
    {
        if (_disposed || _state != LockState.Aquired)
            throw new InvalidOperationException();

        _locker.ReturnLock(this);
        Monitor.Exit(LockData.Lock);
        _state = LockState.Released;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        switch (_state)
        {
            case LockState.Unused: _locker.ReturnLock(this); break;
            case LockState.Aquired: Release(); break;
            case LockState.Released: break;
        }
        _disposed = true;
    }

    private enum LockState
    {
        Unused,
        Aquired,
        Released
    }
}