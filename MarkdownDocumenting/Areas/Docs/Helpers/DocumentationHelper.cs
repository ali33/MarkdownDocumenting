using MarkdownDocumenting.Areas.Docs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace MarkdownDocumenting.Areas.Docs.Helpers
{
    public class DocumentationHelper
    {
        private readonly string docsPath;
        readonly DocumentationItem rootItem;
        private readonly HttpContext context;
        readonly Dictionary<string, DocumentationItem> tableItems = new Dictionary<string, DocumentationItem>(StringComparer.OrdinalIgnoreCase);
        public DocumentationHelper(HttpContext httpContext, string rootPath = "")
        {
            docsPath = rootPath ?? Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Docs");
            rootItem = BuildMenu();
            context = httpContext;
        }

        public DocumentationItem BuildMenu()
        {
            if (!Directory.Exists(docsPath))
                return new DocumentationItem();
            var root = GetMenuItem(docsPath, true);
            return root;
        }
        public DocumentationItem GetMenu()
        {
            return rootItem;
        }
        public DocumentationItem GetMenuItem(string path, bool isFolder)
        {
            string url = path.Substring(docsPath.Length).Replace("\\", "/");
            if (url.StartsWith("/"))
                url = url.Substring(1);
            if (isFolder)
            {
                string metaFile = Path.Combine(path, "_meta.json");
                string name = GetFriendlyName(path);
                string title = name;

                DocumentationItem metaDoc = null;
                if (File.Exists(metaFile))
                {
                    try
                    {
                        metaDoc = JsonSerializer.Deserialize<DocumentationItem>(File.ReadAllText(metaFile));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (metaDoc != null && !string.IsNullOrWhiteSpace(metaDoc.Title))
                {
                    title = metaDoc.Title;
                }

                DocumentationItem folderItem = new DocumentationItem
                {
                    IsFolder = true,
                    Name = name,
                    Title = title,
                    Url = url,
                    SubDocumentations = new List<DocumentationItem>()
                };

                tableItems[url] = folderItem;

                foreach (var subFolder in Directory.GetDirectories(path))
                {
                    var subFolderItem = GetMenuItem(subFolder, true);
                    DocumentationItem fileMetaDoc;
                    if (metaDoc != null
                       && metaDoc.SubDocumentations != null
                       && ((fileMetaDoc = metaDoc.SubDocumentations.FirstOrDefault(d => string.Equals(d.Name, subFolderItem.Name, StringComparison.OrdinalIgnoreCase))) != null))
                    {
                        subFolderItem.Title = fileMetaDoc.Title;
                    }
                    folderItem.SubDocumentations.Add(subFolderItem);
                }

                foreach (var subFile in Directory.GetFiles(path, "*.md"))
                {
                    var subFileItem = GetMenuItem(subFile, false);
                    DocumentationItem fileMetaDoc;
                    if (metaDoc != null
                        && metaDoc.SubDocumentations != null
                        && ((fileMetaDoc = metaDoc.SubDocumentations.FirstOrDefault(d => string.Equals(d.Name, subFileItem.Name, StringComparison.OrdinalIgnoreCase))) != null))
                    {
                        subFileItem.Title = fileMetaDoc.Title;
                    }
                    folderItem.SubDocumentations.Add(subFileItem);
                }

                return folderItem;
            }
            else
            {
                string fileName = GetFriendlyName(path);
                var fileItem = new DocumentationItem
                {
                    IsFolder = false,
                    Name = fileName,
                    Title = fileName,
                    Url = url
                };
                tableItems[url] = fileItem;
                return fileItem;
            }
        }

        public string GetContent(string relativePath)
        {
            if (relativePath == null)
                return "";


            relativePath = RemoveFirstSlash(relativePath);

            if (tableItems.ContainsKey(relativePath))
            {
                var docItem = tableItems[relativePath];
                if (!docItem.IsFolder)
                {
                    if (!relativePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    {
                        relativePath = relativePath + ".md";
                    }
                    string filePath = relativePath;

                    filePath = Path.Combine(docsPath, filePath);
                    return File.ReadAllText(filePath);
                }
                else
                {
                    string filePath = RemoveFirstSlash(relativePath + "/Index.md");
                    filePath = Path.Combine(docsPath, filePath);
                    List<string> files = new List<string>();
                    if (File.Exists(filePath))
                    {
                        return File.ReadAllText(filePath);
                    }
                    else
                    {
                        files.Add($"{filePath}");

                        filePath = RemoveFirstSlash(relativePath + "/README.md");
                        filePath = Path.Combine(docsPath, filePath);

                        if (File.Exists(filePath))
                        {
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            files.Add($"{filePath}");
                        }
                    }
                    return $"{string.Join(" or ", files.Select(f => f.Substring(docsPath.Length).Replace("\\", "/")))} is not found";
                }
            }
            else
            {
                return $"{relativePath} is empty";
            }
        }

        string RemoveFirstSlash(string path)
        {
            if (path == null)
                return "";
            if (path.StartsWith("/") || path.StartsWith("\\"))
                return path.Substring(1);
            return path;
        }

        string GetFriendlyName(string value)
        {
            //if (withFolder)
            //    return "";

            return Path.GetFileNameWithoutExtension(value);
        }

        public DocumentationItem GetDocument(string relativePath)
        {
            if (relativePath == null)
                return new DocumentationItem();

            relativePath = RemoveFirstSlash(relativePath);
            if (tableItems.ContainsKey(relativePath))
            {
                var docItem = tableItems[relativePath];
                if (!docItem.IsFolder)
                {
                    return docItem;
                }
                else
                {
                    string filePath = filePath = RemoveFirstSlash(relativePath + "/Index.md");
                    filePath = Path.Combine(docsPath, filePath);
                    List<string> files = new List<string>();
                    if (File.Exists(filePath))
                    {
                        return GetDocument(relativePath + "/index.md");
                    }
                    else
                    {
                        files.Add($"{filePath}");

                        filePath = filePath = RemoveFirstSlash(relativePath + "/README.md");
                        filePath = Path.Combine(docsPath, filePath);

                        if (File.Exists(filePath))
                        {
                            return GetDocument(relativePath + "/README.md");
                        }
                        else
                        {
                            files.Add($"{filePath}");
                        }
                    }

                    return docItem;
                }
            }
            else
            {
                return new DocumentationItem();
            }
        }
    }
}
