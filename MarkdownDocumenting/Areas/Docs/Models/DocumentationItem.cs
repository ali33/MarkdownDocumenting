using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MarkdownDocumenting.Areas.Docs.Models
{
    public class DocumentationItem
    {
        [JsonPropertyName("isFolder")]
        public bool IsFolder { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("subs")]
        public List<DocumentationItem> SubDocumentations { get; set; }
    }
}
