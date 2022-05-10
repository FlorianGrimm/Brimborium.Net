namespace Brimborium.RowVersion.Extensions;
public static class ListExtension {
    public static int BinarySearchByValue<T, K>(this List<T> list, K value, Func<T, K, int> compare) {
        var low = 0;
        var high = list.Count - 1;

        while (low <= high) {
            var index = low + (high - low >> 1);

            var c = compare(list[index], value);
            if (c == 0) {
                return index;
            } else if (c < 0) {
                low = index + 1;
            } else {
                high = index - 1;
            }
        }

        return ~low;
    }

    public static bool TryGetByValue<T, K>(this List<T> list, K value, Func<T, K, int> compare, [MaybeNullWhen(false)] out T result, out int position) {
        var low = 0;
        var high = list.Count - 1;

        while (low <= high) {
            var index = low + (high - low >> 1);

            var c = compare(list[index], value);
            if (c == 0) {
                result = list[index];
                position = index;
                return true;
            } else if (c < 0) {
                low = index + 1;
            } else {
                high = index - 1;
            }
        }
        result = default;
        position = ~low;
        return false;
    }
}
