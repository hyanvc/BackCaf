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
            // Removido IsUser do login
        }

        public class UsuarioCadastro
        {
            public string Usuario { get; set; }
            public string Senha { get; set; }
            public bool IsUser { get; set; }
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
                if (partes.Length == 3 && login.Usuario == partes[0] && login.Senha == partes[1])
                {
                    bool isUser = bool.TryParse(partes[2], out var val) && val;
                    if (isUser)
                        return Ok(1); // tela usuario
                    else
                        return Ok(2); // tela cozinha
                }
            }

            return Unauthorized("Usuário ou senha inválidos.");
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] UsuarioCadastro novoUsuario)
        {
            if (string.IsNullOrWhiteSpace(novoUsuario.Usuario) || string.IsNullOrWhiteSpace(novoUsuario.Senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            var linhas = System.IO.File.ReadAllLines(_caminhoArquivo);

            if (linhas.Any(linha => linha.Split(';')[0] == novoUsuario.Usuario))
                return Conflict("Usuário já existe.");

            using (var writer = System.IO.File.AppendText(_caminhoArquivo))
            {
                writer.WriteLine($"{novoUsuario.Usuario};{novoUsuario.Senha};{novoUsuario.IsUser}");
            }

            return Ok("Usuário cadastrado com sucesso.");
        }
    }
}
