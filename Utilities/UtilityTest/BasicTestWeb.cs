// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTestWeb.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 18:31</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityTest
{
    #region Using Directives

    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;

    using Xunit;

    using UtilityWeb.Models;
    using UtilityLib;

    #endregion Using Directives

    public class BasicTestWeb : IClassFixture<WebApplicationFactory<UtilityWeb.Startup>>
    {
        private readonly WebApplicationFactory<UtilityWeb.Startup> _factory;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions();

        public BasicTestWeb(WebApplicationFactory<UtilityWeb.Startup> factory)
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
        [InlineData("/health")]
        [InlineData("/health?access=true")]
        [InlineData("/health?timeout=10")]
        [InlineData("/health?access=true&timeout=10")]
        public async Task TestHealthEndpoints(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<UIHealthReport>(json, _options);
            Assert.NotNull(data);
            Assert.Equal(UIHealthStatus.Healthy, data.Status);
        }

        [Theory]
        [InlineData("/settings")]
        [InlineData("/settings/string")]
        [InlineData("/settings/boolean")]
        [InlineData("/settings/integer")]
        [InlineData("/settings/long")]
        [InlineData("/settings/float")]
        [InlineData("/settings/double")]
        [InlineData("/settings/decimal")]
        [InlineData("/settings/datetime")]
        [InlineData("/settings/datetimeoffset")]
        [InlineData("/settings/stringlist")]
        [InlineData("/settings/stringlist/0")]
        [InlineData("/settings/stringlist/1")]
        [InlineData("/settings/stringlist/2")]
        [InlineData("/settings/booleanlist")]
        [InlineData("/settings/booleanlist/0")]
        [InlineData("/settings/booleanlist/1")]
        [InlineData("/settings/booleanlist/2")]
        [InlineData("/settings/integerlist")]
        [InlineData("/settings/integerlist/0")]
        [InlineData("/settings/integerlist/1")]
        [InlineData("/settings/integerlist/2")]
        [InlineData("/settings/longlist")]
        [InlineData("/settings/longlist/0")]
        [InlineData("/settings/longlist/1")]
        [InlineData("/settings/longlist/2")]
        [InlineData("/settings/floatlist")]
        [InlineData("/settings/floatlist/0")]
        [InlineData("/settings/floatlist/1")]
        [InlineData("/settings/floatlist/2")]
        [InlineData("/settings/doublelist")]
        [InlineData("/settings/doublelist/0")]
        [InlineData("/settings/doublelist/1")]
        [InlineData("/settings/doublelist/2")]
        [InlineData("/settings/decimallist")]
        [InlineData("/settings/decimallist/0")]
        [InlineData("/settings/decimallist/1")]
        [InlineData("/settings/decimallist/2")]
        [InlineData("/settings/datetimelist")]
        [InlineData("/settings/datetimelist/0")]
        [InlineData("/settings/datetimelist/1")]
        [InlineData("/settings/datetimelist/2")]
        [InlineData("/settings/datetimeoffsetlist")]
        [InlineData("/settings/datetimeoffsetlist/0")]
        [InlineData("/settings/datetimeoffsetlist/1")]
        [InlineData("/settings/datetimeoffsetlist/2")]
        [InlineData("/settings/dictionary")]
        [InlineData("/settings/dictionary/0")]
        [InlineData("/settings/dictionary/1")]
        [InlineData("/settings/dictionary/2")]
        [InlineData("/settings/settings")]
        [InlineData("/testdata")]
        [InlineData("/testdata/value")]
        [InlineData("/testdata/name")]
        [InlineData("/testdata/address")]
        [InlineData("/testdata/endpoint")]
        [InlineData("/testdata/uri")]
        [InlineData("/testdata/code")]
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
            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            Assert.Equal("a string", settings.StringValue);
            Assert.True(settings.BooleanValue);
            Assert.Equal(1, settings.IntegerValue);
            Assert.Equal(1000000, settings.LongValue);
            Assert.Equal(1.234, settings.FloatValue, 3);
            Assert.Equal(1.23456789, settings.DoubleValue, 9);
            Assert.Equal("1234567890.123456789", settings.DecimalValue.ToString());
            Assert.Equal("16-Apr-20 15:00:00", settings.DateTimeValue.ToString());
            Assert.Equal("16-Apr-20 15:00:00 +02:00", settings.DateTimeOffsetValue.ToString());
            Assert.NotEmpty(settings.StringList);
            Assert.NotEmpty(settings.BooleanList);
            Assert.NotEmpty(settings.IntegerList);
            Assert.NotEmpty(settings.LongList);
            Assert.NotEmpty(settings.FloatList);
            Assert.NotEmpty(settings.DoubleList);
            Assert.NotEmpty(settings.DecimalList);
            Assert.NotEmpty(settings.DateTimeList);
            Assert.NotEmpty(settings.DateTimeOffsetList);
            Assert.Equal(3, settings.StringList.Count);
            Assert.Equal(3, settings.BooleanList.Count);
            Assert.Equal(3, settings.IntegerList.Count);
            Assert.Equal(3, settings.LongList.Count);
            Assert.Equal(3, settings.FloatList.Count);
            Assert.Equal(3, settings.DoubleList.Count);
            Assert.Equal(3, settings.DecimalList.Count);
            Assert.Equal(3, settings.DateTimeList.Count);
            Assert.Equal(3, settings.DateTimeOffsetList.Count);
            Assert.Equal("item1", settings.StringList[0]);
            Assert.Equal("item2", settings.StringList[1]);
            Assert.Equal("item3", settings.StringList[2]);
            Assert.True(settings.BooleanList[0]);
            Assert.False(settings.BooleanList[1]);
            Assert.True(settings.BooleanList[2]);
            Assert.Equal(1, settings.IntegerList[0]);
            Assert.Equal(2, settings.IntegerList[1]);
            Assert.Equal(3, settings.IntegerList[2]);
            Assert.Equal(1000000, settings.LongList[0]);
            Assert.Equal(2000000, settings.LongList[1]);
            Assert.Equal(3000000, settings.LongList[2]);
            Assert.Equal(1.2, settings.FloatList[0], 1);
            Assert.Equal(3.4, settings.FloatList[1], 1);
            Assert.Equal(5.6, settings.FloatList[2], 1);
            Assert.Equal(1.23456789, settings.DoubleList[0], 8);
            Assert.Equal(3.45678912, settings.DoubleList[1], 8);
            Assert.Equal(5.67891234, settings.DoubleList[2], 8);
            Assert.Equal("1234567890.123456789", settings.DecimalList[0].ToString());
            Assert.Equal("1234567890.123456789", settings.DecimalList[1].ToString());
            Assert.Equal("1234567890.123456789", settings.DecimalList[2].ToString());
            Assert.Equal("16-Apr-20 15:00:00", settings.DateTimeList[0].ToString());
            Assert.Equal("16-Apr-20 16:00:00", settings.DateTimeList[1].ToString());
            Assert.Equal("16-Apr-20 17:00:00", settings.DateTimeList[2].ToString());
            Assert.Equal("16-Apr-20 15:00:00 +02:00", settings.DateTimeOffsetList[0].ToString());
            Assert.Equal("16-Apr-20 16:00:00 +02:00", settings.DateTimeOffsetList[1].ToString());
            Assert.Equal("16-Apr-20 17:00:00 +02:00", settings.DateTimeOffsetList[2].ToString());
            Assert.NotEmpty(settings.Dictionary);
            Assert.Equal(3, settings.Dictionary.Count);
            Assert.True(settings.Dictionary.ContainsKey("key1"));
            Assert.True(settings.Dictionary.ContainsKey("key2"));
            Assert.True(settings.Dictionary.ContainsKey("key3"));
            Assert.Equal("value1", settings.Dictionary["key1"]);
            Assert.Equal("value2", settings.Dictionary["key2"]);
            Assert.Equal("value3", settings.Dictionary["key3"]);
            Assert.Equal("a string", settings.Settings.StringValue);
            Assert.NotNull(settings.Settings);
            Assert.True(settings.Settings.BooleanValue);
            Assert.Equal(1, settings.Settings.IntegerValue);
            Assert.Equal(1000000, settings.Settings.LongValue);
            Assert.Equal(1.234, settings.Settings.FloatValue, 3);
            Assert.Equal(1.23456789, settings.Settings.DoubleValue, 9);
            Assert.Equal("1234567890.123456789", settings.Settings.DecimalValue.ToString());
        }

        [Fact]
        public async Task TestTestDataController()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/testdata");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var testdata = JsonSerializer.Deserialize<TestData>(json);
            Assert.Equal(43, testdata.Value);
            Assert.Equal("NewData", testdata.Name);
            Assert.Equal("6DCEB058-526F-4413-8FCE-B60257A496A2", testdata.Uuid.ToString().ToUpper());
            Assert.Equal("10.0.1.77", testdata.Address);
            Assert.Equal("10.0.1.100:8080", testdata.Endpoint);
            Assert.Equal("https://10.0.1.138:88", testdata.Uri);
            Assert.Equal("Accepted", testdata.Code.ToString());
        }

        [Theory]
        [InlineData("/testdata/value",    43,                                     typeof(int)           )]
        [InlineData("/testdata/name",     "NewData",                              typeof(string)        )]
        [InlineData("/testdata/uuid",     "6dceb058-526f-4413-8fce-b60257a496a2", typeof(Guid)          )]
        [InlineData("/testdata/address",  "10.0.1.77",                            typeof(string)        )]
        [InlineData("/testdata/endpoint", "10.0.1.100:8080",                      typeof(string)        )]
        [InlineData("/testdata/uri",      "https://10.0.1.138:88",                typeof(Uri)           )]
        [InlineData("/testdata/code",     HttpStatusCode.Accepted,                typeof(HttpStatusCode))]
        public async Task TestTestDataControllerGet(string url, dynamic data, Type type)
        {
            // Arrange
            if (type == typeof(Guid)) data = new Guid(data);
            if (type == typeof(Uri)) data = new Uri(data);
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonSerializer.Deserialize(json, type, _options);
            Assert.Equal(data, responseData);
        }

        [Theory]
        [InlineData("/testdata/value",    43,                                     typeof(int)           )]
        [InlineData("/testdata/name",     "NewData",                              typeof(string)        )]
        [InlineData("/testdata/uuid",     "6dceb058-526f-4413-8fce-b60257a496a2", typeof(Guid)          )]
        [InlineData("/testdata/address",  "10.0.1.77",                            typeof(string)        )]
        [InlineData("/testdata/endpoint", "10.0.1.100:8080",                      typeof(string)        )]
        [InlineData("/testdata/uri",      "https://10.0.1.138:88",                typeof(Uri)           )]
        [InlineData("/testdata/code",     HttpStatusCode.Accepted,                typeof(HttpStatusCode))]
        public async Task TestTestDataControllerPost(string url, dynamic data, Type type)
        {
            // Arrange
            if (type == typeof(Guid)) data = new Guid(data);
            if (type == typeof(Uri)) data = new Uri(data);
            var stringData = JsonSerializer.Serialize(data, type, _options);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Theory]
        [InlineData("/testdata/value",    100,                     typeof(int)   )]
        [InlineData("/testdata/name",     "This is a long string", typeof(string))]
        [InlineData("/testdata/address",  "10.0.1.77.100",         typeof(string))]
        [InlineData("/testdata/endpoint", "10.0.1.100:8080808080", typeof(string))]
        public async Task TestTestDataControllerPostValidation(string url, dynamic data, Type type)
        {
            // Arrange
            var stringData = JsonSerializer.Serialize(data, type, _options);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}