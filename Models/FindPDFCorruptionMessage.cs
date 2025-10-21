#region Copyright Syncfusion® Inc. 2001-2025.
// Copyright Syncfusion® Inc. 2001-2025. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EJ2MVCSampleBrowser.Models
{
    public class FindPDFCorruptionMessage
    {
        public string Message { get; set; }
    }

    public class UserRegisterationModel
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailID { get; set; }
        public string StateDropdown { get; set; }
        public bool Newsletter { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }
    }
}
