using Shift.Common;

namespace Shift.Test.Common
{
    public class KeyedLockTests
    {
        private const string Key1 = "key-1";
        private const string Key2 = "key-2";
        private const string Key3 = "key-3";

        [Fact]
        public async Task Lock_SameKey_OneThreadOnly()
        {
            var instance = new KeyedLock<string>();
            int current = 0;
            int failed = 0;

            var tasks = Enumerable.Range(0, 50).Select(x => Task.Run(() =>
            {
                using (instance.Lock(Key1))
                {
                    if (Interlocked.Increment(ref current) != 1)
                        Interlocked.Increment(ref failed);

                    Thread.SpinWait(5000);

                    Interlocked.Decrement(ref current);
                }
            }));

            await Task.WhenAll(tasks);

            Assert.Equal(0, failed);
        }

        [Fact]
        public void Lock_DiffKeys_NoLock()
        {
            var instance = new KeyedLock<string>();

            using (instance.Lock(Key1))
            {
                using (instance.Lock(Key2, TimeSpan.FromMilliseconds(100)))
                {

                }
            }
        }

        [Fact]
        public async Task Lock_SameKey_CheckTimeout()
        {
            var instance = new KeyedLock<string>();
            var syncLock = new ManualResetEventSlim();
            var taskLock = new ManualResetEventSlim();

            var task = Task.Run(() =>
            {
                using (instance.Lock(Key1))
                {
                    syncLock.Set();
                    taskLock.Wait();
                }
            });

            syncLock.Wait();

            Assert.Throws<TimeoutException>(() => instance.Lock(Key1, TimeSpan.FromMilliseconds(100)));

            taskLock.Set();

            await task;
        }

        [Fact]
        public async Task Lock_DiffKeys_AllTrackedKeysCleanedUp()
        {
            var instance = new KeyedLock<string>();
            var tasks = Enumerable.Range(0, 150).Select(x => Task.Run(() =>
            {
                using (instance.Lock(Key1))
                    Thread.SpinWait(5000);

                using (instance.Lock(Key2))
                    Thread.SpinWait(5000);

                using (instance.Lock(Key3))
                    Thread.SpinWait(5000);
            }));

            await Task.WhenAll(tasks);

            Assert.Equal(0, instance.TrackedKeyCount);
        }
    }
}
