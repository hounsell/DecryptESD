using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DecryptESD
{
   public class WimFile
   {
      public WimHeader Header { get; set; }
      public XDocument XmlMetadata { get; set; }
   }
}
