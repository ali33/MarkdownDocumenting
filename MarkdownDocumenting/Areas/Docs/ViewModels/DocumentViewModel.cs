using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkdownDocumenting.Areas.Docs.Models;

namespace MarkdownDocumenting.Areas.Docs.ViewModels
{
    public class DocumentViewModel
    {
        public string Title { get; internal set; }
        public DocumentationItem Menu { get; internal set; }
        public string HtmlContent { get; internal set; }
        public DocumentationItem Doc { get; internal set; }
    }
}
