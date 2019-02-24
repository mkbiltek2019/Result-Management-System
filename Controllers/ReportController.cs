using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using PRMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;    
namespace PRMS.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        ProjectDB db = new ProjectDB();
        public ActionResult Index()
        {
           
            return View();
        }


        public ActionResult GradeSheet(string Faculty, String Session, int? Semester)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (Faculty == null || Session == null || Semester == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            List<GradeSheetDataModel> gradeSheetData = Result.GetResultsGradeSheet( Faculty,  Session,  Convert.ToInt32(Semester));

            return View(gradeSheetData);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DownloadPdf(string GridHtml)
        {  
            string HTMLContent = GridHtml;  
            Response.Clear();  
            Response.ContentType = "application/pdf";  
            Response.AddHeader("content-disposition", "attachment;filename=" + "GradeSheet.pdf");  
            Response.Cache.SetCacheability(HttpCacheability.NoCache);  
            Response.BinaryWrite(GetPDF(HTMLContent));  
            Response.End(); 
            
            return View();
        }

          public byte[] GetPDF(string pHTML)  
            {  
            byte[] bPDF = null;  
  
            MemoryStream ms = new MemoryStream();  
            TextReader txtReader = new StringReader(pHTML);

            var styles = new StyleSheet();                 
            styles.LoadTagStyle("table", "border", 1 + "px");
            styles.LoadTagStyle("th", "border", 1 + "px");
            styles.LoadTagStyle("td", "border", 1 + "px");

            styles.LoadTagStyle("table", "border-collapse", "collapse");
            styles.LoadTagStyle("th", "border-collapse", "collapse");
            styles.LoadTagStyle("td", "border-collapse", "collapse");

            // 1: create object of a itextsharp document class  
            Document doc = new Document(new Rectangle(990,712), 30, 30, 120, 25);  
  
            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            using (var htmlWorker = new HTMLWorkerExtended(doc))
            {
                //try
                //{
                    // 3: we create a worker parse the document  
                    //   HTMLWorker htmlWorker = new HTMLWorker(doc);


                    //htmlWorker.SetStyleSheet(styles);
                    // 4: we open document and start the worker on the document  
                    doc.Open();
                    htmlWorker.StartDocument();

                    // 5: parse the html into the document  

                    htmlWorker.Open();
                    //htmlWorker.Parse(new StringReader("hello world"));
                    htmlWorker.Parse(txtReader);
                    //   XMLWorkerHelper.GetInstance().ParseXHtml(oPdfWriter, doc, txtReader);

                    // 6: close the document and the worker  
                    htmlWorker.EndDocument();
                    htmlWorker.Close();
                    doc.Close();
                //}
                //catch (IOException ex)
                //{
                  //  ex.ToString();
                //}
            }
  
            bPDF = ms.ToArray();  
  
            return bPDF;  
        }  

        protected Boolean HasSession()
        {
            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin == null)
            {
                return false;
            }

            return true;
        }

	}
}