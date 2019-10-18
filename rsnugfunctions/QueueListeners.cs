using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using rsnugfunctions.Models;

namespace ServerlessFuncs
{
	public static class QueueListeners
	{
		[FunctionName("QueueListeners")]
		public static async Task Run([QueueTrigger("tarefas", Connection = "AzureWebJobsStorage")]Tarefas tarefa,
			[Blob("tarefas", Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
			TraceWriter log)
		{
			await container.CreateIfNotExistsAsync();
			var blob = container.GetBlockBlobReference($"{tarefa.Id}.txt");
			await blob.UploadTextAsync($"Criando uma nova tarefa: {tarefa.NomeTarefa}");
			log.Info($"C# Queue trigger function processada: {tarefa.NomeTarefa}");
		}
	}
}

