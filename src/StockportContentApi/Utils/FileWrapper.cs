﻿namespace StockportContentApi.Utils;

public interface IFileWrapper
{
    bool Exists(string path);
    string[] ReadAllLines(string path);
}

[ExcludeFromCodeCoverage]
public class FileWrapper : IFileWrapper
{
    public bool Exists(string path)
    {
        return System.IO.File.Exists(path);
    }

    public string[] ReadAllLines(string path)
    {
        return System.IO.File.ReadAllLines(path);
    }
}