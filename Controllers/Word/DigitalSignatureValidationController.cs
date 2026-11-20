using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EJ2MVCSampleBrowser.Models;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Office;

namespace EJ2MVCSampleBrowser.Controllers.Word
{
    public partial class WordController : Controller
    {
        /// <summary>
        /// Displays the Digital Signature Validation sample.
        /// </summary>
        public ActionResult DigitalSignatureValidation()
        {
            SignatureValidationMesssage message =
                new SignatureValidationMesssage();

            message.Message =
                "Browse a DOCX document and then click the button to validate digital signature";

            return View(
                "DigitalSignatureValidation",
                message);
        }

        /// <summary>
        /// Validates the digital signatures in the uploaded Word document.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DigitalSignatureValidation(
            HttpPostedFileBase file)
        {
            SignatureValidationMesssage message =
                new SignatureValidationMesssage();

            // Check whether a Word document is selected.
            if (file == null ||
                file.ContentLength == 0)
            {
                message.Message =
                    "Please choose DOCX document to validate digital signature";

                return View(
                    "DigitalSignatureValidation",
                    message);
            }

            // Check whether the selected document is a DOCX file.
            string extension =
                Path.GetExtension(file.FileName);

            if (!string.Equals(
                extension,
                ".docx",
                StringComparison.OrdinalIgnoreCase))
            {
                message.Message =
                    "Please choose DOCX document to validate digital signature";

                return View(
                    "DigitalSignatureValidation",
                    message);
            }

            WordDocument document = null;

            try
            {
                // Loads the uploaded Word document.
                document =
                    new WordDocument(
                        file.InputStream,
                        FormatType.Docx);

                StringBuilder builder =
                    new StringBuilder();

                string fileName =
                    Path.GetFileName(file.FileName);

                // Checks whether the document contains digital signatures.
                if (document.DigitalSignatures.Count == 0)
                {
                    builder.AppendLine(
                        "No digital signatures were found in " +
                        fileName);
                }
                else
                {
                    builder.AppendLine(
                        "The following digital signatures were found in " +
                        fileName);

                    int index = 1;

                    // Iterates through all digital signatures.
                    foreach (
                        OfficeDigitalSignature signature
                        in document.DigitalSignatures)
                    {
                        builder.AppendLine();

                        builder.AppendLine(
                            string.Format(
                                "{0:D3}:",
                                index));

                        // Validates the digital signature.
                        if (signature.IsValid)
                        {
                            builder.AppendLine(
                                "The signature is valid");
                        }
                        else
                        {
                            builder.AppendLine(
                                "The signature is invalid");
                        }

                        // Adds signature information.
                        builder.AppendLine(
                            "Signed at : " +
                            signature.SigningTime);

                        // Adds certificate information.
                        builder.AppendLine(
                            "Subject name : " +
                            signature.Certificate.Subject);

                        builder.AppendLine(
                            "Issuer name : " +
                            signature.Certificate.Issuer);

                        builder.AppendLine(
                            "Valid From : " +
                            signature.Certificate.ValidFrom);

                        builder.AppendLine(
                            "Valid To : " +
                            signature.Certificate.ValidTo);

                        builder.AppendLine();

                        index++;
                    }
                }

                message.Message =
                    builder.ToString();
            }
            catch (Exception ex)
            {
                message.Message =
                    "Unable to validate the digital signature. " +
                    ex.Message;
            }
            finally
            {
                // Closes the Word document.
                if (document != null)
                {
                    document.Close();
                }
            }

            return View(
                "DigitalSignatureValidation",
                message);
        }
    }
}