using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebDavServer.Utils
{
    /// <summary>
    /// This is some kind of a "View" to an underlying stream
    /// </summary>
    public class StreamView : Stream
    {
        private readonly Stream _baseStream;
        private long _position;

        private StreamView(Stream baseStream, long startPosition, long length)
        {
            this._baseStream = baseStream;
            this.Offset = startPosition;
            this.Length = length;
        }

        /// <inheritdoc />
        public override bool CanRead { get; } = true;

        /// <inheritdoc />
        public override bool CanSeek => this._baseStream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite { get; } = false;

        /// <inheritdoc />
        public override long Length { get; }

        /// <inheritdoc />
        public override long Position
        {
            get
            {
                return this._position;
            }

            set
            {
                if (this._position == value) {
                    return;
                }

                this._baseStream.Seek(value - this._position, SeekOrigin.Current);
                this._position = value;
            }
        }

        private long Offset { get; }

        /// <summary>
        /// Creates a new stream view
        /// </summary>
        /// <remarks>
        /// The <paramref name="baseStream"/> must be at position 0.
        /// </remarks>
        /// <param name="baseStream">The underlying stream</param>
        /// <param name="position">The start position</param>
        /// <param name="length">The length of the data to be read from the underlying stream</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The new stream view</returns>
        public static async Task<StreamView> CreateAsync(
            Stream baseStream,
            long position,
            long length,
            CancellationToken ct)
        {
            if (baseStream.CanSeek)
            {
                baseStream.Seek(position, SeekOrigin.Begin);
            }
            else
            {
                await SkipAsync(baseStream, position, ct).ConfigureAwait(false);
            }

            return new StreamView(baseStream, position, length);
        }

        /// <inheritdoc />
        public override void Flush()
        {
            this._baseStream.Flush();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = Math.Min(this.Length - this._position, count);
            if (remaining == 0) {
                return 0;
            }

            var readCount = this._baseStream.Read(buffer, offset, (int)remaining);
            this._position += readCount;
            return readCount;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            long result;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    var newPosFromBegin = this.Offset + offset;
                    if (newPosFromBegin < this.Offset) {
                        newPosFromBegin = this.Offset;
                    }

                    if (newPosFromBegin > this.Offset + this.Length) {
                        newPosFromBegin = this.Offset + this.Length;
                    }

                    result = this._baseStream.Seek(newPosFromBegin, origin);
                    this._position = offset;
                    break;
                case SeekOrigin.Current:
                    var newPosFromCurrent = this.Offset + this._position + offset;
                    if (newPosFromCurrent < this.Offset) {
                        newPosFromCurrent = this.Offset;
                    }

                    if (newPosFromCurrent > this.Offset + this.Length) {
                        newPosFromCurrent = this.Offset + this.Length;
                    }

                    var newOffset = newPosFromCurrent - (this.Offset + this._position);
                    result = this._baseStream.Seek(newOffset, SeekOrigin.Current);
                    this._position = newPosFromCurrent - this.Offset;
                    break;
                case SeekOrigin.End:
                    result = this._baseStream.Seek(this.Offset + this.Length + offset, SeekOrigin.Begin);
                    this._position = this.Length + offset;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) {
                this._baseStream.Dispose();
            }
        }

        private static async Task SkipAsync(Stream baseStream, long count, CancellationToken ct)
        {
            var buffer = new byte[65536];
            while (count != 0)
            {
                var blockSize = Math.Min(65536, count);
                await baseStream.ReadAsync(buffer, 0, (int)blockSize, ct).ConfigureAwait(false);
                count -= blockSize;
            }
        }
    }
}
