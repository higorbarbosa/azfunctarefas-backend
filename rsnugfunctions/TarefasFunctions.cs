using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using rsnugfunctions;
using rsnugfunctions.Models;
using rsnugfunctions.Mappings;

namespace ServerlessFuncs
{
	public static class TarefasFunctions
	{
		[FunctionName("CriarTarefa")]
		public static async Task<IActionResult> CriarTarefa(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tarefa")]HttpRequest req,
			[Table("tarefas", Connection = "AzureWebJobsStorage")] IAsyncCollector<TarefasTableEntity> tarefaTabela,
			[Queue("tarefas", Connection = "AzureWebJobsStorage")] IAsyncCollector<Tarefas> tabelaQueue,
			TraceWriter log)
		{
			log.Info("Criando nova Tarefa");
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var input = JsonConvert.DeserializeObject<TarefaInserir>(requestBody);

			var tarefa = new Tarefas() { NomeTarefa = input.NomeTarefa };
			await tarefaTabela.AddAsync(tarefa.ToTableEntity());
			await tabelaQueue.AddAsync(tarefa);
			return new OkObjectResult(tarefa);
		}

		[FunctionName("ListarTarefas")]
		public static async Task<IActionResult> ListarTarefas(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tarefas")]HttpRequest req,
			[Table("tarefas", Connection = "AzureWebJobsStorage")] CloudTable tarefaTable,
			TraceWriter log)
		{
			log.Info("Listando todas tarefas.");
			var query = new TableQuery<TarefasTableEntity>();
			var segment = await tarefaTable.ExecuteQuerySegmentedAsync(query, null);
			return new OkObjectResult(segment.Select(Mappings.ToTarefa));
		}

		[FunctionName("GetTarefaById")]
		public static IActionResult GetTarefaById(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tarefa/{id}")]HttpRequest req,
			[Table("tarefas", "TAREFA", "{id}", Connection = "AzureWebJobsStorage")] TarefasTableEntity tarefa,
			TraceWriter log, string id)
		{
			log.Info("Pegando tarefa por id.");
			if (tarefa == null)
			{
				log.Info($"Tarefa {id} nao encontrada.");
				return new NotFoundResult();
			}
			return new OkObjectResult(tarefa.ToTarefa());
		}

		[FunctionName("AtualizarTarefas")]
		public static async Task<IActionResult> AtualizarTarefas(
			[HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tarefa/{id}")]HttpRequest req,
			[Table("tarefas", Connection = "AzureWebJobsStorage")] CloudTable tarefaTable,
			TraceWriter log, string id)
		{
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var atualizar = JsonConvert.DeserializeObject<TarefaAtualizar>(requestBody);
			var operador = TableOperation.Retrieve<TarefasTableEntity>("TAREFA", id);
			var resultado = await tarefaTable.ExecuteAsync(operador);
			if (resultado.Result == null)
			{
				return new NotFoundResult();
			}
			var existeLinha = (TarefasTableEntity)resultado.Result;
			existeLinha.Finalizada = atualizar.Finalizada;
			if (!string.IsNullOrEmpty(atualizar.NomeTarefa))
			{
				existeLinha.NomeTarefa = atualizar.NomeTarefa;
			}

			var replaceOperation = TableOperation.Replace(existeLinha);
			await tarefaTable.ExecuteAsync(replaceOperation);
			return new OkObjectResult(existeLinha.ToTarefa());
		}

		[FunctionName("ExcluirTarefa")]
		public static async Task<IActionResult> ExcluirTarefa(
			[HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tarefa/{id}")]HttpRequest req,
			[Table("tarefas", Connection = "AzureWebJobsStorage")] CloudTable tarefaTabela,
			TraceWriter log, string id)
		{
			var exclusao = TableOperation.Delete(new TableEntity()
			{ PartitionKey = "TAREFA", RowKey = id, ETag = "*" });
			try
			{
				var deleteResult = await tarefaTabela.ExecuteAsync(exclusao);
			}
			catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
			{
				return new NotFoundResult();
			}
			return new OkResult();
		}
	}
}
