using System;
using System.Collections.Generic;
using System.Text;

namespace rsnugfunctions.Models
{
	public class Tarefas
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("n");
		public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
		public string NomeTarefa { get; set; }
		public bool Finalizada { get; set; }
	}
}
