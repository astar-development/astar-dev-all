using System.Reflection;
using AStar.Dev.Admin.Api;
using AStar.Dev.Technical.Debt.Reporting;

Console.WriteLine("Creating Reports");

// This may work but it is not sustainable / maintainable...
var reportTypes = new List<Type> { typeof (IAssemblyMarker), };
reportTypes.Add(typeof (AStar.Dev.File.Classifications.Api.IAssemblyMarker));
reportTypes.Add(typeof (AStar.Dev.Files.Api.IAssemblyMarker));
reportTypes.Add(typeof (AStar.Dev.Images.Api.IAssemblyMarker));

foreach (Type reportType in reportTypes)
{
    var assemblyContainingTechDebt = Assembly.GetAssembly(reportType);

    if (assemblyContainingTechDebt != null)
    {
        Console.WriteLine(Reporter.GenerateReport(assemblyContainingTechDebt));
    }
}
