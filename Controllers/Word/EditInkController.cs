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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using Syncfusion.Mvc.Pdf;
using Syncfusion.Office;

namespace EJ2MVCSampleBrowser.Controllers.Word
{

    public partial class WordController : Controller
    {
        public ActionResult EditInk(string Button, string Group1)
        {
            if (Button == null)
                return View();
            if (Button == "View Template")
                return new TemplateResult("EditInkInput.docx", ResolveApplicationDataPath("Data\\Word"), HttpContext.ApplicationInstance.Response);

            //Opens an existing Word document.
            WordDocument document = new WordDocument(ResolveApplicationDataPath("EditInkInput.docx", "Data\\Word"));
            // Access the first section of the document.
            WSection section = document.Sections[0];

            // Access the first ink and customize its trace points.
            WInk firstInk = section.Paragraphs[0].ChildEntities[0] as WInk;
            // Move the ink vertically.
            firstInk.VerticalPosition = 25f;
            // Copy existing points into the new array.
            int oldTracePointsLength = firstInk.Traces[0].Points.Length;
            int newTracePointsLength = oldTracePointsLength + 3;
            PointF[] newTracePoints = new PointF[newTracePointsLength];
            PointF[] oldTracePoints = firstInk.Traces[0].Points;
            Array.Copy(oldTracePoints, newTracePoints, oldTracePointsLength);
            newTracePoints[newTracePoints.Length - 3] = new PointF(oldTracePoints[3].X, 0);
            newTracePoints[newTracePoints.Length - 2] = new PointF(oldTracePoints[0].X, 0);
            newTracePoints[newTracePoints.Length - 1] = new PointF(oldTracePoints[0].X, oldTracePoints[0].Y);
            // Update the trace points of the first ink with the new array.
            firstInk.Traces[0].Points = newTracePoints;

            // Access the second ink and customize its brush effect.
            WInk secondInk = section.Paragraphs[1].ChildEntities[0] as WInk;
            IOfficeInkTrace secondInkTrace = secondInk.Traces[0];
            // Set the ink size (thickness) to 1 point.
            secondInkTrace.Brush.Size = new SizeF(1f, 1f);

            // Access the third ink and customize its container width.
            WInk thirdInk = section.Paragraphs[2].ChildEntities[0] as WInk;
            // Set the width of the ink container to 130 points.
            thirdInk.Width = 130f;

            // Access the fourth ink and customize its brush color.
            WParagraph paragraph = section.Tables[0].Rows[0].Cells[0].ChildEntities[0] as WParagraph;
            WInk fourthInk = paragraph.ChildEntities[0] as WInk;
            IOfficeInkTrace fourthInkTrace = fourthInk.Traces[0];
            // Set the color of the ink stroke to Yellow.
            fourthInkTrace.Brush.Color = Color.Yellow;

            //Save as .docx format.
            if (Group1== "WordDocx")
            {
                return document.ExportAsActionResult("EditInk.docx", FormatType.Docx, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            }
            //Save as .pdf format.
            else if (Group1 == "Pdf")
            {
                DocToPDFConverter converter = new DocToPDFConverter();
                PdfDocument pdfDoc = converter.ConvertToPDF(document);
                document.Close();
                converter.Dispose();
                return pdfDoc.ExportAsActionResult("EditInk.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
            }
			
            return View();
        }
    }

}