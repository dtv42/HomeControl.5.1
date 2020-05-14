// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTestData.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>18-4-2020 18:25</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityTest
{
    #region Using Directives

    using System.IO;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;

    using Xunit;

    using UtilityLib;
    using UtilityApp.Models;

    #endregion Using Directives

    public class BasicTestData
    {
        private readonly AppSettings _settings;
        private readonly Testdata _testdata;

        public BasicTestData()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("testdata.json").Build();
            services.AddSingleton(configuration.GetSection("AppSettings").Get<AppSettings>().ValidateAndThrow());
            services.AddSingleton(configuration.GetSection("TestData").Get<Testdata>().ValidateAndThrow());
            var provider = services.BuildServiceProvider();
            _settings = provider.GetService<AppSettings>();
            _testdata = provider.GetService<Testdata>();
        }

        [Theory]
        [InlineData("Value")]
        [InlineData("Name")]
        [InlineData("Uuid")]
        [InlineData("Address")]
        [InlineData("Endpoint")]
        [InlineData("Uri")]
        [InlineData("Code")]
        public void TestDataProperties(string property)
        {
            Assert.True(typeof(Testdata).IsProperty(property));
            var info = typeof(Testdata).GetProperty(property);
            Assert.NotNull(info);
            Assert.True(info.CanRead);
            Assert.True(info.CanWrite);
            Assert.Equal(MemberTypes.Property, info.MemberType);
            var value = _testdata.GetPropertyValue(property);
            Assert.NotNull(value);
            _testdata.SetPropertyValue(property, value);
        }

        [Fact]
        public void TestDataInstance()
        {
            Testdata testdata = new Testdata();

            Assert.Equal(42, testdata.Value);
            Assert.Equal("Data", testdata.Name);
            Assert.Equal("00000000-0000-0000-0000-000000000000", testdata.Uuid.ToString());
            Assert.Equal("0.0.0.0", testdata.Address);
            Assert.Equal("0.0.0.0:80", testdata.Endpoint);
            Assert.Equal("http://127.0.0.1:80", testdata.Uri);
            Assert.Equal("OK", testdata.Code.ToString());
        }

        [Fact]
        public void TestDataService()
        {
            Assert.Equal(43, _testdata.Value);
            Assert.Equal("NewData", _testdata.Name);
            Assert.Equal("6DCEB058-526F-4413-8FCE-B60257A496A2", _testdata.Uuid.ToString().ToUpper());
            Assert.Equal("10.0.1.77", _testdata.Address);
            Assert.Equal("10.0.1.100:8080", _testdata.Endpoint);
            Assert.Equal("https://10.0.1.138:88", _testdata.Uri);
            Assert.Equal("Accepted", _testdata.Code.ToString());
        }

        [Theory]
        [InlineData("StringValue")]
        [InlineData("BooleanValue")]
        [InlineData("IntegerValue")]
        [InlineData("LongValue")]
        [InlineData("FloatValue")]
        [InlineData("DoubleValue")]
        [InlineData("DecimalValue")]
        [InlineData("DateTimeValue")]
        [InlineData("DateTimeOffsetValue")]
        [InlineData("StringArray")]
        [InlineData("BooleanArray")]
        [InlineData("IntegerArray")]
        [InlineData("LongArray")]
        [InlineData("FloatArray")]
        [InlineData("DoubleArray")]
        [InlineData("DecimalArray")]
        [InlineData("DateTimeArray")]
        [InlineData("DateTimeOffsetArray")]
        [InlineData("StringList")]
        [InlineData("BooleanList")]
        [InlineData("IntegerList")]
        [InlineData("LongList")]
        [InlineData("FloatList")]
        [InlineData("DoubleList")]
        [InlineData("DecimalList")]
        [InlineData("DateTimeList")]
        [InlineData("DateTimeOffsetList")]
        [InlineData("Dictionary")]
        [InlineData("Settings")]
        public void AppSettingsProperties(string property)
        {
            Assert.True(typeof(AppSettings).IsProperty(property));
            var info = typeof(AppSettings).GetProperty(property);
            Assert.NotNull(info);
            Assert.True(info.CanRead);
            Assert.True(info.CanWrite);
            Assert.Equal(MemberTypes.Property, info.MemberType);
            var value = _settings.GetPropertyValue(property);
            Assert.NotNull(value);
            _settings.SetPropertyValue(property, value);
        }

        [Fact]
        public void AppSettingsService()
        {
            Assert.Equal("a string", _settings.StringValue);
            Assert.True(_settings.BooleanValue);
            Assert.Equal(1, _settings.IntegerValue);
            Assert.Equal(1000000, _settings.LongValue);
            Assert.Equal(1.234, _settings.FloatValue, 3);
            Assert.Equal(1.23456789, _settings.DoubleValue, 9);
            Assert.Equal("1234567890.123456789", _settings.DecimalValue.ToString());
            Assert.Equal("16-Apr-20 15:00:00", _settings.DateTimeValue.ToString());
            Assert.Equal("16-Apr-20 15:00:00 +02:00", _settings.DateTimeOffsetValue.ToString());

            Assert.NotEmpty(_settings.StringArray);
            Assert.NotEmpty(_settings.BooleanArray);
            Assert.NotEmpty(_settings.IntegerArray);
            Assert.NotEmpty(_settings.LongArray);
            Assert.NotEmpty(_settings.FloatArray);
            Assert.NotEmpty(_settings.DoubleArray);
            Assert.NotEmpty(_settings.DecimalArray);
            Assert.NotEmpty(_settings.DateTimeArray);
            Assert.NotEmpty(_settings.DateTimeOffsetArray);

            Assert.NotEmpty(_settings.StringList);
            Assert.NotEmpty(_settings.BooleanList);
            Assert.NotEmpty(_settings.IntegerList);
            Assert.NotEmpty(_settings.LongList);
            Assert.NotEmpty(_settings.FloatList);
            Assert.NotEmpty(_settings.DoubleList);
            Assert.NotEmpty(_settings.DecimalList);
            Assert.NotEmpty(_settings.DateTimeList);
            Assert.NotEmpty(_settings.DateTimeOffsetList);
            Assert.NotEmpty(_settings.Dictionary);

            Assert.Equal("a string", _settings.Settings.StringValue);
            Assert.True(_settings.Settings.BooleanValue);
            Assert.Equal(1, _settings.Settings.IntegerValue);
            Assert.Equal(1000000, _settings.Settings.LongValue);
            Assert.Equal("1.234", _settings.Settings.FloatValue.ToString());
            Assert.Equal("1.23456789", _settings.Settings.DoubleValue.ToString());
            Assert.Equal("1234567890.123456789", _settings.Settings.DecimalValue.ToString());
        }
    }
}