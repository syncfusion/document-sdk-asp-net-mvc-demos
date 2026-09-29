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
