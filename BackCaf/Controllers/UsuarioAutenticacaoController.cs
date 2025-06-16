using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace SuaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioAutenticacaoController : ControllerBase
    {
        public class UsuarioLogin
        {
            public string Usuario { get; set; }
            public string Senha { get; set; }
        }

        private readonly string _caminhoArquivo;

        public UsuarioAutenticacaoController()
        {
            var pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            _caminhoArquivo = Path.Combine(pastaDownloads, "usuarios.txt");

            if (!System.IO.File.Exists(_caminhoArquivo))
            {
                System.IO.File.Create(_caminhoArquivo).Close();
            }
        }

        [HttpPost]
        public IActionResult Autenticar([FromBody] UsuarioLogin login)
        {
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            var linhas = System.IO.File.ReadAllLines(_caminhoArquivo);

            foreach (var linha in linhas)
            {
                var partes = linha.Split(';');
                if (partes.Length == 2 && login.Usuario == partes[0] && login.Senha == partes[1])
                    return Ok("Autenticação bem-sucedida.");
            }

            return Unauthorized("Usuário ou senha inválidos.");
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] UsuarioLogin novoUsuario)
        {
            if (string.IsNullOrWhiteSpace(novoUsuario.Usuario) || string.IsNullOrWhiteSpace(novoUsuario.Senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            var linhas = System.IO.File.ReadAllLines(_caminhoArquivo);

            if (linhas.Any(linha => linha.Split(';')[0] == novoUsuario.Usuario))
                return Conflict("Usuário já existe.");

            using (var writer = System.IO.File.AppendText(_caminhoArquivo))
            {
                writer.WriteLine($"{novoUsuario.Usuario};{novoUsuario.Senha}");
            }

            return Ok("Usuário cadastrado com sucesso.");
        }
    }
}
