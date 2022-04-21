﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#if NET6_0_OR_GREATER
#else
using System.Collections.ObjectModel;

namespace System.Reflection {
    /// <summary>
    /// A class that represents nullability info
    /// </summary>
    public sealed class NullabilityInfo {
        internal NullabilityInfo(Type type, NullabilityState readState, NullabilityState writeState,
            NullabilityInfo? elementType, NullabilityInfo[] typeArguments) {
            Type = type;
            ReadState = readState;
            WriteState = writeState;
            ElementType = elementType;
            TypeArguments = typeArguments;
        }

        /// <summary>
        /// The <see cref="System.Type" /> of the member or generic parameter
        /// to which this NullabilityInfo belongs
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// The nullability read state of the member
        /// </summary>
        public NullabilityState ReadState { get; internal set; }
        /// <summary>
        /// The nullability write state of the member
        /// </summary>
        public NullabilityState WriteState { get; internal set; }
        /// <summary>
        /// If the member type is an array, gives the <see cref="NullabilityInfo" /> of the elements of the array, null otherwise
        /// </summary>
        public NullabilityInfo? ElementType { get; }
        /// <summary>
        /// If the member type is a generic type, gives the array of <see cref="NullabilityInfo" /> for each type parameter
        /// </summary>
        public NullabilityInfo[] TypeArguments { get; }
    }

    /// <summary>
    /// An enum that represents nullability state
    /// Uknown - Nullable context not enabled (oblivious)
    /// NotNull - non nullable value or reference type
    /// Nullable - nullable value or reference type
    /// </summary>
    public enum NullabilityState {
        Unknown,
        NotNull,
        Nullable
    }
}
#endif