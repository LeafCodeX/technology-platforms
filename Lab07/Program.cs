using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab07
{
    internal static class DirectoryExtensions
    {
        public static Tuple<string, DateTime> GetOldestFile(this DirectoryInfo directory)
        {
            var oldestDate = DateTime.MaxValue;
            var oldestFile = "";

            foreach (var file in directory.GetFiles())
            {
                if (file.LastWriteTime < oldestDate)
                {
                    oldestDate = file.LastWriteTime;
                    oldestFile = file.Name;
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                var subOldestFile = subDirectory.GetOldestFile();
                if (subOldestFile.Item2 < oldestDate)
                {
                    oldestDate = subOldestFile.Item2;
                    oldestFile = subOldestFile.Item1;
                }
            }

            return Tuple.Create(oldestFile, oldestDate);
        }

        public static Tuple<string, DateTime> GetNewestFile(this DirectoryInfo directory)
        {
            var newestDate = DateTime.MinValue;
            var newestFile = "";

            foreach (var file in directory.GetFiles())
            {
                if (file.LastWriteTime > newestDate)
                {
                    newestDate = file.LastWriteTime;
                    newestFile = file.Name;
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                var subNewestFile = subDirectory.GetNewestFile();
                if (subNewestFile.Item2 > newestDate)
                {
                    newestDate = subNewestFile.Item2;
                    newestFile = subNewestFile.Item1;
                }
            }

            return Tuple.Create(newestFile, newestDate);
        }
    }
    
    
    

    internal static class FileSystemInfoExtensions
    {
        public static string GetDosAttributes(this FileSystemInfo fileSystemInfo)
        {
            string dosAttributes = "";
            dosAttributes += (fileSystemInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? "r" : "-";
            dosAttributes += (fileSystemInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive ? "a" : "-";
            dosAttributes += (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ? "h" : "-";
            dosAttributes += (fileSystemInfo.Attributes & FileAttributes.System) == FileAttributes.System ? "s" : "-";

            return dosAttributes;
        }
    }

    internal class Program
    {
        private static void PrintDirectory(DirectoryInfo directory, string indent = "")
        {
            const string indentationLevel = "    ";

            var subDirs = directory.EnumerateDirectories()
                .ToArray();
            var subFiles = directory.EnumerateFiles()
                .ToArray();

            Console.WriteLine($"{indent}{directory.Name} ({subDirs.Length + subFiles.Length} elementow) ----");

            foreach (var dir in subDirs)
            {
                PrintDirectory(dir, indent + indentationLevel);
            }

            foreach (var file in subFiles)
            {
                Console.WriteLine($"{indent}{indentationLevel}{file.Name} {file.Length} bajtow {FileSystemInfoExtensions.GetDosAttributes(file)}");
            }
        }

        public static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Console.Error.WriteLine("Usage: Lab07 <directory> - example: Lab07 C:\\Users\\marci\\OneDrive\\Pulpit\\PT_LAB7\\Lab07\\test");
            //    return;
            //}

            //var directory = new DirectoryInfo(Path.GetFullPath(args[0]));
            //PrintDirectory(directory);
            
            var directoryPath = "/Users/marcin/Downloads/Lab07";
            var directory = new DirectoryInfo(directoryPath);
            PrintDirectory(directory);
            
            Console.WriteLine();

            var oldestDate = directory.GetOldestFile();
            Console.WriteLine($"Najstarszy plik: {oldestDate}");
            var newestDate = directory.GetNewestFile();
            Console.WriteLine($"Najmłodszy plik: {newestDate}");
            Console.WriteLine();

            var a = directory.EnumerateFiles().Select(x => (x.Name, x.Length));
            var b = directory.EnumerateDirectories().Select(x => (x.Name, x.EnumerateDirectories().Count() + x.EnumerateFiles().LongCount()));
            var collection = a.Concat(b)
                .OrderBy(x => x.Name.Length)
                .ThenBy(x => x.Name)
                .ToDictionary(x => x.Name, x => x.Item2);

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, collection);

                ms.Position = 0;

                var tmp = formatter.Deserialize(ms) as Dictionary<string, long>;
                foreach (var pair in tmp)
                {
                    Console.WriteLine($"{pair.Key} -> {pair.Value}");
                }
            }
        }
    }
}
