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


namespace EJ2MVCSampleBrowser.Controllers.Excel
{
    public partial class ExcelController : Controller
    {
        //
        // GET: /ExcelToJSON/

        public ActionResult MarkdownToExcel(string button)
        {
            if (button == null)
                return View();
            else if (button == "Input Template")
            {
                Stream ms = new FileStream(ResolveApplicationDataPath(@"MarkdownToExcel.md"), FileMode.Open, FileAccess.Read);
                return File(ms, "text/markdown", "Input Template.md");
            }
            else if(button == "Convert to Excel")
            {
                //Initialize ExcelEngine
                ExcelEngine excelEngine = new ExcelEngine();

                //Initialize Application
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                application.PreserveCSVDataTypes = true;

                //Open the input template workbook
                IWorkbook workbook = application.Workbooks.Open(ResolveApplicationDataPath(@"MarkdownToExcel.md"),ExcelOpenType.Markdown);

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
                fileStreamResult.FileDownloadName = "MarkdownToExcel.xlsx";

                workbook.Close();
                return fileStreamResult;
            }
            return View();
        }
    }
}