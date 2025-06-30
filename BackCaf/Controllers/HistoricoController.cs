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
        /// Retorna o hist�rico de pedidos de um usu�rio.
        /// </summary>
        /// <param name="usuario">Nome do usu�rio</param>
        [HttpGet("{usuario}")]
        public IActionResult GetHistorico(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usu�rio � obrigat�rio.");

            var historico = _bo.Listar()
                .Where(p => p.Usuario != null && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Id)
                .ToList();

            if (!historico.Any())
                return NotFound("Nenhum pedido encontrado para este usu�rio.");

            return Ok(historico);
        }
    }
}