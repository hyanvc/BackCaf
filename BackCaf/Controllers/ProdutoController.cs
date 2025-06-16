using Microsoft.AspNetCore.Mvc;
using BackCaf.BO;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private ProdutoBO _bo = new();


        [HttpGet]
        public IActionResult Get()
        {
            var produtos = _bo.Listar().ToList();
            if (!produtos.Any()) return NotFound();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produto = _bo.Obter(id);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoRequest req)
        {
            var produto = _bo.Criar(req.Tipo, req.LeiteDeAveia, req.Canela, req.SemAcucar);
            var idproduto = produto.Item1;
            return Ok(idproduto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProdutoRequest req)
        {
            if (!_bo.Atualizar(id, req.Tipo, req.LeiteDeAveia, req.Canela, req.SemAcucar))
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_bo.Remover(id)) return NotFound();
            return NoContent();
        }
    }

    public class ProdutoRequest
    {
        public string Tipo { get; set; }
        public bool LeiteDeAveia { get; set; }
        public bool Canela { get; set; }
        public bool SemAcucar { get; set; }
    }
}