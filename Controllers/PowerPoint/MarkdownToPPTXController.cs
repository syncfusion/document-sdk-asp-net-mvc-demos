#region Copyright Syncfusion Inc. 2001 - 2024
// Copyright Syncfusion Inc. 2001 - 2024. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.Presentation;
using Syncfusion.Mvc.Pdf;

namespace EJ2MVCSampleBrowser.Controllers
{
    public partial class PowerPointController : Controller
    {
        // GET: /MarkdownToPPTX/

        public ActionResult MarkdownToPPTX()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MarkdownToPPTX(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();

            IPresentation presentation = GetMarkdown(file);
            if (presentation != null)
            {
                string output = file == null ? "Markdown_To_PPTX" : Path.GetFileNameWithoutExtension(file.FileName);
                //  Saves the presentation
                return new PresentationResult(presentation, output + ".pptx", HttpContext.ApplicationInstance.Response, FormatType.Pptx);
            }
            return View();
        }

        /// <summary>
        /// Gets the  Markdown document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <returns>Returns the IPresentation document instance.</returns>
        private IPresentation GetMarkdown(HttpPostedFileBase file)
        {
            IPresentation presentation;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension == ".md")
                {
                    presentation = Presentation.Open(file.InputStream);
                    return presentation;
                }
                else
                    ViewData["Message"] = string.Format("Please choose Markdown format document to convert to PowerPoint Presentation");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("Markdown_To_PPTX.md");
                presentation = Presentation.Open(filePath);
                return presentation;
            }
            return null;
        }
    }
}