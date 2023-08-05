// See https://aka.ms/new-console-template for more information


using KubectlConsoleApp.Models;
using Spectre.Console;

Console.WriteLine("Hello, World!");

var strCommand = $@"CMD.exe";
List<Container> GetContainers()
{
    var strCommandParameters = $@"/C kubectl get pods";

    //Create process
    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
    //strCommand is path and file name of command to run
    pProcess.StartInfo.FileName = strCommand;
    //strCommandParameters are parameters to pass to program
    pProcess.StartInfo.Arguments = strCommandParameters;
    pProcess.StartInfo.UseShellExecute = false;
    //Set output of program to be written to process output stream
    pProcess.StartInfo.RedirectStandardOutput = true;
    //Optional
    //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
    //Start the process
    pProcess.Start();

    //Get program output
    string strOutput = pProcess.StandardOutput.ReadToEnd();
    var lsContainersOuputStream = strOutput.Split("\n").ToList();
    var lsContainers = new List<Container>();
    var table = new Table();
    if (lsContainersOuputStream.Count > 0)
    {
        table.AddColumn($"ID");
        table.AddColumn(lsContainersOuputStream[0]);
        lsContainersOuputStream.RemoveAt(0);
    }
    var id = 1;

    foreach (var container in lsContainersOuputStream)
    {
        if (!string.IsNullOrEmpty(container))
        {
            var item = new Container()
            {
                Id = id,
                ContainerDescription = container,
                ContainerName = container.Split(" ")[0],
            };
            lsContainers.Add(item);
            table.AddRow(id.ToString(), container);
            id++;
        }
    }
    AnsiConsole.Write(table);
    //Wait for process to finish
    pProcess.WaitForExit();
    return lsContainers;
}

void RunForwardContainer(Container item)
{
    var strCommandParameters = $@"/C kubectl port-forward {item.ContainerName} 5061:22";

    //Create process
    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
    //strCommand is path and file name of command to run
    pProcess.StartInfo.FileName = strCommand;
    //strCommandParameters are parameters to pass to program
    pProcess.StartInfo.Arguments = strCommandParameters;
    pProcess.StartInfo.UseShellExecute = false;
    //Set output of program to be written to process output stream
    pProcess.StartInfo.RedirectStandardOutput = true;
    //Optional
    //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
    //Start the process
    pProcess.Start();
    Console.WriteLine($"begin start command: {strCommandParameters}");
    //Get program output
    //string strOutput = pProcess.StandardOutput.ReadToEnd();
    string strOutput= pProcess.StandardOutput.ReadLine();
    while (strOutput != null)
    {
        Console.WriteLine(strOutput);
        strOutput = pProcess.StandardOutput.ReadLine();
    }
    //Wait for process to finish
    pProcess.WaitForExit();
}

var containers = GetContainers();
Console.WriteLine("Choose number container do you want to forward: ");
var input = Console.ReadLine();
var index = input != null ? (int.Parse(input) - 1) : -1;
var container= containers[index];
RunForwardContainer(container);
