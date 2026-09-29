#region Copyright
// Copyright Syncfusion Inc. 2001 - 2018. All rights reserved.
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
using Syncfusion.Presentation;

namespace EJ2MVCSampleBrowser.Controllers
{
    public class PresentationResult : ActionResult
    {
        private IPresentation m_source;
        
        private string m_filename;
        private FormatType m_formatType;
        private HttpResponse m_response;

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
        public IPresentation Source
        {
            get
            {
                return m_source as IPresentation;
            }

        }
        
        public HttpResponse Response
        {
            get
            {
                return m_response;
            }
        }
       
        

        public FormatType formatType
        {
            get
            {
                return m_formatType;
            }
            set
            {
                m_formatType = value;
            }
        }



        public PresentationResult(IPresentation source, string fileName, HttpResponse response, FormatType formatType)
        {
            this.FileName = fileName;
            this.m_source = source;
            m_response = response;
            this.formatType = formatType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
           
		   
            this.m_source.Save(FileName, formatType, Response);
           
            
        }
    }
}
