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
        
        #region MarkdownToHTML
        public ActionResult MarkdownToHTML(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();
            string output = file == null ? "MarkdownToWord" : Path.GetFileNameWithoutExtension(file.FileName);
            WordDocument document = GetMarkdownDocument(file);
            if (document != null)
            {
                #region Document save option
                //Save as HTML format
                return document.ExportAsActionResult(output + ".html", FormatType.Html, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
                #endregion Document save option
            }
            return View();
        }
        #endregion MarkdownToHTML
    }
}