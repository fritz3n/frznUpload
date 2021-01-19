using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Server.Certificates
{
	public class CertificationResult
	{
		public byte[] Rawdata { get; set; }
		public string SerialNumber { get; set; }
		public DateTime ValidUntil { get; set; }
	}
}
