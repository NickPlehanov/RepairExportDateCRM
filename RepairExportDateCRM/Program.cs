using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RepairExportDateCRM {
	class Program {
		
		static void Main(string[] args) {
			List<ObjectExportDate> objectExportDates = new List<ObjectExportDate>();
			string path = @"\\Server-nas\общее\Applications\ExportFromCrmTo1C\History";
			string[] files = System.IO.Directory.GetFiles(path);
			if(files.Length > 0) {
				foreach(string item in files) {
					//var xml = XDocument.Load(item);
					//var query = from c in xml.Root.Descendants(@"Контрагенты")
					//			//where (int)c.Attribute("id") < 4
					//			select c.Element("НомерОбъекта").Value;
					//foreach(var it in query) {
					//	Console.WriteLine(it);
					//	Console.ReadKey();
					//}
					XmlDocument doc = new XmlDocument();
					doc.Load(item);
					XmlNode node = doc.DocumentElement.SelectSingleNode("/Контрагенты/Контрагент/Объекты/Объект");
					if(node != null) {
						string attr = node.ChildNodes[2].InnerText;
						if(!string.IsNullOrEmpty(attr))
							objectExportDates.Add(new ObjectExportDate() {
								ExportDate = DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()),
								NumberObject = int.Parse(attr)
							});
						Console.WriteLine("{0} - {1}", DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()).ToShortDateString(), attr);
					}
				}
			}
			if(objectExportDates != null) {

			}
		}
	}
}
