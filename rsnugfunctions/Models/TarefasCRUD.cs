using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace rsnugfunctions
{

	public class TarefaInserir
	{
		public string NomeTarefa { get; set; }
	}

	public class TarefaAtualizar
	{
		public string NomeTarefa { get; set; }
		public bool Finalizada { get; set; }
	}

	public class TarefasTableEntity : TableEntity
	{
		public DateTime DataCriacao { get; set; }
		public string NomeTarefa { get; set; }
		public bool Finalizada { get; set; }
	}

	
}
