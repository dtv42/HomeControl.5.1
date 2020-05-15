// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModbusAttribute.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTest
{
    #region Using Directives

    using Xunit;

    using NModbus.Extensions;

    #endregion Using Directives

    /// <summary>
    /// Test class for testing the Modbus attribute.
    /// </summary>
    [Collection("Test Modbus Attribute Collection")]
    public class TestModbusAttribute
    {
        /// <summary>
        /// Internal class holding properties with various Modbus attributes.
        /// </summary>
        private class TestData
        {
            [Modbus]
            public ushort TestRegister { get; set; } = 1;

            [Modbus(offset: 100, access: ModbusAttribute.AccessModes.ReadOnly, length: 2)]
            public int TestInteger { get; set; } = -1;

            [Modbus(200, 2, ModbusAttribute.AccessModes.ReadOnly)]
            public float TestFloat { get; set; } = 1.23F;

            [Modbus(300, 4, ModbusAttribute.AccessModes.WriteOnly)]
            public double TestDouble { get; set; } = 1.23456789;
        }

        /// <summary>
        /// Property to test the various Modbus attributes.
        /// </summary>
        private TestData Test { get; } = new TestData();

        [Theory]
        [InlineData("TestRegister", 0, ModbusAttribute.AccessModes.ReadWrite, 1, (ushort)1)]
        [InlineData("TestInteger", 100, ModbusAttribute.AccessModes.ReadOnly, 2, (int)-1)]
        [InlineData("TestFloat", 200, ModbusAttribute.AccessModes.ReadOnly, 2, (float)1.23F)]
        [InlineData("TestDouble", 300, ModbusAttribute.AccessModes.WriteOnly, 4, (double)1.23456789)]
        public void TestAttribute(string property, ushort offset, ModbusAttribute.AccessModes access, ushort length, dynamic value)
        {
            var info = typeof(TestData).GetProperty(property);
            var attribute = ModbusAttribute.GetModbusAttribute(info);
            Assert.NotNull(info);
            Assert.NotNull(attribute);
            Assert.Equal(offset, attribute?.Offset);
            Assert.Equal(access, attribute?.Access);
            Assert.Equal(length, attribute?.Length);
            Assert.Equal(value, info.GetValue(Test));
        }
    }
}