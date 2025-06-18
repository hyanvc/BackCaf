using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackCaf.Models;

namespace BackCaf.DAO
{
    public class ProdutoDAO
    {
        private readonly string _caminhoArquivo;

        public ProdutoDAO()
        {
            var pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            _caminhoArquivo = Path.Combine(pastaDownloads, "produtos.txt");

            if (!File.Exists(_caminhoArquivo))
                File.Create(_caminhoArquivo).Close();
        }   

        public IEnumerable<(int Id, string Descricao, decimal Preco, string Usuario, string Status)> Listar()
        {
            var linhas = File.ReadAllLines(_caminhoArquivo);
            foreach (var linha in linhas)
            {
                var partes = linha.Split(';');
                if (partes.Length == 5 &&
                    int.TryParse(partes[0], out int id) &&
                    decimal.TryParse(partes[2], out decimal preco))
                {
                    yield return (id, partes[1], preco, partes[3], partes[4]);
                }
            }
        }

        public (int, string, decimal, string, string)? Obter(int id)
        {
            return Listar().FirstOrDefault(p => p.Id == id);
        }

        public (int, string, decimal, string, string) Adicionar(Bebida bebida, string usuario)
        {
            int novoId = 1;
            var produtos = Listar().ToList();
            if (produtos.Any())
                novoId = produtos.Max(p => p.Id) + 1;

            string status = "Pendente";
            using (var writer = File.AppendText(_caminhoArquivo))
            {
                writer.WriteLine($"{novoId};{bebida.Descricao};{bebida.Preco};{usuario};{status}");
            }
            return (novoId, bebida.Descricao, bebida.Preco, usuario, status);
        }

        public bool Remover(int id)
        {
            var produtos = Listar().ToList();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == default) return false;

            produtos.Remove(produto);
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
            return true;
        }

        public bool Atualizar(int id, Bebida bebida)
        {
            var produtos = Listar().ToList();
            var index = produtos.FindIndex(p => p.Id == id);
            if (index == -1) return false;

            var usuario = produtos[index].Usuario;
            var status = produtos[index].Status;
            produtos[index] = (id, bebida.Descricao, bebida.Preco, usuario, status);
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
            return true;
        }
    }
}