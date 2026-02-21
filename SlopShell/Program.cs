// See https://aka.ms/new-console-template for more information

using System;
using System.Drawing;
using System.IO;
using Pastel;

string currentPath = Environment.GetEnvironmentVariable("USERPROFILE") ?? "C:";
Dictionary<string, Func<string, string[], string>> commands = new Dictionary<string, Func<string, string[], string>>();

commands.Add("ls", LsCommand);
commands.Add("cd", CdCommand);
commands.Add("exit", ExitCommand);
commands.Add("cat", CatCommand);
commands.Add("rm", RmCommand);
commands.Add("mk", MkCommand);
commands.Add("mkdir", MkDirCommand);
commands.Add("append", AppendCommand);

Console.WriteLine(
    "____ _    ____ ___  ____ _  _".Pastel("9d9617").PastelBg("4A412A") + " Doing what\n" +
    "[__  |    |  | |__] [__  |__|".Pastel("9d9617").PastelBg("4A412A") + " Catamapp\n" +
    "___] |___ |__| |    ___] |  |".Pastel("9d9617").PastelBg("4A412A") + " Couldn't"
);

while (true)
{
    string input = GetInput(currentPath + "% ".Pastel(ConsoleColor.Red));
    string[] words = input.Split(' ');
    //Func<string, string> func = commands[words[0]];
    if (commands.TryGetValue(words[0], out var func))
    {
        Console.Write(func.Invoke(currentPath, words.Skip(1).ToArray()));
    }
    else
    {
        Console.Write("Invalid command");
    }
}

bool ChangeCurrentPath(string path)
{
    if (Directory.Exists(path))
    {
        currentPath = path;
        return true;
    }

    return false;
}

string AppendCommand(string path, string[] args)
{
    string filePath = GetFilePath(path, string.Join(' ', args));
    if (filePath == "")
    {
        return "Invalid file";
    }

    File.AppendText(string.Join(' ', args));
    return ("Appended text to " + path + '\\' + args.Last()).Pastel(ConsoleColor.Green);
}

string MkDirCommand(string path, string[] args)
{
    string cdPath = GetDirectoryPath(path, string.Join(' ', args[..(args.Length - 1)]));
    if (cdPath == "")
    {
        cdPath = path;
    }

    Directory.CreateDirectory(path + '\\' + args.Last());
    //Directory.Create(path + '\\' + args.Last()).Dispose();
    return ("Created directory " + path + '\\' + args.Last()).Pastel(ConsoleColor.Green);
}

string MkCommand(string path, string[] args)
{
    string cdPath = GetDirectoryPath(path, string.Join(' ', args[..(args.Length - 1)]));
    if (cdPath == "")
    {
        cdPath = path;
    }

    File.Create(path + '\\' + args.Last()).Dispose();
    return ("Created file " + path + '\\' + args.Last()).Pastel(ConsoleColor.Green);
}

string RmCommand(string path, string[] args)
{
    string filePath = GetFilePath(path, string.Join(' ', args));
    if (filePath == "")
    {
        return "Invalid file";
    }

    File.Delete(filePath);
    return ("Deleted " + filePath + " ").Pastel(ConsoleColor.Green) +
           "FOREVER!".Pastel(Color.Yellow).PastelBg(Color.Red);
}

string ExitCommand(string path, string[] args)
{
    System.Environment.Exit(1);
    return "Stop yelling at me, Compiler";
}

string CatCommand(string path, string[] args)
{
    string filePath = GetFilePath(path, string.Join(' ', args));
    if (filePath == "")
    {
        return "Invalid file";
    }

    return string.Join('\n', File.ReadAllLines(filePath));
}

string CdCommand(string path, string[] args)
{
    string cdPath = GetDirectoryPath(path, string.Join(' ', args));
    if (cdPath == "")
    {
        return "Invalid directory";
    }

    ChangeCurrentPath(cdPath);
    return "OK";
}

string GetDirectoryPath(string path, string added)
{
    string cdPath = new string("");
    if (added != ".." && Directory.Exists(path + '\\' + added)) cdPath = path + '\\' + added;
    else if (added != ".." && Directory.Exists(added)) cdPath = added;
    else if (added == "..")
    {
        //string[] splitPath = path.Split('\\');
        //cdPath = string.Join('\\', splitPath[..(splitPath.Length - 1)]);
        var parent = Directory.GetParent(path);
        if (parent == null) cdPath = "";
        else cdPath = parent.ToString();
    }

    return cdPath;
}

string GetFilePath(string path, string added)
{
    string cdPath = new string("");
    if (File.Exists(path + '\\' + added))
    {
        cdPath = path + '\\' + added;
    }
    else if (File.Exists(added))
    {
        cdPath = added;
    }

    return cdPath;
}


string[] LsFiles(string path)
{
    return Directory.GetFiles(path);
}

string[] LsDirectories(string path)
{
    return Directory.GetDirectories(path);
}

string LsCommand(string path, string[] args)
{
    string output = new string("");
    foreach (var x in LsDirectories(path))
    {
        output += (x.Split('\\').Last() + " ").Pastel(ConsoleColor.Blue);
    }

    foreach (var x in LsFiles(path))
    {
        output += Path.GetFileName(x) + " ";
    }

    return output;
}

string GetInput(string prompt, bool doNewLine = true)
{
    if (doNewLine) Console.Write("\n");
    Console.Write(prompt);
    return Console.ReadLine();
}