using Brimborium.CodeFlow.FlowSynchronization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncDictionaryTests {
        [Fact]
        public async Task SyncDictionary_ExclusiveAsync() {
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => id.ToString() ?? string.Empty));
            object key = "1";
            var t1 = sut.LockAsync<object>(key, true, null, default);
            var t2 = sut.LockAsync<object>(key, true, null, default);
            var t3 = sut.LockAsync<object>(key, true, null, default);
            await Task.Yield();
            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t2.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            (await t1).Dispose();
            (await t2).Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            (await t3).Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);

            //var tcsStartAll = new TaskCompletionSource();
            //var tcsStart0 = new TaskCompletionSource();
            //var tcsStart1 = new TaskCompletionSource();
            //var tcs1 = new TaskCompletionSource();
            //var tcs2 = new TaskCompletionSource();
            //var act = new List<int>();
            //var t1 = Task.Run(async () => {
            //    tcsStart0.SetResult();
            //    await tcsStartAll.Task;

            //    await tcs2.Task;
            //    var d1 = await sut.LockAsync<string>("1", true, null, default);
            //    lock (act) {
            //        act.Add(1);
            //    }
            //    await Task.Delay(1);
            //    await tcs1.Task;
            //    d1.Dispose();
            //    lock (act) {
            //        act.Add(2);
            //    }
            //});
            //var t2 = Task.Run(async () => {
            //    tcsStart1.SetResult();
            //    await tcsStartAll.Task;
            //    await tcs1.Task;
            //    await tcs2.Task;
            //    var d2 = await sut.LockAsync<string>("1", true, null, default);
            //    lock (act) {
            //        act.Add(3);
            //    }
            //    await Task.Delay(1);
            //    d2.Dispose();
            //    lock (act) {
            //        act.Add(4);
            //    }
            //});
            //await tcsStart0.Task;
            //await tcsStart1.Task;
            //tcsStartAll.SetResult();

            //tcs1.SetResult();
            //tcs2.SetResult();

            //await Task.WhenAll(t1, t2);
            //var actJoined = string.Join("-", act.Select(i => i.ToString()));
            //Assert.Equal("1-2-3-4", actJoined);
        }

        [Fact]
        public async Task SyncDictionary_SharedAsync() {
            var sut = new SyncDictionary(
                new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    TimeSpan.FromSeconds(1)),
                null);
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => id.ToString() ?? string.Empty));
            object key = "1";
            var t1 = sut.LockAsync<object>(key, false, null, default);
            var t2 = sut.LockAsync<object>(key, false, null, default);
            var t3 = sut.LockAsync<object>(key, false, null, default);
            await Task.Yield();
            Assert.Equal(TaskStatus.RanToCompletion, t1.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);
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
            sut.RegisterType<object>(new SyncItemFactoryFunction<object>((object id) => $"{id}-{idx}"));
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
            (await t1).Dispose();
            (await t2).Dispose();

            Assert.Equal(TaskStatus.WaitingForActivation, t3.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t4.Status);
            (await t3).Dispose();

            Assert.Equal(TaskStatus.RanToCompletion, t3.Status);
            Assert.Equal(TaskStatus.WaitingForActivation, t4.Status);

            (await t4).Dispose();
            Assert.Equal(TaskStatus.RanToCompletion, t4.Status);
        }
        }
}
