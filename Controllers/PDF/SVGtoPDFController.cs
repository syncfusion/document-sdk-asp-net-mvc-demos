using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.Pdf;
using Syncfusion.Mvc.Pdf;
using System.IO;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
namespace EJ2MVCSampleBrowser.Controllers.PDF
{
    public partial class PdfController : Controller
    {
        //
        // GET: /SVGtoPDF/

        public ActionResult SVGtoPDF()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SVGtoPDF(HttpPostedFileBase uploadedFile, string InsideBrowser)
        {
            Stream readFile = null;
            // Maximum allowed uploaded file size: 10 MB
            int maxFileSize = 10 * 1024 * 1024;
            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                if (uploadedFile.ContentLength > maxFileSize)
                {
                    ViewData["lab"] = "The uploaded file is too large. Maximum allowed file size is 10 MB.";
                    return View();
                }
                readFile = uploadedFile.InputStream;
            }
            else
            {
                readFile = new FileStream(ResolveApplicationDataPath(@"SVGtoPDF.svg"), FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            SvgConverter converter = new SvgConverter();
            PdfTemplate temp = converter.Convert(readFile);
            
            PdfDocument document = new PdfDocument();
            document.PageSettings.Margins.All = 0;
            document.PageSettings.Size = new SizeF(temp.Width, temp.Height);
            
            PdfPage page = document.Pages.Add();
            page.Graphics.DrawPdfTemplate(temp, new PointF(0, 0), new SizeF(temp.Width, temp.Height));

            //Save the pdf file            
            if (InsideBrowser == "Browser")
            {
                return document.ExportAsActionResult("sample.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
            }
            else
            {
                return document.ExportAsActionResult("sample.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
            }
        }
    }
}
