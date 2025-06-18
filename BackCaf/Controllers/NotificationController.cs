using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        [HttpGet("{usuario}")]
        public IActionResult Get(string usuario)
        {
            var pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var caminhoArquivo = Path.Combine(pastaDownloads, "notificacoes.txt");

            if (!System.IO.File.Exists(caminhoArquivo))
                return Ok(Array.Empty<string>());

            var notificacoes = System.IO.File.ReadAllLines(caminhoArquivo)
                .Select(linha => linha.Split(';'))
                .Where(partes => partes.Length == 3 && partes[1].Equals(usuario, StringComparison.OrdinalIgnoreCase))
                .Select(partes => new { Data = partes[0], Usuario = partes[1], Mensagem = partes[2] })
                .ToList();

            return Ok(notificacoes);
        }
    }
}