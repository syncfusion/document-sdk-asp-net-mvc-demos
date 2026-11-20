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
        
        #region MarkdownToWord
        public ActionResult MarkdownToWord(string Group1, HttpPostedFileBase file)
        {
            if (Group1 == null)
                return View();
            string output = file == null ? "MarkdownToWord" : Path.GetFileNameWithoutExtension(file.FileName);
            WordDocument document = GetMarkdownDocument(file);
            if (document != null)
            {
                #region Document save option
                //Save as .doc format
                if (Group1 == "WordDoc")
                {
                    return document.ExportAsActionResult(output + ".doc", FormatType.Doc, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                }
                //Save as .docx format
                else if (Group1 == "WordDocx")
                {
                    return document.ExportAsActionResult(output + ".docx", FormatType.Docx, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                }
				//Save as .rtf format
                else if (Group1 == "WordRtf")
                {
                    return document.ExportAsActionResult(output + ".rtf", FormatType.Rtf, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                }
				//Save as WordML format
                else if (Group1 == "WordML")
                {
                    return document.ExportAsActionResult(output + ".xml", FormatType.WordML, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                }
                #endregion Document save option
            }
            return View();
        }
        #endregion MarkdownToWord
        /// <summary>
        /// Gets the Markdown from default template document or uploaded document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <returns>Returns the Word document instance.</returns>
        private WordDocument GetMarkdownDocument(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                
                if (extension == ".md")
                    return new WordDocument(file.InputStream, FormatType.Markdown);
                else
                    ViewData["Message"] = string.Format("Please choose Markdown format document to convert to Word or HTML or PDF");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("MarkdownToWord.md");
                return new WordDocument(filePath);
            }
            return null;
        }
    }
}