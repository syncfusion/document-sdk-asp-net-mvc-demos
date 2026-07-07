#region Copyright Syncfusion Inc. 2001 - 2018
// Copyright Syncfusion Inc. 2001 - 2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using Syncfusion.XlsIO;
using System.Web;

namespace EJ2MVCSampleBrowser.Controllers.Markdown
{
    public partial class MarkdownController : Controller
    {
        //
        // GET: /ExcelToJSON/

        public ActionResult MarkdownToExcel(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();
            string output = file == null ? "MarkdownToExcel" : Path.GetFileNameWithoutExtension(file.FileName);
            //Initialize ExcelEngine
            ExcelEngine excelEngine = new ExcelEngine();
            //Initialize Application
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;
            application.PreserveCSVDataTypes = true;
            //Open the input template workbook
            IWorkbook workbook = GetMarkdownDoc(file, application);
            if (workbook != null)
            {
                IWorksheet sheet = workbook.Worksheets[0];
                sheet.UsedRange.AutofitColumns();
                sheet.Calculate();
                //Save the Excel workbook as JSON file
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                //If the position is not set to '0' then the file will be empty.
                stream.Position = 0;
                //Download the converted JSON file in the browser
                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/excel");
                fileStreamResult.FileDownloadName = output + ".xlsx";
                workbook.Close();
                return fileStreamResult;
            }
            return View();
        }

        /// <summary>
        /// Gets the Markdown from default template document or uploaded document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <param name="file">Excel engine application.</param>
        /// <returns>Returns the Word document instance.</returns>
        private IWorkbook GetMarkdownDoc(HttpPostedFileBase file, IApplication application)
        {
            IWorkbook workbook;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".md")
                    return application.Workbooks.Open(file.InputStream, ExcelOpenType.Markdown);
                else
                    ViewData["Message"] = string.Format("Please choose Markdown format document to convert to Word or HTML or PDF");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("MarkdownToExcel.md");
                return application.Workbooks.Open(filePath, ExcelOpenType.Markdown);
            }
            return null;
        }
    }
}