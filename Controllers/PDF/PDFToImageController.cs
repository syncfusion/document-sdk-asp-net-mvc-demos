using System.IO;
using System.Web.Mvc;
using Syncfusion.Mvc.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using Syncfusion.PdfToImageConverter;

namespace EJ2MVCSampleBrowser.Controllers.PDF
{
    public partial class PdfController : Controller
    {
        // GET: /PDFToImage/
        public ActionResult PDFToImage()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PDFToImage(string Browser, string submit1)
        {
            if (submit1 == "View Template")
            {
                string dataPath = ResolveApplicationDataPath("HTTP Succinctly.pdf");
                Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //Load the template document
                PdfLoadedDocument ldoc = new PdfLoadedDocument(file2);
                return ldoc.ExportAsActionResult("InputTemplate.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
            }

            //Load an existing PDF.
            Stream inputStream = new FileStream(ResolveApplicationDataPath("Brochure.pdf"), FileMode.Open, FileAccess.Read, FileShare.Read);
            PdfToImageConverter imageConverter = new PdfToImageConverter(inputStream);

            if (imageConverter.PageCount > 0)
            {
                //Extract images from first page.            
                Stream outputStream = imageConverter.Convert(0, false, false);

                System.Drawing.Image image = System.Drawing.Image.FromStream(outputStream);

                //Stream the output to the browser.    
                if (Browser == "Browser")
                {
                    ExportAsImage(image, "sample.png", System.Drawing.Imaging.ImageFormat.Png, HttpContext.ApplicationInstance.Response, HttpReadType.Open);
                }
                else
                {
                    ExportAsImage(image, "sample.png", System.Drawing.Imaging.ImageFormat.Png, HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                }
            }
			
            return View();
        }
    }
}
