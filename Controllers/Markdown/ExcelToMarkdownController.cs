#region Copyright Syncfusion Inc. 2001 - 2018
// Copyright Syncfusion Inc. 2001 - 2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EJ2MVCSampleBrowser.Controllers.Markdown
{
    public partial class MarkdownController : Controller
    {
        //
        // GET: /ExcelToJSON/

        public ActionResult ExcelToMarkdown(string button, HttpPostedFileBase file)
        {
            if (button == null)
                return View();
            string output = file == null ? "ExcelToMarkdown" : Path.GetFileNameWithoutExtension(file.FileName);
            //Initialize ExcelEngine
            ExcelEngine excelEngine = new ExcelEngine();
            if (excelEngine != null)
            {
                //Initialize Application
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                //Open the input template workbook
                IWorkbook workbook = GetInputExcel(file, application);
                if (workbook != null)
                {
                    //Save the Excel workbook as markdown file
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream, ExcelSaveType.Markdown);
                    //If the position is not set to '0' then the file will be empty.
                    stream.Position = 0;
                    //Download the converted markdown file in the browser
                    FileStreamResult fileStreamResult = new FileStreamResult(stream, "text/markdown");
                    fileStreamResult.FileDownloadName = output + ".md";
                    workbook.Close();
                    return fileStreamResult;
                }
            }
            return View();
        }

        /// <summary>
        /// Gets the Excel from default template document or uploaded document.
        /// </summary>
        /// <param name="file">HttpPostedFileBase contains the uploaded file data.</param>
        /// <param name="application">Excel engine application.</param>
        /// <returns>Returns the IWorkbook document instance.</returns>
        private IWorkbook GetInputExcel(HttpPostedFileBase file, IApplication application)
        {
            IWorkbook workbook;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension == ".xls" || extension == ".xlsx" || extension == ".xlsm")
                {
                    workbook = application.Workbooks.Open(file.InputStream);
                    return workbook;
                }
                else
                    ViewData["Message"] = string.Format("Please choose Excel document to convert to Markdown");
            }
            else
            {
                string filePath = ResolveApplicationDataPath("ExcelToMarkdown.xlsx");
                workbook = application.Workbooks.Open(filePath);
                return workbook;
            }
            return null;
        }
    }

    #region Excel
    public class ExcelResult : ActionResult
    {
        private IWorkbook m_source;
        private ExcelEngine m_engine;
        private string m_filename;
        private HttpResponse m_response;
        private ExcelDownloadType m_downloadType;
        private ExcelHttpContentType m_contentType;
        private string m_separator;

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
        public IWorkbook Source
        {
            get
            {
                return m_source as IWorkbook;
            }

        }
        public ExcelEngine Engine
        {
            get
            {
                return m_engine as ExcelEngine;
            }

        }
        public HttpResponse Response
        {
            get
            {
                return m_response;
            }
        }
        public ExcelDownloadType DownloadType
        {
            set
            {
                m_downloadType = value;
            }
            get
            {
                return m_downloadType;
            }
        }
        public ExcelHttpContentType ContentType
        {
            set
            {
                m_contentType = value;
            }
            get
            {
                return m_contentType;
            }
        }
        public string Separator
        {
            set
            {
                m_separator = value;
            }
            get
            {
                return m_separator;
            }
        }

        public ExcelResult(ExcelEngine engine, IWorkbook source, string fileName, HttpResponse response, ExcelDownloadType downloadType, ExcelHttpContentType contentType)
        {
            this.FileName = fileName;
            this.m_source = source;
            this.m_engine = engine;
            m_response = response;
            DownloadType = downloadType;
            ContentType = contentType;
        }

        public ExcelResult(ExcelEngine engine, IWorkbook source, string fileName, string separator, HttpResponse response, ExcelDownloadType downloadType, ExcelHttpContentType contentType)
        {
            this.FileName = fileName;
            this.m_source = source;
            this.m_engine = engine;
            m_response = response;
            DownloadType = downloadType;
            ContentType = contentType;
            Separator = separator;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            if (m_contentType == ExcelHttpContentType.CSV)
            {
                this.m_source.SaveAs(FileName, Separator, Response, DownloadType, ContentType);
                this.m_source.Close();
                this.m_engine.Dispose();
            }
            else
            {
                this.m_source.SaveAs(FileName, Response, DownloadType, ContentType);
                this.m_source.Close();
                this.m_engine.Dispose();
            }
        }
    }
    public static class XlsIOExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, HttpResponse response)
        {
            ExcelHttpContentType contentType = ExcelHttpContentType.Excel2007;
            if (_workbook.Version == ExcelVersion.Excel2007)
                contentType = ExcelHttpContentType.Excel2007;
            else if (_workbook.Version == ExcelVersion.Excel97to2003)
                contentType = ExcelHttpContentType.Excel2000;

            return new ExcelResult(_engine, _workbook, filename, response, ExcelDownloadType.PromptDialog, contentType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="response"></param>
        /// <param name="DownloadType"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, HttpResponse response, ExcelDownloadType DownloadType)
        {
            ExcelHttpContentType contentType = ExcelHttpContentType.Excel2007;
            if (_workbook.Version == ExcelVersion.Excel2007)
                contentType = ExcelHttpContentType.Excel2007;
            else if (_workbook.Version == ExcelVersion.Excel97to2003)
                contentType = ExcelHttpContentType.Excel2000;
            return new ExcelResult(_engine, _workbook, filename, response, DownloadType, contentType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="response"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, HttpResponse response, ExcelHttpContentType contentType)
        {
            return new ExcelResult(_engine, _workbook, filename, response, ExcelDownloadType.PromptDialog, contentType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="response"></param>
        /// <param name="DownloadType"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
        {
            return new ExcelResult(_engine, _workbook, filename, response, DownloadType, contentType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="saveType"></param>
        /// <param name="response"></param>
        /// <param name="DownloadType"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, ExcelSaveType saveType, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
        {
            return new ExcelResult(_engine, _workbook, filename, response, DownloadType, contentType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_workbook"></param>
        /// <param name="filename"></param>
        /// <param name="saveType"></param>
        /// <param name="response"></param>
        /// <param name="DownloadType"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ExcelResult SaveAsActionResult(this ExcelEngine _engine, IWorkbook _workbook, string filename, string separator, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
        {
            return new ExcelResult(_engine, _workbook, filename, separator, response, DownloadType, contentType);
        }
    #endregion Excel
    }
}