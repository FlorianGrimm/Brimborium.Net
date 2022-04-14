using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebSockets {
    public class WebSocketManagerMiddleware {
        private readonly RequestDelegate _Next;
        private readonly ConnectionManager _ConnectionManager;
        private readonly ILogger<WebSocketManagerMiddleware> _Logger;
        private readonly ArrayPool<byte> _ArrayPool;

        public WebSocketManagerMiddleware(
            RequestDelegate next,
            ConnectionManager connectionManager,
            ILogger<WebSocketManagerMiddleware> logger
            ) {
            this._Next = next;
            this._ConnectionManager = connectionManager;
            this._Logger = logger;
            this._ArrayPool = connectionManager.ArrayPool;
        }

        public Task InvokeAsync(HttpContext context) {
            if (context.WebSockets.IsWebSocketRequest) {
                return this.InvokeCore(context);
            }
            return this._Next(context);
        }

        private async Task InvokeCore(HttpContext context) {
            // context.User.
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var socketState = new ConnectionSocketState();
            this._ConnectionManager.OnConnected(socket, socketState);

            {
                SocketMessage3 socketMessage = this._ConnectionManager.OnStartMessage(socket, socketState);
                byte[]? buffer = null;
                while (socket.State == WebSocketState.Open) {
                    if (buffer == null) {
                        buffer = this._ConnectionManager.RentBuffer();

                    }


                    WebSocketReceiveResult? receiveResult = null;
                    try {
                        receiveResult = await socket.ReceiveAsync(
                            buffer: socketMessage.GetArraySegment(),
                            //buffer: new ArraySegment<byte>(buffer),
                            cancellationToken: CancellationToken.None);
                    } catch (System.Exception error) {
                        this._Logger.LogError(error, "TODO");
                    }

                    if (receiveResult is not null) {
                        try {
                            if (receiveResult.MessageType == WebSocketMessageType.Text) {
                                var bufferCanBeReused = await _ConnectionManager.ReceiveTextAsync(socket, socketState, receiveResult, buffer);
                                if (bufferCanBeReused) {
                                    //OK
                                } else {
                                    buffer = null;
                                }
                            } else if (receiveResult.MessageType == WebSocketMessageType.Binary) {
                                var bufferCanBeReused = await _ConnectionManager.ReceiveBinaryAsync(socket, socketState, receiveResult, buffer);
                                if (bufferCanBeReused) {
                                    //OK
                                } else {
                                    buffer = null;
                                }
                            } else if (receiveResult.MessageType == WebSocketMessageType.Close) {
                                await _ConnectionManager.OnDisconnected(socket, socketState);
                                this._ArrayPool.Return(buffer);
                                buffer = null;
                            }
                        } catch (System.Exception error) {
                            this._Logger.LogError(error, "TODO");
                        }
                    } else {
                        // ReceiveAsync return an error -> good bye?
                        if (socket.State == WebSocketState.Open) {
                            try {
                                await socket.CloseAsync(WebSocketCloseStatus.ProtocolError, null, CancellationToken.None);
                            } catch { 
                            }
                        }
                    }
                }
            }
            this._ConnectionManager.OnDispose(socket, socketState);
            try {
                socket.Dispose();
            } catch { }

            //await Receive(socket, async (result, buffer) => {
            //    if (result.MessageType == WebSocketMessageType.Text) {
            //        await _ConnectionManager.ReceiveTextAsync(socket, result, buffer);
            //        return;
            //    } else if (result.MessageType == WebSocketMessageType.Binary) {
            //        await _ConnectionManager.ReceiveBinaryAsync(socket, result, buffer);
            //        return;
            //    } else if (result.MessageType == WebSocketMessageType.Close) {
            //        await _ConnectionManager.OnDisconnected(socket);
            //        return;
            //    }
            //});

        }

        //private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage) {
        //    byte[]? buffer = null;
        //    while (socket.State == WebSocketState.Open) {
        //        if (buffer == null) { 
        //            buffer = this._ArrayPool.Rent(1024 * 4);
        //        }

        //        var result = await socket.ReceiveAsync(
        //            buffer: new ArraySegment<byte>(buffer),
        //            cancellationToken: CancellationToken.None);

        //        handleMessage(result, buffer);
        //        this._ArrayPool.Return(buffer);
        //    }
        //}
    }
}
