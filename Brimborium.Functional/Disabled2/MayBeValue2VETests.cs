#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional {
    public class MayBeValue2VETests {
        [Fact()]
        public void DeconstructTest() {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var (a, b) = sut;
            Assert.Equal(true, a);
            Assert.Equal(1, b);
        }

        [Fact()]
        public void TryGetValueTest() {
            {
                var sut = new MayBeValue<int, Exception>(true, 1);
                Assert.Equal(true, sut.TryGetValue(out var result));
                Assert.Equal(1, result);
            }
            {
                var sut = new MayBeValue<int, Exception>(false, 1);
                Assert.Equal(true, sut.TryGetValue(out var result));
                Assert.Equal(1, result);
            }
        }

        [Fact()]
        public void TryGetSuccessfullValueTest() {
            {
                var sut = new MayBeValue<int, Exception>(true, 1);
                Assert.Equal(true, sut.TryGetSuccessfullValue(out var result));
                Assert.Equal(1, result);
            }
            {
                var sut = new MayBeValue<int, Exception>(false, 1);
                Assert.Equal(false, sut.TryGetSuccessfullValue(out var _));
            }
        }

        [Fact()]
        public void TryGetFailureValueTest() {
            {
                var sut = new MayBeValue<int, Exception>(false, 1);
                Assert.Equal(false, sut.TryGetFailureValue(out var result));
            }
        }

        [Fact()]
        public void WithMayValueTest() {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithMayValue(true, 2);
            Assert.Equal(true, act.Success);
            Assert.Equal(2, act.Value);
        }

        [Fact()]
        public void WithSuccessfullValueTest() {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithSuccessfullValue(2);
            Assert.Equal(true, act.Success);
            Assert.Equal(2, act.Value);
        }

        [Fact()]
        public void WithUndecidedValueTest() {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithUndecidedValue(2);
            Assert.Equal(false, act.Success);
            Assert.Equal(2, act.Value);
        }

        [Fact()]
        public void MapTest() {
            {
                var sut = new MayBeValue<int, Exception>(true, 1);
                var act = sut.Map<string, Exception>(
                    onSuccessfull: (a, r) => r.WithSuccessfullValue("OK"),
                    onUndecided: (_, _) => throw new Exception("onUndecided"),
                    onFail: (_, _) => throw new Exception("onFail"),
                    onNoValue: (_) => throw new Exception("onNoValue")
                    );
                Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
                Assert.Equal("OK", r);
            }
            {
                var sut = new MayBeValue<int, Exception>(false, 1);
                var act = sut.Map<string, Exception>(
                    onSuccessfull: (a, r) => throw new Exception("onSuccessfull"),
                    onUndecided: (_, r) => r.WithSuccessfullValue("OK"),
                    onFail: (_, _) => throw new Exception("onFail"),
                    onNoValue: (_) => throw new Exception("onNoValue")
                    );
                Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
                Assert.Equal("OK", r);
            }
            {
                var sut = new MayBeFail<int, Exception>(new Exception("OK"));
                var act = sut.Map<string, Exception>(
                    onSuccessfull: (_, _) => throw new Exception("onSuccessfull"),
                    onUndecided: (_, _) => throw new Exception("onUndecided"),
                    onFail: (_, r) => r.WithSuccessfullValue("OK"),
                    onNoValue: (_) => throw new Exception("onNoValue")
                    );
                Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
                Assert.Equal("OK", r);
            }
            {
                var sut = MayBeNoValue<int, Exception>.Empty();
                var act = sut.Map<string, Exception>(
                    onSuccessfull: (a, r) => r.WithSuccessfullValue("OK"),
                    onUndecided: (_, _) => throw new Exception("onUndecided"),
                    onFail: (_, _) => throw new Exception("onFail"),
                    onNoValue: (r) => r.WithSuccessfullValue("OK")
                    );
                Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
                Assert.Equal("OK", r);
            }
            {
                var sut = new MayBeValue<int, Exception>(true, 1);
                var act = sut.Map<string, Exception>();
                Assert.Equal(false, act.Success);
                Assert.Equal(false, act.Fail);
            }
        }

        [Fact()]
        public void MapToTest() {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.MapTo<string>(
                onSuccessfull: (a) => (a.Value.ToString()),
                onUndecided: (_) => throw new Exception("onUndecided"),
                onFail: (_) => throw new Exception("onFail"),
                onNoValue: () => throw new Exception("onNoValue")
                );
            Assert.Equal("1", act);
        }
    }
}