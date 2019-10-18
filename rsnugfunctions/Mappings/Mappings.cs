using rsnugfunctions.Models;

namespace rsnugfunctions.Mappings
{
	public static class Mappings
	{
		public static TarefasTableEntity ToTableEntity(this Tarefas tarefa)
		{
			return new TarefasTableEntity()
			{
				PartitionKey = "TAREFA",
				RowKey = tarefa.Id,
				DataCriacao = tarefa.DataCriacao,
				Finalizada = tarefa.Finalizada,
				NomeTarefa = tarefa.NomeTarefa
			};
		}

		public static Tarefas ToTarefa(this TarefasTableEntity tarefa)
		{
			return new Tarefas()
			{
				Id = tarefa.RowKey,
				DataCriacao = tarefa.DataCriacao,
				Finalizada = tarefa.Finalizada,
				NomeTarefa = tarefa.NomeTarefa
			};
		}

	}
}
