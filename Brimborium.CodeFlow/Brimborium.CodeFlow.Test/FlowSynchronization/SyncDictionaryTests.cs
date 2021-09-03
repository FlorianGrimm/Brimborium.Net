
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncDictionaryTests {
        [Fact]
        public async Task SyncDictionary_ExclusiveAsync() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key = "1";
            var t1 = sut.LockAsync<object>(key, true, null, default);
            var t2 = sut.LockAsync<object>(key, true, null, default);
            var t3 = sut.LockAsync<object>(key, true, null, default);
            await Task.Yield();
            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t2.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            var v1 = (await t1).GetItem().ToString();
            Assert.Equal("1-1", v1);

            (await t1).Dispose();
            (await t2).Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            (await t3).Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);
        }

        [Fact]
        public async Task SyncDictionary_SharedAsync() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key = "1";
            var t1 = sut.LockAsync<object>(key, false, null, default);
            var t2 = sut.LockAsync<object>(key, false, null, default);
            var t3 = sut.LockAsync<object>(key, false, null, default);
            await Task.Yield();
            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);
            var d1 = await t1;
            Assert.Equal("1-1", d1.GetItem().ToString());
            (await t1).Dispose();
            (await t2).Dispose();
            (await t3).Dispose();
        }

        [Fact]
        public async Task SyncDictionary_SharedExclusiveSharedAsync() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key = "1";
            var t1 = sut.LockAsync<object>(key, false, null, default);
            var t2 = sut.LockAsync<object>(key, false, null, default);
            var t3 = sut.LockAsync<object>(key, true, null, default);
            var t4 = sut.LockAsync<object>(key, false, null, default);
            await Task.Yield();

            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t4.Status);

            var d1 = await t1;
            Assert.Equal("1-1", d1.GetItem().ToString());
            var d2 = await t2;
            Assert.Equal("1-1", d2.GetItem().ToString());
            d1.Dispose();
            d2.Dispose();

            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t4.Status);
            var d3 = await t3;
            Assert.Equal("1-1", d3.GetItem().ToString());
            d3.Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t4.Status);

            var d4 = await t4;
            Assert.Equal("1-1", d4.GetItem().ToString());
            d4.Dispose();
            Assert.Equal(TaskStatus.RanToCompletion, t4.Status);
        }
        [Fact]
        public async Task SyncDictionary_SharedDifferentKey() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key1 = "one";
            object key2 = "two";
            var t1 = sut.LockAsync<object>(key1, false, null, default);
            var t2 = sut.LockAsync<object>(key2, false, null, default);
            var d1 = await t1;
            Assert.Equal("one-1", d1.GetItem().ToString());
            var d2 = await t2;
            Assert.Equal("two-2", d2.GetItem().ToString());
        }

        [Fact]
        public async Task SyncDictionary_Exclusive2Async() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key = "1";

            var tcsStartAll = new TaskCompletionSource();
            var tcsStart0 = new TaskCompletionSource();
            var tcsStart1 = new TaskCompletionSource();
            var act = new List<int>();
            var lock1 = sut.LockAsync<object>(key, true, null, default);
            var lock2 = sut.LockAsync<object>(key, true, null, default);
            var t1 = Task.Run(async () => {
                tcsStart0.SetResult();
                await tcsStartAll.Task;

                var d1 = await lock1;
                lock (act) {
                    act.Add(1);
                }
                await Task.Delay(10);

                d1.Dispose();
                lock (act) {
                    act.Add(2);
                }
            });
            var t2 = Task.Run(async () => {
                tcsStart1.SetResult();
                await tcsStartAll.Task;

                var d2 = await lock2;
                lock (act) {
                    act.Add(3);
                }
                await Task.Delay(10);
                d2.Dispose();
                lock (act) {
                    act.Add(4);
                }
            });
            await tcsStart0.Task;
            await tcsStart1.Task;
            tcsStartAll.SetResult();

            await Task.WhenAll(t1, t2);
            var actJoined = string.Join("-", act.Select(i => i.ToString()));
            Assert.Equal("1-2-3-4", actJoined);
        }


        [Fact]
        public async Task SyncDictionary_SharedExclusiveShared_2_Async() {
            int idx = 1;
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx++}"), null);
            object key = "1";
            var tlock1 = sut.LockAsync<object>(key, false, null, default);
            var tlock2 = sut.LockAsync<object>(key, false, null, default);
            var tlock3 = sut.LockAsync<object>(key, true, null, default);
            var tlock4 = sut.LockAsync<object>(key, false, null, default);

            var tcsStartAll = new TaskCompletionSource();
            var act = new List<int>();

            var t1 = await runTask(tcsStartAll, tlock1, 10);
            var t2 = await runTask(tcsStartAll, tlock2, 20);
            var t3 = await runTask(tcsStartAll, tlock3, 30);
            var t4 = await runTask(tcsStartAll, tlock4, 40);

            tcsStartAll.SetResult();

            await Task.WhenAll(t1, t2, t3, t4);
            // var actJoined = string.Join("-", act.Select(i => i.ToString()));

            foreach (var n in new int[] { 10, 11, 20, 21, 30, 31, 40, 41 }) {
                Assert.Contains(n, act);
            }

            Assert.True(act.IndexOf(10) < act.IndexOf(11));
            Assert.True(act.IndexOf(20) < act.IndexOf(21));
            Assert.True(act.IndexOf(20) < act.IndexOf(30));
            Assert.True(act.IndexOf(21) < act.IndexOf(30));
            Assert.True(act.IndexOf(30) < act.IndexOf(31));
            Assert.True(act.IndexOf(31) < act.IndexOf(40));
            Assert.True(act.IndexOf(40) < act.IndexOf(41));

            async Task<Task> runTask(TaskCompletionSource tscStartAll, Task<ISyncLock<object>> tlock, int report) {
                TaskCompletionSource tscStart = new TaskCompletionSource();
                var task = Task.Run(async () => {
                    tscStart.SetResult();
                    await tcsStartAll.Task;

                    var syncLock = await tlock;
                    lock (act) {
                        act.Add(report);
                    }
                    await Task.Delay(report % 7);
                    syncLock.Dispose();
                    lock (act) {
                        act.Add(report + 1);
                    }
                });
                await tscStart.Task;
                return task;
            }
        }
    }
}
