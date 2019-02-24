using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class HTMLWorkerExtended : HTMLWorker
    {
        LineSeparator line = new LineSeparator(1, 10, BaseColor.BLACK, Element.ALIGN_RIGHT, -1);
          
        public HTMLWorkerExtended(IDocListener document)
            : base(document)
        {

        }
        public override void StartElement(string tag, IDictionary<string, string> str)
        {
            
            if (tag.Equals("newpage"))
                document.Add(Chunk.NEXTPAGE);
            else if (tag.Equals("hrline")) {
                
                document.Add(new Chunk(line));

            }
            else
                base.StartElement(tag, str);
        }


    }
}