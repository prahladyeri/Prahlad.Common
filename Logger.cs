/**
 * Logger.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.IO;
using System.Reflection;

namespace Prahlad.Common
{
    public static class Logger
    {
        public static readonly string BaseName =
            Assembly.GetExecutingAssembly().GetName().Name;
        private const long MaxSize = 2 * 1024 * 1024; // 2 MB
        private const int MaxBackups = 5; 
        private static readonly object _lock = new object();


        public static string GetPath(string context = "core")
        {
            return Path.Combine(AppContext.BaseDirectory, $"{BaseName}_{context}_log.txt");
        }

        public static void WriteLog(string message, string context = "core")
        {
            lock (_lock)
            {
                try
                {
                    string path = GetPath(context);
                    using (StreamWriter writer = new StreamWriter(path, true, System.Text.Encoding.UTF8, bufferSize: 4096))
                    {
                        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{context}] {message}");
                    }
                    RotateIfNeeded(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Logging failed for context '{context}': {ex.Message}");
                }
            }
        }

        private static void RotateIfNeeded(string path)
        {
            if (!File.Exists(path)) return;
            FileInfo fi = new FileInfo(path);
            if (fi.Length < MaxSize) return;

            try
            {
                string oldest = path.Replace(".txt", $".bak.{MaxBackups}.txt");
                if (File.Exists(oldest)) File.Delete(oldest);

                for (int i = MaxBackups - 1; i >= 1; i--)
                {
                    string src = path.Replace(".txt", $".bak.{i}.txt");
                    string dst = path.Replace(".txt", $".bak.{i + 1}.txt");
                    if (File.Exists(src)) File.Move(src, dst);
                }

                string bak1 = path.Replace(".txt", ".bak.1.txt");
                File.Move(path, bak1);
            }
            catch (Exception ex)
            {
                string emergency = path.Replace(".txt", ".emergency.txt");
                File.AppendAllText(emergency, $"{DateTime.Now:u} [LOGGER] Rotation failed: {ex}\n");
            }
        }
    }

}
