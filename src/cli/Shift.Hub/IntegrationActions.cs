using System.Reflection;

namespace Shift.Hub;

internal class IntegrationActions
{
    private string? _commandName;
    private string? _integrationName;

    private object? _command = null;
    private MethodInfo? _method = null;

    public void Execute(string[] args)
    {
        if (!ValidateArguments(args))
            return;

        if (!LoadIntegration())
            return;

        ExecuteIntegration(args);
    }

    private bool ValidateArguments(string[] args)
    {
        if (args.Length < 2)
        {
            Logger.Instance.Information("Usage: Shift.Hub <integration> <command>");
            return false;
        }

        _integrationName = args[0];
        _commandName = args[1];
        return true;
    }

    private bool LoadIntegration()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var className = $"Arc.Cli.Integration.{_integrationName}.{_commandName}";
        var classType = assembly.GetType(className);
        if (classType == null)
        {
            Logger.Instance.Information($"Class {className} not found in the assembly.");
            return false;
        }

        _command = Activator.CreateInstance(classType);

        const string methodName = "Execute";
        _method = classType.GetMethod(methodName);
        if (_method == null)
        {
            Logger.Instance.Information($"Method {methodName} not found in the class {className}.");
            return false;
        }

        return _command != null && _method != null;
    }

    private void ExecuteIntegration(string[] args)
    {
        Logger.Instance.Information($"{_integrationName}.{_commandName}.Execute ...");

        if (_method == null)
            throw new ArgumentNullException(nameof(_method));

        // Prepare the parameters.
        object[] parameters = args.Length > 2 ? args.Skip(2).ToArray() : [];

        var oldOut = Console.Out;
        var oldError = Console.Error;

        Console.SetOut(new LoggerWriter(oldOut, false));
        Console.SetError(new LoggerWriter(oldOut, true));

        try
        {
            // Invoke the Execute method on the integration.
            _method.Invoke(_command, parameters);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex?.InnerException?.Message ?? ex?.Message);
        }
        finally
        {
            Console.SetOut(oldOut);
            Console.SetError(oldError);
        }
    }
}
