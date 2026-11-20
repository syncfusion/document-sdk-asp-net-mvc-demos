using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Mvc.Pdf;
using System.IO;

namespace EJ2MVCSampleBrowser.Controllers.PDF
{
    public partial class PdfController : Controller
    {
        //
        // GET: /LinearizedPDF/

        public ActionResult LinearizedPDF()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LinearizedPDF(string InsideBrowser, HttpPostedFileBase file)
        {
            Stream readFile;
            string fileName;
            if (file != null && file.ContentLength > 0)
            {
                // FILE SIZE LIMIT (10 MB)
                const long MAX_FILE_SIZE = 10 * 1024 * 1024;
                if (file.ContentLength > MAX_FILE_SIZE)
                {
                    ViewData["lab"] = string.Format("The uploaded PDF file exceeds the maximum allowed size of 10 MB. File size: {0:F2} MB.", file.ContentLength / (1024.0 * 1024.0));
                    return View();
                }

                readFile = file.InputStream;
                fileName = Path.GetFileName(file.FileName);
            }
            else
            {
                readFile = new FileStream(ResolveApplicationDataPath(@"HTTP Succinctly.pdf"), FileMode.Open, FileAccess.Read, FileShare.Read);
                fileName = "HTTP Succinctly.pdf";
            }

            //Load an existing PDF document
            PdfLoadedDocument document = new PdfLoadedDocument(readFile);

            //Enable the linearization to optimize the PDF for viewing over the web
            document.FileStructure.IsLinearized = true;

            //Save the pdf file            
            if (InsideBrowser == "Browser")
            {
                return document.ExportAsActionResult(fileName, HttpContext.ApplicationInstance.Response, HttpReadType.Open);
            }
            else
            {
                return document.ExportAsActionResult(fileName, HttpContext.ApplicationInstance.Response, HttpReadType.Save);
            }
        }
    }
}
