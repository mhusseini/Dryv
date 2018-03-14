using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Dryv.Demo.TagHelpers
{
    public static class HostingEnvironmentAwareViewComponentExtensions
    {
        private static readonly ConcurrentDictionary<string, string> FileContent = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, DateTimeOffset> FileModifications = new ConcurrentDictionary<string, DateTimeOffset>(StringComparer.OrdinalIgnoreCase);

        public static string ReadFile(this IHostingEnvironmentAwareViewComponent vc, string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            var path = vc.GetContentFilePath(file);
            var fileInfo = vc.HostingEnvironment.ContentRootFileProvider.GetFileInfo(path);
            return fileInfo.Exists ? GetFileContent(fileInfo) : null;
        }

        private static string GetContentFilePath(this IHostingEnvironmentAwareViewComponent vc, string file) =>
                    Path.IsPathRooted(file)
                ? file
                : Path.Combine(Path.GetDirectoryName(vc.ViewContext.ExecutingFilePath), file);

        private static string GetFileContent(IFileInfo file)
        {
            if (FileModifications.TryGetValue(file.PhysicalPath, out var modified) &&
                modified == file.LastModified &&
                FileContent.TryGetValue(file.PhysicalPath, out var existingContent))
            {
                return existingContent;
            }

            using (var stream = file.CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();

                FileModifications.AddOrUpdate(file.PhysicalPath, file.LastModified, (_, __) => file.LastModified);
                return FileContent.AddOrUpdate(file.PhysicalPath, content, (_, __) => content);
            }
        }
    }
}