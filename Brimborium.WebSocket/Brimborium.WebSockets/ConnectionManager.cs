using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Brimborium.WebSockets {
    public class ConnectionManager {
        private Dictionary<WebSocket, ConnectionSocketState> _StateBySocket;
        private readonly ArrayPool<byte> _ArrayPool;

        public ArrayPool<byte> ArrayPool => this._ArrayPool;

        public ConnectionManager() {
            
            this._StateBySocket = new Dictionary<WebSocket, ConnectionSocketState>();
            this._ArrayPool = ArrayPool<byte>.Shared;
        }

        internal void OnConnected(WebSocket socket, ConnectionSocketState socketState) {
            lock (this) {
                this._StateBySocket.Add(socket, socketState);
            }
        }

        internal SocketMessage3 OnStartMessage(WebSocket socket, ConnectionSocketState socketState) {
            return new SocketMessage(this.RentBuffer());
        }


        internal byte[] RentBuffer() {
            return this._ArrayPool.Rent(1024 * 64);
        }

        internal async Task<bool> ReceiveTextAsync(WebSocket socket, ConnectionSocketState socketState, WebSocketReceiveResult result, byte[] buffer) {
            //var socketState=GetState(socket);
            result.Count
            //socketState.
            // socketState.ReceiveTextAsync(result, buffer)
            await Task.CompletedTask;
            //if (result.EndOfMessage) {
            //    return true;
            //} else {
            //    return false;
            //}
            return result.EndOfMessage;
        }



        internal async Task<bool> ReceiveBinaryAsync(WebSocket socket, ConnectionSocketState socketState, WebSocketReceiveResult result, byte[] buffer) {
            await Task.CompletedTask;
            return result.EndOfMessage;
        }

        internal async Task OnDisconnected(WebSocket socket, ConnectionSocketState socketState) {
            await Task.CompletedTask;
        }


        internal void OnDispose(WebSocket socket, ConnectionSocketState socketState) {
            lock (this) { 
                this._StateBySocket.Remove(socket); ;
            }
        }


        private ConnectionSocketState GetState(WebSocket socket) {
            lock (this) {
                if (this._StateBySocket.TryGetValue(socket, out var socketState)) {
                    return socketState;
                } else {
                    throw new InvalidOperationException("unknown socket");
                }
            }
        }
    }
    public class ConnectionSocketState {
    }
    //public class SocketMessage3 {
    //    private readonly int _MinimumReceiveCapacity;
    //    private readonly int _InitialCapacity;
    //    private ArrayBufferWriter<byte> _Writer;
    //    public SocketMessage3(int initialCapacity, int minimumReceiveCapacity) {
    //        this._MinimumReceiveCapacity = System.Math.Max(minimumReceiveCapacity, 16 * 1024);
    //        initialCapacity = System.Math.Max(minimumReceiveCapacity * 4, initialCapacity);
    //        this._Writer = new ArrayBufferWriter<byte>(initialCapacity);
    //    }
    //    internal ArraySegment<byte> GetArraySegment() {
    //        return new ArraySegment<byte>(this._Writer.);
    //        //this._Writer.GetMemory(this._MinimumReceiveCapacity);
    //    }
    //}
    public class SocketMessage {
        private readonly ArrayPool<byte> _ArrayPool;
        private readonly int _MinimumReceiveCapacity;
        private byte[] _Buffer;
        private int _Offset;

        public SocketMessage(ArrayPool<byte> arrayPool, int initialCapacity, int minimumReceiveCapacity) {
            this._ArrayPool = arrayPool;
            this._MinimumReceiveCapacity = System.Math.Max(minimumReceiveCapacity, 32*1024);
            this._Offset = 0;
            this._Buffer = arrayPool.Rent(System.Math.Max(initialCapacity, this._MinimumReceiveCapacity));
        }

        public byte[] Buffer => this._Buffer;

        internal ArraySegment<byte> GetArraySegment() {
            if ((this._Offset + this._MinimumReceiveCapacity) <= this._Buffer.Length) {
                // OK
            } else {
                byte[] oldBuffer = this._Buffer;
                byte[] nextBuffer = this._ArrayPool.Rent(this._Buffer.Length + this._MinimumReceiveCapacity);
                oldBuffer.AsSpan().CopyTo(nextBuffer);
                this._Buffer = nextBuffer;
                this._ArrayPool.Return(oldBuffer);
            }
            if ((this._Offset + this._MinimumReceiveCapacity) <= this._Buffer.Length) {
                // OK
            } else {
                throw new InvalidOperationException($"{this._Offset} + {this._MinimumReceiveCapacity} <= {this._Buffer.Length}");
            }
            return new ArraySegment<byte>(this._Buffer).Slice(this._Offset);
        }

        internal void AdjustCount(int count) {
            this._Offset += count;
        }
    }
}