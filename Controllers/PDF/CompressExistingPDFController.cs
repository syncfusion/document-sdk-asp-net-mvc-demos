#region Copyright Syncfusion Inc. 2001 - 2017
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Mvc.Pdf;
using System.IO;

namespace EJ2MVCSampleBrowser.Controllers.PDF
{
    public partial class PdfController : Controller
    {

        public ActionResult CompressExistingPDF()
        {
            Session.Clear();
            RememberCompressionOptions(
                compressImage: true,
                imageQuality: 50,
                jpxDecodeEnabled: true,
                downsampleImages: true,
                downsampleAlgorithm: "Bicubic",
                targetDpi: 150,
                optPageContents: true,
                optFont: true,
                removeMetaData: true);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompressExistingPDF(
            bool compressImage = false,
            int imageQuality = 50,
            bool jpxDecodeEnabled = false,
            bool downsampleImages = false,
            string downsampleAlgorithm = "Bicubic",
            int targetDpi = 150,
            bool optPageContents = false,
            bool optFont = false,
            bool removeMetaData = false,
            string Browser = "null",
            HttpPostedFileBase file = null)
        {

            RememberCompressionOptions(compressImage, imageQuality, jpxDecodeEnabled, downsampleImages, downsampleAlgorithm, targetDpi, optPageContents, optFont, removeMetaData);

            if (file != null && file.ContentLength > 0)
            {
                //Load a existing PDF document
                PdfLoadedDocument ldoc = new PdfLoadedDocument(file.InputStream);

                //Create a new PDF compression options
                PdfCompressionOptions options = new PdfCompressionOptions
                {
                    CompressImages = compressImage,
                    ImageQuality = Clamp(imageQuality, 10, 100),
                    EnableJpxDecode = compressImage && jpxDecodeEnabled,
                    DownsampleImages = compressImage && downsampleImages,
                    TargetDpi = Clamp(targetDpi, 36, 2400),
                    DownsampleAlgorithm = Enum.TryParse<DownsamplingAlgorithm>(downsampleAlgorithm, true, out var algorithm)
                        ? algorithm
                        : DownsamplingAlgorithm.Bicubic,
                    OptimizeFont = optFont,
                    OptimizePageContents = optPageContents,
                    RemoveMetadata = removeMetaData
                };

                //Set the options to loaded PDF document
                ldoc.CompressionOptions = options;

                string inputFileSize = "";
                inputFileSize = GetActualSize(file.ContentLength);

                Session["inputFileSize"] = inputFileSize;

                MemoryStream ms = new MemoryStream();
                ldoc.Save(ms);
                string outputFileSize = "";
                outputFileSize = GetActualSize(ms.Length);

                Session["outputFileSize"] = outputFileSize;

                string[] fileName = file.FileName.Split(new string[] { ".pdf" }, StringSplitOptions.RemoveEmptyEntries);

                //Save to disk
                //Stream the output to the browser.
                if (Browser == "Browser")
                    return ldoc.ExportAsActionResult(fileName[0] + "_compressed.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
                else
                    return ldoc.ExportAsActionResult(fileName[0] + "_compressed.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);


            }
            else
            {
                ViewData["lab"] = "Choose a valid PDF file.";
                return View();
            }
        }

        private void RememberCompressionOptions(
            bool compressImage,
            int imageQuality,
            bool jpxDecodeEnabled,
            bool downsampleImages,
            string downsampleAlgorithm,
            int targetDpi,
            bool optPageContents,
            bool optFont,
            bool removeMetaData)
        {
            ViewData["compressImage"] = compressImage;
            ViewData["imageQuality"] = Clamp(imageQuality, 10, 100);
            ViewData["jpxDecodeEnabled"] = jpxDecodeEnabled;
            ViewData["downsampleImages"] = downsampleImages;
            ViewData["downsampleAlgorithm"] = Enum.TryParse<DownsamplingAlgorithm>(downsampleAlgorithm, true, out _)
                ? downsampleAlgorithm
                : "Bicubic";
            ViewData["targetDpi"] = Clamp(targetDpi, 36, 2400);
            ViewData["optPageContents"] = optPageContents;
            ViewData["optFont"] = optFont;
            ViewData["removeMetaData"] = removeMetaData;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private string GetActualSize(double value)
        {
            string[] bytenaming = { "B", "KB", "MB", "GB", "TB" };

            int order = 0;
            while (value >= 1024 && order < bytenaming.Length - 1)
            {
                order++;
                value = value / 1024;
            }


            string result = String.Format("{0:0.##} {1}", value, bytenaming[order]);
            return result;
        }

        public string GetStatus()
        {
            string status = string.Empty;
            if (Session["inputFileSize"] != null && Session["outputFileSize"] != null)
            {
                status = "We compressed your file from " + Session["inputFileSize"] + " to " + Session["outputFileSize"]; ;
            }
            return status;
        }


    }
}
