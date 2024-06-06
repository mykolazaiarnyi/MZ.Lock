# MZ.Lock

Lock by key using Monitor with ref counting to avoid memory leaks.
```C#
// Create a single `ILocker` instance using factory.
ILocker locker = LockerFactory.Factory;

// Get `ILock` by providing a string locking key. `ILock` implements IDisposable to ensure the lock is properly released and memory used by the lock is cleared.
using ILock @lock = locker.GetLock("42");

// Call `Aquire` method to lock.
@lock.Aquire();

// Release the lock manually using `Release` method or let `Dispose` do it.
@lock.Release();
```

The `ILocker` implementation is thread safe, so a single instance can be reused accross 
entire application. The internal data for each locking key is removed when not used, so 
the same single instance of `ILocker` can be reused for the lifetime of the application.