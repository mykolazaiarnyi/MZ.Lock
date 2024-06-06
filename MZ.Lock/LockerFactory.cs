using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("MZ.Lock.Tests")]

namespace MZ.Lock;

public static class LockerFactory
{
    public static ILocker Locker => new Locker();
}
