using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BackCaf.BO;
using BackCaf.Models;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricoController : ControllerBase
    {
        private readonly ProdutoBO _bo = new();

        /// <summary>
        /// Retorna o histórico de pedidos de um usuário.
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        [HttpGet("{usuario}")]
        public IActionResult GetHistorico(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usuário é obrigatório.");

            var historico = _bo.Listar()
                .Where(p => p.Usuario != null && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Id)
                .ToList();

            if (!historico.Any())
                return NotFound("Nenhum pedido encontrado para este usuário.");

            return Ok(historico);
        }
    }
}