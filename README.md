# Rakis.Logging - A simple logging library

This library provides some simple support for logging, with output sent to the console, files, a (reactive) `IObserver`,
or any other custom sink. Configuration is done using a simple text file or a `Configurer`.

## Quickstart

## How to log

### Log messages

Log messages (`Rakis.Logging.LogEntry`) are objects containing:
1. A timestamp,
2. A source, which is typically the name of the logger object,
3. A level, showing how important the entry is, and
4. An actual message.

Levels are the common Log-levels used in most environments, from low to high:
| Level | Description |
| :--- | :--- |
| `TRACE` | Detailed logging to show internal workings. This level is normally used only for detailed debugging of the code using it, because it contains so much information that producing it negatively impacts performance. Typically only the authors of the code would use this level. |
| `DEBUG` | Normal debugging information. This level might be used also by end-users to provide additional information when trying to explain behavior. That said, like `TRACE`, it may affect performance enough that you'd normally leave this off. |
| `INFO` | Informational messages showing important hints about the code's behavior. This is the default level, although it does not provide information essential to the operation of the application. |
| `WARN` | Important messages about unexpected or unwanted situations that the code can easily compensate for, but the user should be aware of. |
| `ERROR` | Things that should not have happened, but the code was able to recover from. |
| `FATAL` | An error that could **not** be recovered from. Normally this would announce an abnormal abort of the application. |

### Loggers

Logging is done using an `Rakis.Logging.ILogger` conforming object, which encapsulates:
* A name, which is a structured name like a fully qualified Class name, like "`Rakis.Logging.Logger`,"
* A sink, which is the object actually writing the logs to whatever has been chosen, and
* A threshold, below which messages are ignored.

You can obtain a logger with the static `GetLogger` method, which you pass a name or a type:

```c#
private static ILogger logger = Logger.GetLogger(typeof(MyClass));
private static ILogger auditLogger = Logger.GetLogger("AUDIT");
```

### Writing log messages

To produce logging output, use one of the "leveled" loggers:
```c#
  logger.Info?.Log("Application starting up");
  logger.Trace?.Log($"Application started at {DateTime.Now}.");
```
The levels will be a null-reference if they fall below the threshold, so none of the actual logging is performed if the output is filtered out. If you have multiple entries you'd like to produce or need some preperatory work, you can check if a level is enabled:
```c#
  if (logger.IsEnabled(LogLevel.DEBUG)) { ... }
```
or
```c#
  if (logger.IsDebugEnabled) {... }
```

## Configuring the loggers

By default, logging will be sent to the console with a threshold of `INFO`.

### Using a configuration file

### Programmatically configuring the logging

You can use configurers 