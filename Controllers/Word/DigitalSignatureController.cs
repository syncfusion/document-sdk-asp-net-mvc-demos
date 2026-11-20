using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Office;

namespace EJ2MVCSampleBrowser.Controllers.Word
{
    public partial class WordController : Controller
    {
        /// <summary>
        /// Displays the Digital Signature sample.
        /// </summary>
        public ActionResult DigitalSignature()
        {
            return View();
        }

        /// <summary>
        /// Creates a digitally signed Word document.
        /// </summary>
        [HttpPost]
        public ActionResult DigitalSignature(
            string password,
            string comment,
            string submit,
            string SignatureStandard,
            string SignatureType,
            HttpPostedFileBase wordDocument,
            HttpPostedFileBase certificate,
            HttpPostedFileBase image)
        {
            bool isUploadedDocument =
                submit == "Sign Word Document";

            bool isInvisibleSignature =
                SignatureType == "Invisible";

            if (isUploadedDocument)
            {
                if (!ValidateDigitalSignatureInputs(
                    wordDocument,
                    certificate,
                    password,
                    image,
                    isInvisibleSignature))
                {
                    ViewBag.Message =
                        "NOTE: Fill all fields and then sign Word document";

                    return View();
                }
            }

            WordDocument document = null;
            OfficeDigitalSignatureCertificate sigCertificate = null;
            string tempCertificatePath = null;

            try
            {
                if (isUploadedDocument)
                {
                    // Opens the uploaded Word document.
                    document = new WordDocument(
                        wordDocument.InputStream,
                        FormatType.Docx);

                    // Creates the digital signature certificate.
                    sigCertificate = CreateDigitalSignatureCertificate(
                        certificate,
                        password,
                        out tempCertificatePath);
                }
                else
                {
                    // Creates a new Word document.
                    document = new WordDocument();

                    IWSection section =
                        document.AddSection();

                    WTextBody body =
                        section.Body;

                    IWTextRange textRange =
                        body.AddParagraph()
                            .AppendText("INVOICE");

                    textRange.CharacterFormat.Bold = true;
                    textRange.CharacterFormat.FontSize = 24;

                    body.AddParagraph();

                    textRange =
                        body.AddParagraph()
                            .AppendText(
                                "This invoice has been generated for the goods and services provided as per our agreement.\r\n" +
                                "We value your business and look forward to serving you again in the future.");

                    body.AddParagraph();

                    string certificatePath =
                        Server.MapPath(
                            "~/App_Data/Data/Word/Certificate.pfx");

                    sigCertificate =
                        new OfficeDigitalSignatureCertificate(
                            certificatePath,
                            "password");
                }

                if (isInvisibleSignature)
                {
                    SignInvisibleDocument(
                        document,
                        sigCertificate,
                        comment,
                        SignatureStandard);
                }
                else
                {
                    AddSignatureLine(document);

                    byte[] signatureImage;

                    if (isUploadedDocument)
                    {
                        signatureImage =
                            GetImageBytes(image);
                    }
                    else
                    {
                        string imagePath =
                            Server.MapPath(
                                "~/App_Data/Images/Word/signature.png");

                        signatureImage =
                            GetImageBytes(imagePath);
                    }

                    SignVisibleDocument(
                        document,
                        sigCertificate,
                        signatureImage,
                        comment,
                        SignatureStandard);
                }

                return DownloadDigitalSignatureDocument(
                    document);
            }
            catch (Exception ex)
            {
                ViewBag.Message =
                    "Unable to create the digital signature. " +
                    ex.Message;

                return View();
            }
            finally
            {
                if (document != null)
                {
                    document.Close();
                }

                if (!string.IsNullOrEmpty(tempCertificatePath) &&
                    System.IO.File.Exists(tempCertificatePath))
                {
                    try
                    {
                        System.IO.File.Delete(
                            tempCertificatePath);
                    }
                    catch
                    {
                        // Ignore temporary file cleanup errors.
                    }
                }
            }
        }

        /// <summary>
        /// Validates the uploaded files and password.
        /// </summary>
        private bool ValidateDigitalSignatureInputs(
            HttpPostedFileBase wordDocument,
            HttpPostedFileBase certificate,
            string password,
            HttpPostedFileBase image,
            bool isInvisibleSignature)
        {
            return
                wordDocument != null &&
                wordDocument.ContentLength > 0 &&
                certificate != null &&
                certificate.ContentLength > 0 &&
                certificate.FileName.EndsWith(
                    ".pfx",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(password) &&
                (isInvisibleSignature ||
                    (image != null &&
                     image.ContentLength > 0));
        }

        /// <summary>
        /// Creates the digital signature certificate from the uploaded PFX.
        /// </summary>
        private OfficeDigitalSignatureCertificate
            CreateDigitalSignatureCertificate(
                HttpPostedFileBase certificate,
                string password,
                out string tempCertificatePath)
        {
            tempCertificatePath =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(certificate.FileName));

            certificate.SaveAs(
                tempCertificatePath);

            return new OfficeDigitalSignatureCertificate(
                tempCertificatePath,
                password);
        }

        /// <summary>
        /// Adds an invisible digital signature to the Word document.
        /// </summary>
        private void SignInvisibleDocument(
            WordDocument document,
            OfficeDigitalSignatureCertificate certificate,
            string comment,
            string signatureStandard)
        {
            SignatureSettings settings =
                new SignatureSettings
                {
                    Comments =
                        comment != null
                            ? comment
                            : string.Empty,

                    XmlDsigLevel =
                        GetXmlDsigLevel(
                            signatureStandard)
                };

            document.AddDigitalSignature(
                certificate,
                settings);
        }

        /// <summary>
        /// Adds a visible digital signature to the Word document.
        /// </summary>
        private void SignVisibleDocument(
            WordDocument document,
            OfficeDigitalSignatureCertificate certificate,
            byte[] signatureImage,
            string comment,
            string signatureStandard)
        {
            WPicture picture =
                GetSignatureLinePicture(
                    document);

            if (picture == null)
            {
                throw new Exception(
                    "Signature line not found.");
            }

            SignatureSettings settings =
                new SignatureSettings
                {
                    SignatureLineId =
                        picture.SignatureLine.Id,

                    SignTime =
                        DateTime.Now,

                    SignatureLineImage =
                        signatureImage,

                    Comments =
                        comment != null
                            ? comment
                            : string.Empty,

                    XmlDsigLevel =
                        GetXmlDsigLevel(
                            signatureStandard)
                };

            document.AddDigitalSignature(
                certificate,
                settings);
        }

        /// <summary>
        /// Adds a signature line to the Word document.
        /// </summary>
        private void AddSignatureLine(
            WordDocument document)
        {
            SignatureLineSettings settings =
                new SignatureLineSettings
                {
                    Signer = "John Doe",
                    SignerTitle = "Manager",
                    Email = "john.doe@company.com",
                    ShowDate = true
                };

            document.LastSection
                .AddParagraph()
                .AppendSignatureLine(
                    settings,
                    192,
                    96);
        }

        /// <summary>
        /// Gets the picture containing the signature line.
        /// </summary>
        private WPicture GetSignatureLinePicture(
            WordDocument document)
        {
            WPicture picture = null;

            foreach (
                WSection section
                in document.Sections)
            {
                foreach (
                    WParagraph paragraph
                    in section.Paragraphs)
                {
                    foreach (
                        Entity entity
                        in paragraph.ChildEntities)
                    {
                        WPicture currentPicture =
                            entity as WPicture;

                        if (currentPicture != null &&
                            currentPicture.IsSignatureLine)
                        {
                            picture =
                                currentPicture;

                            break;
                        }
                    }

                    if (picture != null)
                    {
                        break;
                    }
                }

                if (picture != null)
                {
                    break;
                }
            }

            return picture;
        }

        /// <summary>
        /// Gets image bytes from the uploaded signature image.
        /// </summary>
        private byte[] GetImageBytes(
            HttpPostedFileBase image)
        {
            using (MemoryStream stream =
                new MemoryStream())
            {
                image.InputStream.CopyTo(
                    stream);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Gets image bytes from the specified image path.
        /// </summary>
        private byte[] GetImageBytes(
            string imagePath)
        {
            return System.IO.File.ReadAllBytes(
                imagePath);
        }

        /// <summary>
        /// Gets the XML digital signature standard.
        /// </summary>
        private XmlDsigLevel GetXmlDsigLevel(
            string signatureStandard)
        {
            return
                signatureStandard == "XML-DSig"
                    ? XmlDsigLevel.XmlDSig
                    : XmlDsigLevel.XAdEsEpes;
        }

        /// <summary>
        /// Downloads the digitally signed Word document.
        /// </summary>
        private ActionResult DownloadDigitalSignatureDocument(
            WordDocument document)
        {
            MemoryStream stream =
                new MemoryStream();

            document.Save(
                stream,
                FormatType.Docx);

            stream.Position = 0;

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DigitalSignature.docx");
        }
    }
}