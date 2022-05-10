namespace Brimborium.RowVersion.Extensions;

public class ListExtensionTests {
    [Fact()]
    public void BinarySearchByValue_Test() {
        {
            var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var idx = sut.BinarySearchByValue(
                15,
                (item, search) => item.CompareTo((search - 10)));
            Assert.Equal(4, idx);
        }
        {
            var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var idx = sut.BinarySearchByValue(
                19,
                (item, search) => item.CompareTo((search - 10)));
            Assert.Equal(~8, idx);
        }
    }

    [Fact()]
    public void TryGetByValue_Test() {
        {
            var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.True(sut.TryGetByValue(
                15,
                (item, search) => item.CompareTo((search - 10)),
                out var result,
                out var position));
            Assert.Equal(5, result);
            Assert.Equal(4, position);
        }
    }
}
