#region Copyright
// Copyright Syncfusion Inc. 2001 - 2018. All rights reserved.
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
using System.Web.Mvc.Ajax;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace EJ2MVCSampleBrowser.Controllers.Markdown
{
    public partial class MarkdownController : Controller
    {
        #region Word to Markdown
        public ActionResult WordToMarkdown(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();
            WordDocument document = GetInputWordocumentForConversion(file);
            if (document != null)
            {
                string output = file == null ? "WordtoMD" : Path.GetFileNameWithoutExtension(file.FileName);

                //Convert word document into Markdown document
                try
                {
                   return document.ExportAsActionResult(output + ".md", FormatType.Markdown, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                }
                catch (Exception)
                {
                }
                finally
                {

                }
            }
            return View();
        }
        /// <summary>
        /// Gets the Word document from default template document or uploaded document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <returns>Returns the Word document instance.</returns>
        private WordDocument GetInputWordocumentForConversion(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".doc" || extension == ".docx" || extension == ".dot" || extension == ".dotx" || extension == ".dotm" || extension == ".docm"
                   || extension == ".xml" || extension == ".rtf")
                    return new WordDocument(file.InputStream, FormatType.Automatic);
                else
                    ViewData["Message"] = string.Format("Please choose Word format document to convert to Markdown");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("WordtoMD.docx");
                return new WordDocument(filePath);
            }
            return null;
        }
        #endregion Word to Markdown

        protected string ResolveApplicationDataPath(string fileName)
        {
            string dataPath = new System.IO.DirectoryInfo(Server.MapPath("~\\App_Data")).FullName;
            if (fileName != string.Empty)
                dataPath += "\\Markdown\\" + fileName;
            return dataPath;
        }
    }

    #region Word
    /// <summary>
    /// This Class represents the Custom ActionResult for WordDocument.
    /// </summary>
    public class DocumentResult : ActionResult
    {
        #region Fields
        private string m_filename;
        private IWordDocument m_document;
        private FormatType m_formatType;
        private HttpResponse m_response;
        private HttpContentDisposition m_contentDisposition;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets/Sets the Name of the file.
        /// </summary>
        /// <value>Name of the file</value>
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
        /// <summary>
        /// Gets the WordDocument
        /// </summary>
        /// <value>The WordDocument</value>
        public IWordDocument Document
        {
            get
            {
                if (m_document != null)
                    return m_document;
                return null;
            }
        }
        /// <summary>
        /// Gets/Sets the Format Type
        /// </summary>
        /// <value>The FormatType</value>
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
        /// <summary>
        /// Gets/Sets the type of ContentDisposition
        /// </summary>
        /// <value>The type of the ContentDisposition.</value>
        public HttpContentDisposition ContentDisposition
        {
            get
            {
                return m_contentDisposition;
            }
            set
            {
                m_contentDisposition = value;
            }
        }
        /// <summary>
        /// Gets the response
        /// </summary>
        /// <value>The Response.</value>
        public HttpResponse Response
        {
            get
            {
                return m_response;
            }
        }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentResult"/> class.
        /// </summary>
        /// <param name="document">The Word Document</param>
        /// <param name="filename">The Filename</param>
        /// <param name="formattype">The FormatType</param>
        /// <param name="response">The Resposne</param>
        /// <param name="contentDisposition">The Type of ContentDisposition</param>
        public DocumentResult(IWordDocument document, string filename, FormatType formattype, HttpResponse response, HttpContentDisposition contentDisposition)
        {
            FileName = filename;
            m_document = document;
            this.formatType = formattype;
            m_response = response;
            this.ContentDisposition = contentDisposition;
        }
        #endregion Constructor

        #region Implementation
        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The Context.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            this.Document.Save(FileName, formatType, Response, ContentDisposition);
        }
        #endregion Implementation
    }
    /// <summary>
    /// DocIO Extension
    /// </summary>
    public static class DocIOExtension
    {
        /// <summary>
        /// Export the document as ActionResult, returns the DocResult
        /// </summary>
        /// <param name="document">WordDocument to serialize</param>
        /// <param name="filename">Name of the File</param>
        /// <param name="formattype">Format type of the document</param>
        /// <param name="response">Response</param>
        /// <param name="contentDisposition">HttpContentDisposition</param>
        /// <returns></returns>
        public static DocumentResult ExportAsActionResult(this WordDocument document, string filename, FormatType formattype, HttpResponse response, HttpContentDisposition contentDisposition)
        {
            return new DocumentResult(document, filename, formattype, response, contentDisposition);
        }
    }
    #endregion Word
}