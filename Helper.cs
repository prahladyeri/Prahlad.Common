/**
 * Helper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Reflection;

namespace Prahlad.Common
{
    public static class Helper
    {
        public static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;

        public static string GetVersion()
        {
            var v = Assembly.GetEntryAssembly().GetName().Version;
            return $"v{v.Major}.{v.Minor}";
        }
    }
}
