using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using rsnugfunctions;

namespace ServerlessFuncs
{
	public static class ScheduledFunction
	{
		[FunctionName("ExclusaoAgendada")]
		public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
			[Table("tarefas", Connection = "AzureWebJobsStorage")] CloudTable tarefaTable,
			TraceWriter log)
		{
			var query = new TableQuery<TarefasTableEntity>();
			var tarefas = await tarefaTable.ExecuteQuerySegmentedAsync(query, null);
			var numExcluidas = 0;
			foreach (var tarefa in tarefas)
			{
				if (tarefa.Finalizada)
				{
					await tarefaTable.ExecuteAsync(TableOperation.Delete(tarefa));
					numExcluidas++;
				}
			}
			log.Info($"Excluido tarefa {numExcluidas} : {DateTime.Now}");
		}
	}
}
