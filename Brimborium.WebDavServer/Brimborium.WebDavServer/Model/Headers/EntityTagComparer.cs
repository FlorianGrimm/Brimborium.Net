using System.Collections.Generic;


namespace Brimborium.WebDavServer.Model.Headers
{
    /// <summary>
    /// A comparer for entity tags
    /// </summary>
    public class EntityTagComparer : IEqualityComparer<EntityTag>
    {
        private readonly bool _useStrongComparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTagComparer"/> class.
        /// </summary>
        /// <param name="useStrongComparison">Has strong comparison to be used?</param>
        private EntityTagComparer(bool useStrongComparison)
        {
            this._useStrongComparison = useStrongComparison;
        }

        /// <summary>
        /// Gets a default strong entity tag comparer
        /// </summary>
        public static EntityTagComparer Strong { get; } = new EntityTagComparer(true);

        /// <summary>
        /// Gets a default weak entity tag comparer
        /// </summary>
        public static EntityTagComparer Weak { get; } = new EntityTagComparer(false);

        /// <inheritdoc />
        public bool Equals(EntityTag x, EntityTag y)
        {
            if (this._useStrongComparison) {
                return x.Value == y.Value && x.IsWeak == y.IsWeak && !x.IsWeak;
            }

            return x.Value == y.Value;
        }

        /// <inheritdoc />
        public int GetHashCode(EntityTag obj)
        {
            unchecked
            {
                var result = obj.Value.GetHashCode();
                if (this._useStrongComparison) {
                    result ^= 137 * obj.IsWeak.GetHashCode();
                }

                return result;
            }
        }
    }
}
