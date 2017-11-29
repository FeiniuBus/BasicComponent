using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FeiniuBus.AspNetCore.Buffering
{
    public class BufferingReadStream : Stream
    {
        private const int MaxRentedBufferSize = 1024 * 1024; // 1MB
        private readonly long? _bufferLimit;
        private readonly ArrayPool<byte> _bytePool;

        private readonly Stream _innerStream;
        private readonly int _memoryThreshold;
        private readonly Func<string> _tempFileDirectoryAccessor;

        private Stream _buffer;
        private bool _completelyBuffered;
        private bool _inMemory = true;
        private bool _disposed;
        private byte[] _rentedBuffer;
        private string _tempFileDirectory;

        public BufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit,
            Func<string> tempFileDirectoryAccessor) : this(inner, memoryThreshold, bufferLimit,
            tempFileDirectoryAccessor, ArrayPool<byte>.Shared)
        {
        }

        public BufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit, string tempFileDirectory) :
            this(inner, memoryThreshold, bufferLimit, tempFileDirectory, ArrayPool<byte>.Shared)
        {
        }

        public BufferingReadStream(Stream innerStream, int memoryThreshold, long? bufferLimit,
            Func<string> tempFileDirectoryAccessor, ArrayPool<byte> bytePool)
        {
            if (innerStream == null)
                throw new ArgumentNullException(nameof(innerStream));
            if (tempFileDirectoryAccessor == null)
                throw new ArgumentNullException(nameof(tempFileDirectoryAccessor));

            _bytePool = bytePool;
            if (memoryThreshold < MaxRentedBufferSize)
            {
                _rentedBuffer = bytePool.Rent(memoryThreshold);
                _buffer = new MemoryStream(_rentedBuffer);
                _buffer.SetLength(0);
            }
            else
            {
                _buffer = new MemoryStream();
            }

            _innerStream = innerStream;
            _memoryThreshold = memoryThreshold;
            _bufferLimit = bufferLimit;
            _tempFileDirectoryAccessor = tempFileDirectoryAccessor;
        }

        public BufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit, string tempFileDirectory,
            ArrayPool<byte> bytePool)
        {
            if (inner == null)
                throw new ArgumentNullException(nameof(inner));
            if (tempFileDirectory == null)
                throw new ArgumentNullException(nameof(tempFileDirectory));

            _bytePool = bytePool;
            if (memoryThreshold < MaxRentedBufferSize)
            {
                _rentedBuffer = bytePool.Rent(memoryThreshold);
                _buffer = new MemoryStream(_rentedBuffer);
                _buffer.SetLength(0);
            }
            else
            {
                _buffer = new MemoryStream();
            }

            _innerStream = inner;
            _memoryThreshold = memoryThreshold;
            _bufferLimit = bufferLimit;
            _tempFileDirectory = tempFileDirectory;
        }

        public string TempFileName { get; private set; }
        public bool InMemory => _inMemory;
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _buffer.Length;

        public override long Position
        {
            get => _buffer.Position;
            set
            {
                ThrowIfDisposed();
                _buffer.Position = value;
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            if (_buffer.Position < _buffer.Length || _completelyBuffered)
            {
                // Just read from the buffer
                return _buffer.Read(buffer, offset, (int) Math.Min(count, _buffer.Length - _buffer.Position));
            }

            int read = _innerStream.Read(buffer, offset, count);

            if (_bufferLimit.HasValue && _bufferLimit - read < _buffer.Length)
            {
                Dispose();
                throw new IOException("Buffer limit exceeded.");
            }

            if (_inMemory && _buffer.Length + read > _memoryThreshold)
            {
                _inMemory = false;
                var oldBuffer = _buffer;
                _buffer = CreateTempFile();
                if (_rentedBuffer == null)
                {
                    oldBuffer.Position = 0;
                    var rentedBuffer = _bytePool.Rent(Math.Min((int) oldBuffer.Length, MaxRentedBufferSize));
                    var copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    while (copyRead > 0)
                    {
                        _buffer.Write(rentedBuffer, 0, copyRead);
                        copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    }
                    _bytePool.Return(rentedBuffer);
                }
                else
                {
                    _buffer.Write(_rentedBuffer, 0, (int)oldBuffer.Length);
                    _bytePool.Return(_rentedBuffer);
                    _rentedBuffer = null;
                }
            }

            if (read > 0)
            {
                _buffer.Write(buffer, offset, read);
            }
            else
            {
                _completelyBuffered = true;
            }

            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (_buffer.Position < _buffer.Length || _completelyBuffered)
            {
                // Just read from the buffer
                return await _buffer.ReadAsync(buffer, offset, (int) Math.Min(count, _buffer.Length - _buffer.Position),
                    cancellationToken);
            }

            int read = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

            if (_bufferLimit.HasValue && _bufferLimit - read < _buffer.Length)
            {
                Dispose();
                throw new IOException("Buffer limit exceeded.");
            }

            if (_inMemory && _buffer.Length + read > _memoryThreshold)
            {
                _inMemory = false;
                var oldBuffer = _buffer;
                _buffer = CreateTempFile();
                if (_rentedBuffer == null)
                {
                    oldBuffer.Position = 0;
                    var rentedBuffer = _bytePool.Rent(Math.Min((int) oldBuffer.Length, MaxRentedBufferSize));
                    // oldBuffer is a MemoryStream, no need to do async reads.
                    var copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    while (copyRead > 0)
                    {
                        await _buffer.WriteAsync(rentedBuffer, 0, copyRead, cancellationToken);
                        copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    }
                    _bytePool.Return(rentedBuffer);
                }
                else
                {
                    await _buffer.WriteAsync(_rentedBuffer, 0, (int)oldBuffer.Length, cancellationToken);
                    _bytePool.Return(_rentedBuffer);
                    _rentedBuffer = null;
                }
            }

            if (read > 0)
            {
                await _buffer.WriteAsync(buffer, offset, read, cancellationToken);
            }
            else
            {
                _completelyBuffered = true;
            }

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            if (!_completelyBuffered && origin == SeekOrigin.End)
            {
                throw new NotSupportedException("The content has not been fully buffered yet.");
            }
            if (!_completelyBuffered && origin == SeekOrigin.Current && offset + Position > Length)
            {
                throw new NotSupportedException("The content has not been fully buffered yet.");
            }
            if (!_completelyBuffered && origin == SeekOrigin.Begin && offset > Length)
            {
                throw new NotSupportedException("The content has not been fully buffered yet.");
            }

            return _buffer.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("This Stream only supports Read operations.");
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("This Stream only supports Read operations.");
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_rentedBuffer != null)
                    _bytePool.Return(_rentedBuffer);

                if (disposing)
                    _buffer.Dispose();
            }
        }

        private Stream CreateTempFile()
        {
            if (_tempFileDirectory == null)
            {
                Debug.Assert(_tempFileDirectoryAccessor != null);
                _tempFileDirectory = _tempFileDirectoryAccessor();
                Debug.Assert(_tempFileDirectory != null);
            }

            TempFileName = Path.Combine(_tempFileDirectory, "ASPNETCORE_" + Guid.NewGuid().ToString() + ".tmp");
            return new FileStream(TempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete, 1024 * 16,
                FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BufferingReadStream));
        }
    }
}