using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Models
{
	public class APICallResponse<T>
	{
		public bool IsSuccess { get; set; }
		public string Status { get; set; }
		public List<string> Message { get; set; }
		public T Respose { get; set; }
	}
}
