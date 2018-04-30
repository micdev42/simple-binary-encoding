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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.SbeTool.Sbe.Dll;

namespace Org.SbeTool.Sbe.Tests
{
    [TestClass]
    public sealed class EndianessConverterTests
    {
        [TestMethod]
        [DataRow(short.MinValue)]
        [DataRow(short.MaxValue)]
        [DataRow((short)0)]
        public void ApplyShortWithLittleEndianShouldNoOp(short input)
        {
            var result = EndianessConverter.ApplyInt16(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(short.MinValue)]
        [DataRow(short.MaxValue)]
        [DataRow((short)0)]
        public void ApplyShortWithBigEndianShouldReverseBytes(short input)
        {
            var result = EndianessConverter.ApplyInt16(ByteOrder.BigEndian, input);

            short expected = BitConverter.ToInt16(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(ushort.MinValue)]
        [DataRow(ushort.MaxValue)]
        [DataRow((ushort)0)]
        public void ApplyUShortWithLittleEndianShouldNoOp(ushort input)
        {
            var result = EndianessConverter.ApplyUint16(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(ushort.MinValue)]
        [DataRow(ushort.MaxValue)]
        [DataRow((ushort)0)]
        public void ApplyUShortWithBigEndianShouldReverseBytes(ushort input)
        {
            var result = EndianessConverter.ApplyUint16(ByteOrder.BigEndian, input);

            ushort expected = BitConverter.ToUInt16(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(int.MinValue)]
        [DataRow(int.MaxValue)]
        [DataRow(0)]
        public void ApplyIntWithLittleEndianShouldNoOp(int input)
        {
            var result = EndianessConverter.ApplyInt32(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(int.MinValue)]
        [DataRow(int.MaxValue)]
        [DataRow((int)0)]
        public void ApplyIntWithBigEndianShouldReverseBytes(int input)
        {
            var result = EndianessConverter.ApplyInt32(ByteOrder.BigEndian, input);

            int expected = BitConverter.ToInt32(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(uint.MinValue)]
        [DataRow(uint.MaxValue)]
        [DataRow((uint)0)]
        public void ApplyUIntWithLittleEndianShouldNoOp(uint input)
        {
            var result = EndianessConverter.ApplyUint32(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(uint.MinValue)]
        [DataRow(uint.MaxValue)]
        [DataRow((uint)0)]
        public void ApplyUIntWithBigEndianShouldReverseBytes(uint input)
        {
            var result = EndianessConverter.ApplyUint32(ByteOrder.BigEndian, input);

            uint expected = BitConverter.ToUInt32(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(ulong.MinValue)]
        [DataRow(ulong.MaxValue)]
        [DataRow((ulong)0)]
        public void ApplyULongWithLittleEndianShouldNoOp(ulong input)
        {
            var result = EndianessConverter.ApplyUint64(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(ulong.MinValue)]
        [DataRow(ulong.MaxValue)]
        [DataRow((ulong)0)]
        public void ApplyULongWithBigEndianShouldReverseBytes(ulong input)
        {
            var result = EndianessConverter.ApplyUint64(ByteOrder.BigEndian, input);
            
            ulong expected = BitConverter.ToUInt64(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(long.MinValue)]
        [DataRow(long.MaxValue)]
        [DataRow(0)]
        public void ApplyLongWithLittleEndianShouldNoOp(long input)
        {
            var result = EndianessConverter.ApplyInt64(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(long.MinValue)]
        [DataRow(long.MaxValue)]
        [DataRow(0)]
        public void ApplyLongWithBigEndianShouldReverseBytes(long input)
        {
            var result = EndianessConverter.ApplyInt64(ByteOrder.BigEndian, input);

            long expected = BitConverter.ToInt64(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(double.MinValue)]
        [DataRow(double.MaxValue)]
        [DataRow(0)]
        public void ApplyDoubleWithLittleEndianShouldNoOp(double input)
        {
            var result = EndianessConverter.ApplyDouble(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(double.MinValue)]
        [DataRow(double.MaxValue)]
        [DataRow(0)]
        public void ApplyDoubleWithBigEndianShouldReverseBytes(double input)
        {
            var result = EndianessConverter.ApplyDouble(ByteOrder.BigEndian, input);

            double expected = BitConverter.ToDouble(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(float.MinValue)]
        [DataRow(float.MaxValue)]
        [DataRow(0)]
        public void ApplyFloatWithLittleEndianShouldNoOp(float input)
        {
            var result = EndianessConverter.ApplyFloat(ByteOrder.LittleEndian, input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        [DataRow(float.MinValue)]
        [DataRow(float.MaxValue)]
        [DataRow(0)]
        public void ApplyFloatWithBigEndianShouldReverseBytes(float input)
        {
            var result = EndianessConverter.ApplyFloat(ByteOrder.BigEndian, input);

            float expected = BitConverter.ToSingle(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            Assert.AreEqual(expected, result);
        }
    }
}
