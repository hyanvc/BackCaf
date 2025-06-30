using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BackCaf.BO;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FidelidadeController : ControllerBase
    {
        private readonly ProdutoBO _bo = new();

        /// <summary>
        /// Retorna a quantidade de recompensas de fidelidade do usuário.
        /// </summary>
        [HttpGet("{usuario}")]
        public IActionResult GetFidelidade(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usuário é obrigatório.");

            var concluidos = _bo.Listar()
                .Where(p => p.Usuario != null
                            && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase)
                            && p.Status != null
                            && p.Status.Equals("Concluido", System.StringComparison.OrdinalIgnoreCase))
                .Count();

            int recompensas = concluidos / 2;
            int faltam = 2 - (concluidos % 2);

            return Ok(new
            {
                Usuario = usuario,
                PedidosConcluidos = concluidos,
                RecompensasDisponiveis = recompensas,
                FaltamParaProxima = faltam == 2 ? 0 : faltam
            });
        }

        /// <summary>
        /// Consome uma recompensa de fidelidade do usuário, se disponível.
        /// </summary>
        [HttpPost("{usuario}/consumir")]
        public IActionResult ConsumirFidelidade(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usuário é obrigatório.");

            // Busca todos os pedidos concluídos do usuário
            var concluidos = _bo.Listar()
                .Where(p => p.Usuario != null
                            && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase)
                            && p.Status != null
                            && p.Status.Equals("Concluido", System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Id)
                .ToList();

            int recompensas = concluidos.Count / 2;
            if (recompensas < 1)
                return BadRequest("Usuário não possui recompensas disponíveis para consumir.");

            // Marca os dois pedidos mais antigos como "ConsumidoFidelidade"
            var pedidosParaConsumir = concluidos.Take(2).ToList();
            foreach (var pedido in pedidosParaConsumir)
            {
                _bo.AtualizarStatus(pedido.Id, "ConsumidoFidelidade");
            }

            return Ok(new
            {
                Usuario = usuario,
                Mensagem = "Recompensa de fidelidade consumida com sucesso!",
                PedidosConsumidos = pedidosParaConsumir.Select(p => p.Id).ToList()
            });
        }
    }
}