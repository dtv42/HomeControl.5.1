# UtilityLib

The *UtilityLib* library provides a set of helper classes to facilitate the implementation of applications by providing standardized implementations in various contexts.
An application class is derived from the corresponding base class using the default implementation (constructor, properties, methods).
Other helper classes are for default exit codes, data status, property validation, json converters, and custom attributes.

The following interfaces and classes are provided:
##

##### Interfaces
- IGateway
- IHttpClientSettings
- IPingSettings
- ITimedServiceSettings
- IUdpClientSettings

##### Base Classes
- BaseClass
- BaseCommand
- BaseController
- BaseGateway
- BaseProgram

##### Helper Classes
- CommandLineHost
- TimedService
- ValidatedCommand
- NumberConverter
- TimeSpanConverter
- SpecialDoubleConverter
- UIHealthReport
- UIResponseWriter

##### Status and Health Checks 
- DataStatus
- ExitCodes
- StatusCheck
- PingCheck

##### Extensions
- JsonExtensions
- PropertyExtensions
- ValidationExtensions
- WebHostBuilderExtensions

##### Settings
- HttpClientSettings
- PingSettings
- TimedServiceSettings
- UdpClientSettings

##### Attributes
- AbsoluteUriAttribute
- IPAddressAttribute
- IPEndPointAttribute
- OptionValidationAttribute

## Base Classes

### BaseClass
The *BaseClass* class provides a standard logger data member and an optional application settings member using dependency injection.

~~~CSharp
protected readonly ILogger<BaseClass> _logger;
~~~
and
~~~CSharp
protected readonly ILogger<BaseClass<AppSettings>> _logger;
protected readonly AppSettings _settings;
~~~

### BaseCommand
The *BaseCommand* class for console application commands provides logger, settings, and environment data members.
Standard **--help**, **--version** and **loglevel** options simplify a command implementation allowing synchronous or async execution:

~~~CSharp
int OnExecute()
~~~
and
~~~CSharp
async Task<int> OnExecuteAsync(CancellationToken cancellationToken)~~~
~~~

The *loglevel* command provides mapping of various loglevels (Microsoft.Extensions.Logging, Serilog):

- trace, verbose
- debug
- info, information
- warning
- error
- fatal, critical

The *BaseCommand* class is derived from the **ValidatedCommand** class which provides a custom validation routine:

~~~CSharp
public virtual bool CheckOptions()
~~~

The validation integration is done using the **OptionValidationAttribute** and the **ValidationExtension** class.

### BaseController
The *BaseController* class for an API controller provides logger and settings data members.
Some additional methods returning *StatusCodeResult* and *ObjectResult* are available: 

|    Method Name    |  Status Code   |
|:------------------|:--------------:|
| MethodNotAllowed  |      405       |
| NotAcceptable     |      406       |
| RequestTimeout    |      408       |
| Gone              |      410       |
| ExpectationFailed |      417       |
| ImaTeapot         |      418       |
| Locked            |      423       |
| NotImplemented    |      501       |
| BadGateway        |      502       |
| Unavailable       |      503       |
| GatewayTimeout    |      504       |

### BaseGateway
The *BaseGateway* class implements some common features of a gateway.

~~~CSharp
        public bool IsStartupOk { get; }
        public bool IsLocked { get; }

        public DataStatus Status { get; set; }

        public bool Startup();
        public bool CheckAccess();
        public void Lock();
        public async Task LockAsync();
        public void Unlock();
~~~

Locking primitives are implemented using a *SemaphoreSlim*.
Derived classes should override the **Startup()**, **CheckAccess()** methods, and the **IsStartupOk** property.
Note that the timestamp of the *Status* property is automatically updated.

### BaseProgram
The *BaseProgram* class provides a standardized main routine, and host setup using the *CommandLineHost* helper class and adds singleton service for the application settings using an JSON *appsettings.json* configuration file.
The static JSON serilizer options are initialized adding default converters.
The *CultureInfo* is set to "en-US" and a *Stopwatch* timer is used to display elapsed time.

~~~CSharp
public static async Task<int> RunCommandLineApplicationAsync(IHostBuilder builder, string[] args)
public static IHostBuilder CreateDefaultBuilder()
~~~

## Helper Classes

### CommandLineHost
The *CommandLineHost* class provides a standard implementation for a console application default host builder.
This is similar to the standard generic host setup, but adds Serilog support.

### TimedService
Helper classes providing a basic background service for repetitive execution.
Three virtual methods are available for overriding in a derived class:

~~~CSharp
protected virtual async Task DoStart()
protected virtual async Task DoWork()
protected virtual async Task DoStop()
~~~

### ValidatedCommand
This helper class is used by the *BaseCommand* class.

### NumberConverter
This converter is used to convert strings containing numbers.

### TimeSpanConverter
This converter is used to convert TimeSpans (note: should use ISO8601).

### SpecialDoubleConverter
This converter allows special values (NaN, Infinity, -Infinity).

### UIHealthReport
Helper class to provide a custom data type for health report output.
See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks.

### UIResponseWriter
Helper class to provide JSON formatted health reports.
See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks.

## Status and HealthChecks

### DataStatus
Class to provide OPC UA compatible result codes.

~~~CSharp
public long Timestamp { get; set; }
public uint Code { get; set; }
public string Name { get; set; }
public string Explanation { get; set; }

public bool IsGood { get; }
public bool IsNotGood { get; }
public bool IsUncertain { get; }
public bool IsNotUncertain { get; }
public bool IsBad { get; }
public bool IsNotBad { get; }
~~~

### ExitCodes
The *ExitCodes* class provides standard exit codes for a console application.

|     Exit Code Name    | Code |
|:----------------------|:----:|
| SuccessfullyCompleted |   0  |
| IncorrectFunction     |  -1  |
| FileNotFound          |   2  |
| PathNotFound          |   3  |
| CantOpenFile          |   4  |
| AccessDenied          |   5  |
| InvalidData           |  13  |
| ProgramNotRecognized  | 9009 |

##### On Windows OS
|     Exit Code Name    |    Code    |
|:----------------------|:----------:|
| OperationCanceled     | 0xC000013A |
| UnhandledException    | 0xE0434F4D |

##### On Non Windows OS
|     Exit Code Name    |      Code     |
|:----------------------|:-------------:|
| OperationCanceled     | 130 - SIGINT  |
| UnhandledException    | 134 - SIGABRT |

The last two exit codes are OS platform dependent.

### StatusCheck
A *HealthCheck* implementation interpreting the DataStatus property of a gateway class.

### PingCheck
A *HealthCheck* to determine whether a remote computer is accessible over the network.

## Extensions

### JsonExtensions
The *JsonExtensions* class provides helper extension methods dealing with JSON serializer options.

~~~CSharp
public static JsonSerializerOptions DefaultSerializerOptions
public static void AddDefaultOptions(this JsonSerializerOptions options)
~~~

### PropertyExtensions
Extension class to provide property helper methods for an object.

~~~CSharp
public static bool IsProperty(this Type type, string name)
public static object? GetPropertyValue(this object obj, string name)
public static void SetPropertyValue(this object obj, string name, object value)
~~~

### ValidationExtensions
Extension class to provide a validate an object based on its DataAnnotations, possibly throwing an exception if the object is not valid.

~~~CSharp
public static ICollection<ValidationResult> Validate<T>(this T obj)
public static T ValidateAndThrow<T>(this T obj)
~~~

### WebHostBuilder Extensions
The *WebHostBuilderExtensions* class extends the standard implementation for a web application
providing **Serilog** logger support, and an *AppSettings* singleton.
~~~CSharp
public static IWebHostBuilder ConfigureBaseHost<TSettings>(this IWebHostBuilder builder)
    where TSettings : class, new()
~~~

## Settings

### HttpClientSettings
The *HttpClientSettings* are used to configure *HttpClients*.

~~~CSharp
    public class HttpClientSettings : IHttpClientSettings
    {
        [AbsoluteUri]
        public string BaseAddress { get; set; } = "http://localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;
    }
~~~

### PingSettings
The *PingSettings* are used to configure the execution parameter of the *Ping* health check.
The **Host** property is used to define the remote host for the Ping.
The **Roundtrip** property is used to limit the allowed Ping roundtrip time (degrading) to the specified number of milliseconds.
The **DontFragment** and **Ttl** are ping options used to control how Ping data packets are transmitted.

~~~CSharp
    public class PingSettings : IPingSettings
    {
        public string Host { get; set; } = "localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;

        public bool DontFragment { get; set; }

        [Range(1, Int32.MaxValue)]
        public int Ttl { get; set; } = 128;

        [Range(1, Int32.MaxValue)]
        public int Roundtrip { get; set; } = 100;
    }
~~~

### TimedServiceSettings
The *TimedServiceSettings* are used to configure the execution parameter of the *TimedService*.
The **Delay** property is used to delay the *DoStart()* method by the specified number of milliseconds.
The **Period** property is the time between executions of the *DoWork()* method.

~~~CSharp
public class TimedServiceSettings
{
    [Range(0, Int32.MaxValue)]
    public int Delay { get; set; }

    [Range(0, Int32.MaxValue)]
    public int Period { get; set; } = 1000;
}
~~~

### UdpClientSettings
The *UdpClientSettings* are used to configure *UdpClients*.

~~~CSharp
    public class UdpClientSettings : IUdpClientSettings
    {
        [IPEndPoint]
        public string EndPoint { get; set; } = string.Empty;

        [Range(0, 65535)]
        public int Port { get; set; }

        [Range(0, double.MaxValue)]
        public double Timeout { get; set; } = 1.0;
    }
~~~

## Attributes

- IPAddress: Validates using *IPAddress.TryParse()*.
- IPEndPoint: Validates using *IPEndPoint.TryParse()*.
- AbsoluteUri: Validates using *Uri.TryCreate()*.
- OptionValidation: Validates using *CheckOptions()*.

