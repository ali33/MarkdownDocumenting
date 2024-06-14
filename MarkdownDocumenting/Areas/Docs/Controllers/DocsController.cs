using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Markdig;
using MarkdownDocumenting.Areas.Docs.Helpers;
using MarkdownDocumenting.Areas.Docs.Models;
using MarkdownDocumenting.Areas.Docs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MarkdownDocumenting.Areas.Docs.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class DocsController : Controller
    {
        readonly DocumentationHelper docManager;
        public DocsController()
        {
            string docPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Docs");
            docManager = new DocumentationHelper(this.HttpContext, docPath);
        }
        public DocumentViewModel Index()
        {
            DocumentViewModel docModel = new DocumentViewModel();
            DocumentationItem menu = docManager.GetMenu();
            docModel.Doc = docManager.GetDocument("");
            docModel.Title = docModel.Doc.Title;
            docModel.Menu = menu;
            docModel.HtmlContent = Markdown.ToHtml(docManager.GetContent(""));
            return docModel;
        }

        [HttpGet("{folder}/{file}")]
        public DocumentViewModel Index(string folder, string file)
        {
            string relativePath = folder + "/" + file;
            DocumentViewModel docModel = new DocumentViewModel();
            DocumentationItem menu = docManager.GetMenu();
            docModel.Doc = docManager.GetDocument(relativePath);
            docModel.Title = docModel.Doc.Title;
            docModel.Menu = menu;
            docModel.HtmlContent = Markdown.ToHtml(docManager.GetContent(relativePath));
            return docModel;
        }

        [HttpGet("{file}")]
        public DocumentViewModel RootDoc(string file)
        {
            string relativePath = file;
            DocumentViewModel docModel = new DocumentViewModel();
            DocumentationItem menu = docManager.GetMenu();
            docModel.Doc = docManager.GetDocument(relativePath);
            docModel.Title = docModel.Doc.Title;
            docModel.Menu = menu;
            docModel.HtmlContent = Markdown.ToHtml(docManager.GetContent(relativePath));
            return docModel;
        }
    }
}
