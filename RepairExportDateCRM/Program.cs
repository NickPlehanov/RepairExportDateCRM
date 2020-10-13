using RepairExportDateCRM.Data;
using RepairExportDateCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RepairExportDateCRM {
	class Program {
		
		static void Main(string[] args) {
			List<ObjectExportDate> objectExportDates = new List<ObjectExportDate>();
			string path = @"\\Server-nas\общее\Applications\ExportFromCrmTo1C\History\test";
			string[] files = System.IO.Directory.GetFiles(path);
			if(files.Length > 0) {
				foreach(string item in files) {
					XmlDocument doc = new XmlDocument();
					doc.Load(item);
					XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Контрагенты/Контрагент/Объекты/Объект");
					foreach(XmlNode _item in nodes) {
						string attr = _item.ChildNodes[2].InnerText;
						if(!string.IsNullOrEmpty(attr))
							objectExportDates.Add(new ObjectExportDate() {
								ExportDate = DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()),
								NumberObject = int.Parse(attr)
							});
						Console.WriteLine("{0} - {1}", DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()).ToShortDateString(), attr);
					}
					//if(node != null) {
					//	string attr = node.ChildNodes[2].InnerText;
					//	if(!string.IsNullOrEmpty(attr))
					//		objectExportDates.Add(new ObjectExportDate() {
					//			ExportDate = DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()),
					//			NumberObject = int.Parse(attr)
					//		});
					//	Console.WriteLine("{0} - {1}", DateTime.Parse(item.Substring(item.LastIndexOf(@"\"), 11).Replace(@"\", " ").Trim()).ToShortDateString(), attr);
					//}					
				}
			}
			if(objectExportDates != null) {
				using (Vityaz_MSCRMContext context = new Vityaz_MSCRMContext()) {
					foreach (ObjectExportDate item in objectExportDates) {
						var obj = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewObjectNumber == item.NumberObject
						&& x.NewPriostDate == null && x.NewObjDeleteDate == null);
						if(obj!=null) {
							DateTime? exportDate = obj.NewLastExportDate.HasValue ? DateTime.TryParse(obj.NewLastExportDate.Value.ToString(), out _) ? DateTime.Parse(obj.NewLastExportDate.Value.ToString()) : DateTime.MinValue : DateTime.MinValue;
							Guid id = obj.NewGuardObjectId;
							if(item.ExportDate > exportDate) {
								obj.NewLastExportDate = item.ExportDate;
								context.SaveChanges();
							}
						}
					}
				}
			}
		}
	}
}
