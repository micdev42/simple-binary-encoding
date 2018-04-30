// Portions Copyright (C) 2017 MarketFactory, Inc
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.SbeTool.Sbe.Dll;

namespace Org.SbeTool.Sbe.Tests
{
    [TestClass]
    public sealed unsafe class DirectBufferTests
    {
        private byte[] _buffer;
        private DirectBuffer _directBuffer;
        private byte* _pBuffer;

        [TestInitialize]
        public void SetUp()
        {
            _buffer = new byte[16];
            _directBuffer = new DirectBuffer(_buffer);
            var handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

            _pBuffer = (byte*) handle.AddrOfPinnedObject().ToPointer();
        }

        [TestMethod]
        public void CheckPositionShouldNotThrowWhenPositionIsInRange()
        {
            _directBuffer.CheckLimit(_buffer.Length);
        }

        [TestMethod]
        public void CheckPositionShouldThrowWhenPositionIsNotInRange()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _directBuffer.CheckLimit(_buffer.Length + 1));
        }

        [TestMethod]
        [DataRow(long.MaxValue)]
        [DataRow(long.MinValue)]
        [DataRow(0)]
        public void ConstructFromNativeBuffer(long value)
        {
            var managedBuffer = new byte[16];
            var handle = GCHandle.Alloc(managedBuffer, GCHandleType.Pinned);
            var unmanagedBuffer = (byte*) handle.AddrOfPinnedObject().ToPointer();

            const int index = 0;

            using (var directBufferFromUnmanagedbuffer = new DirectBuffer(unmanagedBuffer, managedBuffer.Length))
            {
                directBufferFromUnmanagedbuffer.Int64PutLittleEndian(index, value);
                Assert.AreEqual(value, *(long*) (unmanagedBuffer + index));
            }
        }

        [TestMethod]
        [DataRow(long.MaxValue)]
        [DataRow(long.MinValue)]
        [DataRow(0)]
        public void Recycle(long value)
        {
            var directBuffer = new DirectBuffer();
            var firstBuffer = new byte[16];
            directBuffer.Wrap(firstBuffer);
            int index = 0;

            directBuffer.Int64PutLittleEndian(index, value);
            Assert.AreEqual(value, BitConverter.ToInt64(firstBuffer, index));

            index = 1;
            var secondBuffer = new byte[16];
            var secondBufferHandle = GCHandle.Alloc(secondBuffer, GCHandleType.Pinned);
            var secondUnmanagedBuffer = (byte*) secondBufferHandle.AddrOfPinnedObject().ToPointer();
            directBuffer.Wrap(secondUnmanagedBuffer, 16);
            directBuffer.Int64PutLittleEndian(index, value);
            Assert.AreEqual(value, BitConverter.ToInt64(secondBuffer, index));

            directBuffer.Dispose();
        }

        [TestMethod]
        public void Reallocate()
        {
            const int initialBufferSize = 8;
            var initialBuffer = new byte[initialBufferSize];

            const int biggerBufferSize = 100;
            var biggerBuffer = new byte[biggerBufferSize];

            var reallocableBuffer = new DirectBuffer(initialBuffer,
                (existingSize, requestedSize) =>
                {
                    Assert.AreEqual(initialBufferSize, existingSize);
                    Assert.AreEqual(16, requestedSize);
                    return biggerBuffer;
                });

            reallocableBuffer.CheckLimit(8);
            reallocableBuffer.Int64PutLittleEndian(0, 1);
            Assert.AreEqual(initialBufferSize, reallocableBuffer.Capacity);

            reallocableBuffer.CheckLimit(16);
            reallocableBuffer.Int64PutLittleEndian(8, 2);
            Assert.AreEqual(biggerBufferSize, reallocableBuffer.Capacity);

            Assert.AreEqual(1, BitConverter.ToInt64(biggerBuffer, 0));
            Assert.AreEqual(2, BitConverter.ToInt64(biggerBuffer, 8));
        }

        [TestMethod]
        public void ReallocateFailure()
        {
            const int initialBufferSize = 8;
            var initialBuffer = new byte[initialBufferSize];
            var reallocableBuffer = new DirectBuffer(initialBuffer, (existingSize, requestedSize) =>
            {
                Assert.AreEqual(initialBufferSize, existingSize);
                Assert.AreEqual(16, requestedSize);
                return null;
            });

            reallocableBuffer.CheckLimit(8);
            reallocableBuffer.Int64PutLittleEndian(0, 1);
            Assert.AreEqual(initialBufferSize, reallocableBuffer.Capacity);
            Assert.ThrowsException<IndexOutOfRangeException>(() => reallocableBuffer.CheckLimit(16));
        }

        #region Byte

        [TestMethod]
        [DataRow(byte.MaxValue, 1)]
        [DataRow(byte.MinValue, 2)]
        [DataRow((byte)0, 3)]
        public void ShouldPutByte(byte value, int index)
        {
            _directBuffer.CharPut(index, value);

            Assert.AreEqual(value, _buffer[index]);
        }

        [TestMethod]
        [DataRow(byte.MaxValue, 1)]
        [DataRow(byte.MinValue, 2)]
        [DataRow((byte)0, 3)]
        public void ShouldGetByte(byte value, int index)
        {
            _buffer[index] = value;

            var result = _directBuffer.CharGet(index);

            Assert.AreEqual(value, result);
        }

        #endregion

        #region Int8

        [TestMethod]
        [DataRow(sbyte.MaxValue, 1)]
        [DataRow(sbyte.MinValue, 2)]
        [DataRow((sbyte)0, 3)]
        public void ShouldPutInt8(sbyte value, int index)
        {
            _directBuffer.Int8Put(index, value);

            Assert.AreEqual(value, *(sbyte*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(sbyte.MaxValue, 1)]
        [DataRow(sbyte.MinValue, 2)]
        [DataRow((sbyte)0, 3)]
        public void ShouldGetInt8(sbyte value, int index)
        {
            _buffer[index] = *(byte*) &value;

            var result = _directBuffer.Int8Get(index);

            Assert.AreEqual(value, result);
        }

        #endregion

        #region Int16

        [TestMethod]
        [DataRow(short.MaxValue, 1)]
        [DataRow(short.MinValue, 2)]
        [DataRow((short)0, 3)]
        public void ShouldPutInt16LittleEndian(short value, int index)
        {
            _directBuffer.Int16PutLittleEndian(index, value);

            Assert.AreEqual(value, *(short*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(short.MaxValue, 1)]
        [DataRow(short.MinValue, 2)]
        [DataRow((short)0, 3)]
        public void ShouldPutInt16BigEndian(short value, int index)
        {
            _directBuffer.Int16PutBigEndian(index, value);

            short expected = EndianessConverter.ApplyInt16(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(short*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(short.MaxValue, 1)]
        [DataRow(short.MinValue, 2)]
        [DataRow((short)0, 3)]
        public void ShouldGetInt16LittleEndian(short value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 2);

            short result = _directBuffer.Int16GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(short.MaxValue, 1)]
        [DataRow(short.MinValue, 2)]
        [DataRow((short)0, 3)]
        public void ShouldGetInt16BigEndian(short value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 2);

            short result = _directBuffer.Int16GetBigEndian(index);

            short expected = EndianessConverter.ApplyInt16(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Int32

        [TestMethod]
        [DataRow(int.MaxValue, 1)]
        [DataRow(int.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldPutInt32LittleEndian(int value, int index)
        {
            _directBuffer.Int32PutLittleEndian(index, value);

            Assert.AreEqual(value, *(int*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(int.MaxValue, 1)]
        [DataRow(int.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldPutInt32BigEndian(int value, int index)
        {
            _directBuffer.Int32PutBigEndian(index, value);

            int expected = EndianessConverter.ApplyInt32(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(int*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(int.MaxValue, 1)]
        [DataRow(int.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldGetInt32LittleEndian(int value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            int result = _directBuffer.Int32GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(int.MaxValue, 1)]
        [DataRow(int.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldGetInt32BigEndian(int value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            int result = _directBuffer.Int32GetBigEndian(index);

            int expected = EndianessConverter.ApplyInt32(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Int64

        [TestMethod]
        [DataRow(long.MaxValue, 1)]
        [DataRow(long.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldPutInt64LittleEndian(long value, int index)
        {
            _directBuffer.Int64PutLittleEndian(index, value);

            Assert.AreEqual(value, *(long*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(long.MaxValue, 1)]
        [DataRow(long.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldPutInt64BigEndian(long value, int index)
        {
            _directBuffer.Int64PutBigEndian(index, value);

            long expected = EndianessConverter.ApplyInt64(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(long*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(long.MaxValue, 1)]
        [DataRow(long.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldGetInt64LittleEndian(long value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            long result = _directBuffer.Int64GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(long.MaxValue, 1)]
        [DataRow(long.MinValue, 2)]
        [DataRow(0, 3)]
        public void ShouldGetInt64BigEndian(long value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            long result = _directBuffer.Int64GetBigEndian(index);
            long expected = EndianessConverter.ApplyInt64(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region UInt8

        [TestMethod]
        [DataRow(byte.MaxValue, 1)]
        [DataRow(byte.MinValue, 2)]
        [DataRow((byte)0, 3)]
        public void ShouldPutUInt8(byte value, int index)
        {
            _directBuffer.Uint8Put(index, value);

            Assert.AreEqual(value, *(_pBuffer + index));
        }

        [TestMethod]
        [DataRow(byte.MaxValue, 1)]
        [DataRow(byte.MinValue, 2)]
        [DataRow((byte)0, 3)]
        public void ShouldGetUInt8(byte value, int index)
        {
            _buffer[index] = *&value;

            byte result = _directBuffer.Uint8Get(index);

            Assert.AreEqual(value, result);
        }

        #endregion

        #region UInt16

        [TestMethod]
        [DataRow(ushort.MaxValue, 1)]
        [DataRow(ushort.MinValue, 2)]
        public void ShouldPutUInt16LittleEndian(ushort value, int index)
        {
            _directBuffer.Uint16PutLittleEndian(index, value);

            Assert.AreEqual(value, *(ushort*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(ushort.MaxValue, 1)]
        [DataRow(ushort.MinValue, 2)]
        public void ShouldPutUInt16BigEndian(ushort value, int index)
        {
            _directBuffer.Uint16PutBigEndian(index, value);

            ushort expected = EndianessConverter.ApplyUint16(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(ushort*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(ushort.MaxValue, 1)]
        [DataRow(ushort.MinValue, 2)]
        public void ShouldGetUInt16LittleEndian(ushort value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 2);

            ushort result = _directBuffer.Uint16GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(ushort.MaxValue, 1)]
        [DataRow(ushort.MinValue, 2)]
        public void ShouldGetUInt16BigEndian(ushort value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 2);

            ushort result = _directBuffer.Uint16GetBigEndian(index);

            ushort expected = EndianessConverter.ApplyUint16(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region UInt32

        [TestMethod]
        [DataRow(uint.MaxValue, 1)]
        [DataRow(uint.MinValue, 2)]
        public void ShouldPutUInt32LittleEndian(uint value, int index)
        {
            _directBuffer.Uint32PutLittleEndian(index, value);

            Assert.AreEqual(value, *(uint*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(uint.MaxValue, 1)]
        [DataRow(uint.MinValue, 2)]
        public void ShouldPutUInt32BigEndian(uint value, int index)
        {
            _directBuffer.Uint32PutBigEndian(index, value);

            uint expected = EndianessConverter.ApplyUint32(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(uint*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(uint.MaxValue, 1)]
        [DataRow(uint.MinValue, 2)]
        public void ShouldGetUInt32LittleEndian(uint value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            uint result = _directBuffer.Uint32GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(uint.MaxValue, 1)]
        [DataRow(uint.MinValue, 2)]
        public void ShouldGetUInt32BigEndian(uint value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            uint result = _directBuffer.Uint32GetBigEndian(index);

            uint expected = EndianessConverter.ApplyUint32(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region UInt64

        [TestMethod]
        [DataRow(ulong.MaxValue, 1)]
        [DataRow(ulong.MinValue, 2)]
        public void ShouldPutUInt64LittleEndian(ulong value, int index)
        {
            _directBuffer.Uint64PutLittleEndian(index, value);

            Assert.AreEqual(value, *(ulong*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(ulong.MaxValue, 1)]
        [DataRow(ulong.MinValue, 2)]
        public void ShouldPutUInt64BigEndian(ulong value, int index)
        {
            _directBuffer.Uint64PutBigEndian(index, value);

            ulong expected = EndianessConverter.ApplyUint64(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(ulong*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(ulong.MaxValue, 1)]
        [DataRow(ulong.MinValue, 2)]
        public void ShouldGetUInt64LittleEndian(ulong value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            ulong result = _directBuffer.Uint64GetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(ulong.MaxValue, 1)]
        [DataRow(ulong.MinValue, 2)]
        public void ShouldGetUInt64BigEndian(ulong value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            ulong result = _directBuffer.Uint64GetBigEndian(index);

            ulong expected = EndianessConverter.ApplyUint64(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Float

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(float.MinValue, 2)]
        [DataRow(float.MaxValue, 3)]
        public void ShouldPutFloatLittleEndian(float value, int index)
        {
            _directBuffer.FloatPutLittleEndian(index, value);

            Assert.AreEqual(value, *(float*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(float.MinValue, 2)]
        [DataRow(float.MaxValue, 3)]
        public void ShouldPutFloatBigEndian(float value, int index)
        {
            _directBuffer.FloatPutBigEndian(index, value);

            float expected = EndianessConverter.ApplyFloat(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(float*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(float.MinValue, 2)]
        [DataRow(float.MaxValue, 3)]
        public void ShouldGetFloatLittleEndian(float value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            float result = _directBuffer.FloatGetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(float.MinValue, 2)]
        [DataRow(float.MaxValue, 3)]
        public void ShouldGetFloatBigEndian(float value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 4);

            float result = _directBuffer.FloatGetBigEndian(index);

            float expected = EndianessConverter.ApplyFloat(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Double

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(double.MinValue, 2)]
        [DataRow(double.MaxValue, 3)]
        public void ShouldPutDoubleLittleEndian(double value, int index)
        {
            _directBuffer.DoublePutLittleEndian(index, value);

            Assert.AreEqual(value, *(double*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(double.MinValue, 2)]
        [DataRow(double.MaxValue, 3)]
        public void ShouldPutDoubleBigEndian(double value, int index)
        {
            _directBuffer.DoublePutBigEndian(index, value);

            double expected = EndianessConverter.ApplyDouble(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, *(double*) (_pBuffer + index));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(double.MinValue, 2)]
        [DataRow(double.MaxValue, 3)]
        public void ShouldGetDoubleLittleEndian(double value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            double result = _directBuffer.DoubleGetLittleEndian(index);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(double.MinValue, 2)]
        [DataRow(double.MaxValue, 3)]
        public void ShouldGetDoubleBigEndian(double value, int index)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, _buffer, index, 8);

            double result = _directBuffer.DoubleGetBigEndian(index);

            double expected = EndianessConverter.ApplyDouble(ByteOrder.BigEndian, value);
            Assert.AreEqual(expected, result);
        }

        #endregion
    }
}
