﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Brimborium.WebDavServer {
    /// <summary>
    /// Provides support for asynchronous lazy initialization. This type is fully threadsafe.
    /// </summary>
    /// <typeparam name="T">The type of object that is being asynchronously initialized.</typeparam>
    [DebuggerDisplay("State = {" + nameof(GetStateForDebugger) + "}")]
    [DebuggerTypeProxy(typeof(AsyncLazy<>.DebugView))]
    public sealed class AsyncLazy<T> {
        /// <summary>
        /// The synchronization object protecting <c>_instance</c>.
        /// </summary>
        private readonly object _mutex = new object();

        /// <summary>
        /// The underlying lazy task.
        /// </summary>
        private readonly Lazy<Task<T>> _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The asynchronous delegate that is invoked to produce the value when it is needed. May not be <c>null</c>.</param>
        public AsyncLazy(Func<Task<T>> factory) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            var factoryFunc = this.RunOnThreadPool(factory);
            this._instance = new Lazy<Task<T>>(factoryFunc);
        }

        /// <summary>
        /// The current status of the <see cref="Task"/> of this <see cref="AsyncLazy{T}"/>
        /// </summary>
        internal enum LazyState {
            /// <summary>
            /// The underlying task wasn't started yet
            /// </summary>
            NotStarted,

            /// <summary>
            /// The underlying task is still executing
            /// </summary>
            Executing,

            /// <summary>
            /// The underlying task is finished
            /// </summary>
            Completed,
        }

        /// <summary>
        /// Gets the resulting task.
        /// </summary>
        /// <remarks>
        /// Starts the asynchronous factory method, if it has not already started.
        /// </remarks>
        public Task<T> Task {
            get {
                lock (this._mutex) {
                    return this._instance.Value;
                }
            }
        }

        [DebuggerNonUserCode]
        internal LazyState GetStateForDebugger {
            get {
                if (!this._instance.IsValueCreated) {
                    return LazyState.NotStarted;
                }

                if (!this._instance.Value.IsCompleted) {
                    return LazyState.Executing;
                }

                return LazyState.Completed;
            }
        }

        /// <summary>
        /// Asynchronous infrastructure support. This method permits instances of <see cref="AsyncLazy&lt;T&gt;"/> to be await'ed.
        /// </summary>
        /// <returns>the task awaiter</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public TaskAwaiter<T> GetAwaiter() {
            return this.Task.GetAwaiter();
        }

        /// <summary>
        /// Asynchronous infrastructure support. This method permits instances of <see cref="AsyncLazy&lt;T&gt;"/> to be await'ed.
        /// </summary>
        /// <param name="continueOnCapturedContext">Continue on captured context?</param>
        /// <returns>The configured task awaiter</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext) {
            return this.Task.ConfigureAwait(continueOnCapturedContext);
        }

        private Func<Task<T>> RunOnThreadPool(Func<Task<T>> factory) {
            return () => System.Threading.Tasks.Task.Run(factory);
        }

        [DebuggerNonUserCode]
        internal sealed class DebugView {
            private readonly AsyncLazy<T> _lazy;

            public DebugView(AsyncLazy<T> lazy) {
                this._lazy = lazy;
            }

            public LazyState State => this._lazy.GetStateForDebugger;

            public Task Task {
                get {
                    if (!this._lazy._instance.IsValueCreated) {
                        throw new InvalidOperationException("Not yet created.");
                    }

                    return this._lazy._instance.Value;
                }
            }

            public T Value {
                get {
                    if (!this._lazy._instance.IsValueCreated || !this._lazy._instance.Value.IsCompleted) {
                        throw new InvalidOperationException("Not yet created.");
                    }

                    return this._lazy._instance.Value.Result;
                }
            }
        }
    }
}
