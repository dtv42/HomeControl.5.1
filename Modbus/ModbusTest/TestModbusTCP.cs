// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModbusTCP.cs" company="DTV-Online">
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

    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;

    using Xunit;

    using ModbusLib.Models;
    using ModbusTCP.Models;
    using UtilityLib;

    #endregion

    public class TestModbusTCP : IClassFixture<WebApplicationFactory<ModbusTCP.Startup>>
    {
        private readonly WebApplicationFactory<ModbusTCP.Startup> _factory;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions();

        public TestModbusTCP(WebApplicationFactory<ModbusTCP.Startup> factory)
        {
            _factory = factory;
            _options.AddDefaultOptions();
        }
        
        [Theory]
        [InlineData("/swagger")]
        [InlineData("/swagger/v1/swagger.json")]
        public async Task TestSwaggerEndpoints(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("/coil/0")]
        [InlineData("/coils/0")]
        [InlineData("/discreteinput/0")]
        [InlineData("/discreteinputs/0")]
        [InlineData("/holdingregister/0")]
        [InlineData("/holdingregisters/0")]
        [InlineData("/inputregister/0")]
        [InlineData("/inputregisters/0")]

        [InlineData("/rosingle/string/0")]
        [InlineData("/rosingle/hexstring/0")]
        [InlineData("/rosingle/bits/0")]
        [InlineData("/rosingle/bool/0")]
        [InlineData("/rosingle/short/0")]
        [InlineData("/rosingle/ushort/0")]
        [InlineData("/rosingle/int32/0")]
        [InlineData("/rosingle/uint32/0")]
        [InlineData("/rosingle/float/0")]
        [InlineData("/rosingle/double/0")]
        [InlineData("/rosingle/long/0")]
        [InlineData("/rosingle/ulong/0")]

        [InlineData("/roarray/bool/0")]
        [InlineData("/roarray/bytes/0")]
        [InlineData("/roarray/short/0")]
        [InlineData("/roarray/ushort/0")]
        [InlineData("/roarray/int32/0")]
        [InlineData("/roarray/uint32/0")]
        [InlineData("/roarray/float/0")]
        [InlineData("/roarray/double/0")]
        [InlineData("/roarray/long/0")]
        [InlineData("/roarray/ulong/0")]

        [InlineData("/rwsingle/string/0")]
        [InlineData("/rwsingle/hexstring/0")]
        [InlineData("/rwsingle/bits/0")]
        [InlineData("/rwsingle/bool/0")]
        [InlineData("/rwsingle/short/0")]
        [InlineData("/rwsingle/ushort/0")]
        [InlineData("/rwsingle/int32/0")]
        [InlineData("/rwsingle/uint32/0")]
        [InlineData("/rwsingle/float/0")]
        [InlineData("/rwsingle/double/0")]
        [InlineData("/rwsingle/long/0")]
        [InlineData("/rwsingle/ulong/0")]

        [InlineData("/rwarray/bool/0")]
        [InlineData("/rwarray/bytes/0")]
        [InlineData("/rwarray/short/0")]
        [InlineData("/rwarray/ushort/0")]
        [InlineData("/rwarray/int32/0")]
        [InlineData("/rwarray/uint32/0")]
        [InlineData("/rwarray/float/0")]
        [InlineData("/rwarray/double/0")]
        [InlineData("/rwarray/long/0")]
        [InlineData("/rwarray/ulong/0")]

        [InlineData("/settings")]
        public async Task TestEndpoints(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/coil/0",            typeof(bool)    )]
        [InlineData("/discreteinput/0",   typeof(bool)    )]
        [InlineData("/holdingregister/0", typeof(ushort)  )]
        [InlineData("/inputregister/0",   typeof(ushort)  )]
                                                          
        [InlineData("/rosingle/bool/0",   typeof(bool)    )]
        [InlineData("/rosingle/short/0",  typeof(short)   )]
        [InlineData("/rosingle/ushort/0", typeof(ushort)  )]
        [InlineData("/rosingle/int32/0",  typeof(int)     )]
        [InlineData("/rosingle/uint32/0", typeof(uint)    )]
        [InlineData("/rosingle/float/0",  typeof(float)   )]
        [InlineData("/rosingle/double/0", typeof(double)  )]
        [InlineData("/rosingle/long/0",   typeof(long)    )]
        [InlineData("/rosingle/ulong/0",  typeof(ulong)   )]
                                                          
        [InlineData("/rwsingle/bool/0",   typeof(bool)    )]
        [InlineData("/rwsingle/short/0",  typeof(short)   )]
        [InlineData("/rwsingle/ushort/0", typeof(ushort)  )]
        [InlineData("/rwsingle/int32/0",  typeof(int)     )]
        [InlineData("/rwsingle/uint32/0", typeof(uint)    )]
        [InlineData("/rwsingle/float/0",  typeof(float)   )]
        [InlineData("/rwsingle/double/0", typeof(double)  )]
        [InlineData("/rwsingle/long/0",   typeof(long)    )]
        [InlineData("/rwsingle/ulong/0",  typeof(ulong)   )]
        public async Task TestGetResponseData(string url, Type type)
        {
            // Arrange
            var responseType = typeof(ModbusResponseData<>).MakeGenericType(type);
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonSerializer.Deserialize(json, responseType, _options);
            Assert.NotNull(data);
            Assert.NotNull(data.Value);
        }

        [Theory]
        [InlineData("/rosingle/string/0"   )]
        [InlineData("/rosingle/hexstring/0")]
        [InlineData("/rosingle/bits/0"     )]
        [InlineData("/rwsingle/string/0"   )]
        [InlineData("/rwsingle/hexstring/0")]
        [InlineData("/rwsingle/bits/0"     )]
        public async Task TestGetResponseDataString(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ModbusResponseStringData>(json, _options);
            Assert.NotNull(data);
            Assert.NotNull(data.Value);
        }

        [Theory]
        [InlineData("/coils/0?number=1",            typeof(bool)  )]
        [InlineData("/discreteinputs/0?number=1",   typeof(bool)  )]
        [InlineData("/holdingregisters/0?number=1", typeof(ushort))]
        [InlineData("/inputregisters/0?number=1",   typeof(ushort))]

        [InlineData("/roarray/bool/0?number=1",     typeof(bool)  )]
        [InlineData("/roarray/bytes/0?number=1",    typeof(byte)  )]
        [InlineData("/roarray/short/0?number=1",    typeof(short) )]
        [InlineData("/roarray/ushort/0?number=1",   typeof(ushort))]
        [InlineData("/roarray/int32/0?number=1",    typeof(int)   )]
        [InlineData("/roarray/uint32/0?number=1",   typeof(uint)  )]
        [InlineData("/roarray/float/0?number=1",    typeof(float) )]
        [InlineData("/roarray/double/0?number=1",   typeof(double))]
        [InlineData("/roarray/long/0?number=1",     typeof(long)  )]
        [InlineData("/roarray/ulong/0?number=1",    typeof(ulong) )]

        [InlineData("/rwarray/bool/0?number=1",     typeof(bool)  )]
        [InlineData("/rwarray/bytes/0?number=1",    typeof(byte)  )]
        [InlineData("/rwarray/short/0?number=1",    typeof(short) )]
        [InlineData("/rwarray/ushort/0?number=1",   typeof(ushort))]
        [InlineData("/rwarray/int32/0?number=1",    typeof(int)   )]
        [InlineData("/rwarray/uint32/0?number=1",   typeof(uint)  )]
        [InlineData("/rwarray/float/0?number=1",    typeof(float) )]
        [InlineData("/rwarray/double/0?number=1",   typeof(double))]
        [InlineData("/rwarray/long/0?number=1",     typeof(long)  )]
        [InlineData("/rwarray/ulong/0?number=1",    typeof(ulong ))]
        public async Task TestGetResponseDataArray(string url, Type type)
        {
            // Arrange
            var responseType = typeof(ModbusResponseArrayData<>).MakeGenericType(type);
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonSerializer.Deserialize(json, responseType, _options);
            Assert.NotNull(data);
            Assert.NotEmpty(data.Values);
            Assert.Equal(1, data.Values.Length);
            Assert.NotNull(data.Values[0]);
        }

        [Theory]
        [InlineData("/coil/0",            true,        typeof(bool)  )]
        [InlineData("/holdingregister/0", (ushort)1,   typeof(ushort))]
        [InlineData("/rwsingle/bool/0",   true,        typeof(bool)  )]
        [InlineData("/rwsingle/short/0",  (short)1,    typeof(short) )]
        [InlineData("/rwsingle/ushort/0", (ushort)1,   typeof(ushort))]
        [InlineData("/rwsingle/int32/0",  1,           typeof(int)   )]
        [InlineData("/rwsingle/uint32/0", (uint)1,     typeof(uint)  )]
        [InlineData("/rwsingle/float/0",  (float)1.0F, typeof(float) )]
        [InlineData("/rwsingle/double/0", (double)1.0, typeof(double))]
        [InlineData("/rwsingle/long/0",   (long)1,     typeof(long)  )]
        [InlineData("/rwsingle/ulong/0",  (ulong)1,    typeof(ulong) )]
        public async Task TestPutResponseData(string url, dynamic data, Type type)
        {
            var stringData = JsonSerializer.Serialize(data, type, _options);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, content);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<ModbusRequestData>(json, _options);
            Assert.NotNull(responseData);
            Assert.Equal(0, responseData.Offset);
            Assert.Equal(1, responseData.Number);
        }

        [Theory]
        [InlineData("/rwsingle/string/0",    "1234",             4)]
        [InlineData("/rwsingle/hexstring/0", "1234",             4)]
        [InlineData("/rwsingle/bits/0",      "0010110001001000", 1)]
        public async Task TestPutResponseDataString(string url, string data, int number)
        {
            // Arrange
            var stringData = JsonSerializer.Serialize(data, _options);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, content);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<ModbusRequestData>(json, _options);
            Assert.NotNull(responseData);
            Assert.Equal(0, responseData.Offset);
            Assert.Equal(number, responseData.Number);
        }

        [Theory]
        [InlineData("/coils/0",            new bool[]   { true }, typeof(bool)  )]
        [InlineData("/holdingregisters/0", new ushort[] { 1 },    typeof(ushort))]
        [InlineData("/rwarray/bool/0",     new bool[]   { true }, typeof(bool)  )]
        [InlineData("/rwarray/bytes/0",    new byte[]   { 1 },    typeof(byte)  )]
        [InlineData("/rwarray/short/0",    new short[]  { 1 },    typeof(short) )]
        [InlineData("/rwarray/ushort/0",   new ushort[] { 1 },    typeof(ushort))]
        [InlineData("/rwarray/int32/0",    new int[]    { 1 },    typeof(int)   )]
        [InlineData("/rwarray/uint32/0",   new uint[]   { 1 },    typeof(uint)  )]
        [InlineData("/rwarray/float/0",    new float[]  { 1.0F }, typeof(float) )]
        [InlineData("/rwarray/double/0",   new double[] { 1.0 },  typeof(double))]
        [InlineData("/rwarray/long/0",     new long[]   { 1 },    typeof(long)  )]
        [InlineData("/rwarray/ulong/0",    new ulong[]  { 1 },    typeof(ulong) )]
        public async Task TestPutResponseDataArray(string url, dynamic data, Type type)
        {
            var valuesType = typeof(List<>).MakeGenericType(type);
            dynamic modbusdata = Activator.CreateInstance(valuesType, data);
            var stringData = JsonSerializer.Serialize(modbusdata, valuesType, _options);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync(url, content);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<ModbusRequestData>(json, _options);
            Assert.NotNull(responseData);
            Assert.Equal(0, responseData.Offset);
            Assert.Equal(1, responseData.Number);
        }

        [Fact]
        public async Task TestSettingsController()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/settings");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var settings = JsonSerializer.Deserialize<TcpClientSettings>(json, _options);

            Assert.False(settings.TcpMaster.ExclusiveAddressUse);
            Assert.Equal(1000, settings.TcpMaster.ReceiveTimeout);
            Assert.Equal(1000, settings.TcpMaster.SendTimeout);
            Assert.Equal("10.0.1.77", settings.TcpSlave.Address);
            Assert.Equal(502, settings.TcpSlave.Port);
            Assert.Equal(1, (int)settings.TcpSlave.ID);
        }
    }
}
