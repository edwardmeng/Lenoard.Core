using System;
using System.IO;
using System.Security.Permissions;

namespace Lenoard.Core
{
    public static class PathHelper
    {
        private static bool IsAbsolutePhysicalPath(string path)
        {
            return path != null && path.Length >= 3 && (path[1] == ':' && IsDirectorySeparatorChar(path[2]) || IsUncSharePath(path));
        }

        private static bool IsDirectorySeparatorChar(char ch)
        {
            return ch == '\\' || ch == '/';
        }

        private static bool IsUncSharePath(string path)
        {
            return path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]);
        }

        private static bool IsUriPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            var colonIndex = path.IndexOf(":", StringComparison.Ordinal);
            if (colonIndex == -1) return false;
            if (path.Length < colonIndex + 3 || path[colonIndex + 1] != '/' || path[colonIndex + 2] != '/') return false;
            var scheme = path.Substring(0, colonIndex).Trim().ToLower();
            return scheme == "http" || scheme == "https" || scheme == "ftp" || scheme == "file" || scheme == "news";
        }

        private static string GetDataDirectory()
        {
            string directory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                AppDomain.CurrentDomain.SetData("DataDirectory", directory, new FileIOPermission(FileIOPermissionAccess.PathDiscovery, directory));
            }
            return directory;
        }

        private static bool IsAppRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path.Length < 2) return false;
            return path[0] == '~' && IsDirectorySeparatorChar(path[1]);
        }

        private static bool TryReplaceSpecialFolder(ref string virtualPath)
        {
            foreach (string name in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                if (virtualPath.StartsWith("|" + name + "|"))
                {
                    virtualPath =
                        Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), name)),
                            virtualPath.Substring(name.Length + 2));
                    return true;
                }
            }
            if (virtualPath.StartsWith("|DataDirectory|"))
            {
                virtualPath = Path.Combine(GetDataDirectory(), virtualPath.Substring(15));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Maps the specified virtual path to a physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual path (absolute or relative) for the current environment.</param>
        /// <returns>The physical path specified by <paramref name="virtualPath"/>.</returns>
        public static string ResolvePath(string virtualPath)
        {
            if (IsUriPath(virtualPath))
            {
                return virtualPath.StartsWith("file://", StringComparison.OrdinalIgnoreCase) ? virtualPath.Substring(7) : virtualPath;
            }
            if (TryReplaceSpecialFolder(ref virtualPath))
            {
                return virtualPath;
            }
            if (!IsAbsolutePhysicalPath(virtualPath))
            {
                if (IsAppRelativePath(virtualPath))
                {
                    virtualPath = virtualPath.Substring(2);
                }
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
            }
            return virtualPath;
        }
    }
}
