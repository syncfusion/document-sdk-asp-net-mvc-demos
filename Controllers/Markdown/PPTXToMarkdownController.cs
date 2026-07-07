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

namespace EJ2MVCSampleBrowser.Controllers.Markdown
{
    public partial class MarkdownController : Controller
    {
        // GET: /PPTXToMarkdown/

        public ActionResult PPTXToMarkdown()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PPTXToMarkdown(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();

            IPresentation presentation = GetInputPresentation(file);
            if (presentation != null)
            {
                string output = file == null ? "PPTX_To_Markdown" : Path.GetFileNameWithoutExtension(file.FileName);
                return new PresentationResult(presentation, output + ".md", HttpContext.ApplicationInstance.Response, FormatType.Markdown);
            }
            return View();
        }

        /// <summary>
        /// Gets the PowerPoint presentation from default template document or uploaded document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <returns>Returns the IPresentation document instance.</returns>
        private IPresentation GetInputPresentation(HttpPostedFileBase file)
        {
            IPresentation presentation;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension == ".pptx")
                {
                    presentation = Presentation.Open(file.InputStream);
                    return presentation;
                }
                else
                    ViewData["Message"] = string.Format("Please choose PowerPoint Presentation document (PPTX) to convert to Markdown");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("PPTX_To_Markdown.pptx");
                presentation = Presentation.Open(filePath);
                return presentation;
            }
            return null;
        }
    }

    #region Presentation
    public class PresentationResult : ActionResult
    {
        private IPresentation m_source;
        private string m_filename;
        private FormatType m_formatType;
        private HttpResponse m_response;

        public string FileName
        {
            get
            {
                return m_filename;
            }
            set
            {
                m_filename = value;
            }
        }
        public IPresentation Source
        {
            get
            {
                return m_source as IPresentation;
            }

        }

        public HttpResponse Response
        {
            get
            {
                return m_response;
            }
        }

        public FormatType formatType
        {
            get
            {
                return m_formatType;
            }
            set
            {
                m_formatType = value;
            }
        }
        public PresentationResult(IPresentation source, string fileName, HttpResponse response, FormatType formatType)
        {
            this.FileName = fileName;
            this.m_source = source;
            m_response = response;
            this.formatType = formatType;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            this.m_source.Save(FileName, formatType, Response);
        }
    }
    #endregion Presentation
}