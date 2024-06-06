namespace MZ.Lock.Tests
{
    public class LockerTests
    {
        private ManualResetEvent _checkPointLock1 = new(false);
        private ManualResetEvent _checkPointLock2 = new(false);
        private ManualResetEvent _checkPointLock3 = new(false);
        private ManualResetEvent _checkPointLock4 = new(false);

        private bool lockedByThread1 = false;
        private bool lockedByThread2 = false;

        [Fact]
        public void Test()
        {
            const string Key = "42";

            Locker locker = new Locker();

            Lock lock1 = (Lock)locker.GetLock(Key);
            Lock lock2 = (Lock)locker.GetLock(Key);

            Assert.Same(lock1.LockData, lock2.LockData);

            Assert.Equal(1, locker._lockData.Count);
            Assert.True(locker._lockData.ContainsKey(Key));
            Assert.Equal(Key, lock1.LockData.Key);
            Assert.Equal(2, lock1.LockData.RefCount);

            new Thread(() => TestLock1(lock1)).Start();
            new Thread(() => TestLock2(lock2)).Start();

            // Thread1 must aquire the lock
            Thread.Sleep(1000);
            Assert.True(lockedByThread1);
            Assert.False(lockedByThread2);
            _checkPointLock1.Set();

            // Thread1 still holds the lock
            // Thread2 tries to aquire the lock, but blocks
            Thread.Sleep(1000);
            Assert.True(lockedByThread1);
            Assert.False(lockedByThread2);
            _checkPointLock2.Set();

            // Thread1 releases the lock
            // Thread2 aquires the lock
            Thread.Sleep(1000);
            Assert.False(lockedByThread1);
            Assert.True(lockedByThread2);
            Assert.Equal(1, locker._lockData.Count);
            Assert.Equal(1, lock1.LockData.RefCount);
            _checkPointLock3.Set();

            // Thread 2 releases the lock
            Thread.Sleep(1000);
            Assert.False(lockedByThread1);
            Assert.False(lockedByThread2);
            Assert.Equal(0, locker._lockData.Count);
            Assert.Equal(0, lock1.LockData.RefCount);
        }

        private void TestLock1(Lock @lock)
        {
            @lock.Aquire();
            lockedByThread1 = true;

            _checkPointLock1.WaitOne();

            _checkPointLock2.WaitOne();

            @lock.Release();
            lockedByThread1 = false;
            _checkPointLock3.WaitOne();
        }

        private void TestLock2(Lock @lock)
        {
            _checkPointLock1.WaitOne();

            @lock.Aquire();
            lockedByThread2 = true;

            _checkPointLock2.WaitOne();
            _checkPointLock3.WaitOne();

            @lock.Release();
            lockedByThread2 = false;
        }
    }
}