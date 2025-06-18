using System;
using System.IO;

namespace BackCaf.Models
{
    public class NotificacaoArquivoObserver : INotificationObserver
    {
        private readonly string _caminhoArquivo;

        public NotificacaoArquivoObserver()
        {
            var pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            _caminhoArquivo = Path.Combine(pastaDownloads, "notificacoes.txt");
            if (!File.Exists(_caminhoArquivo))
                File.Create(_caminhoArquivo).Close();
        }

        public void Notificar(string mensagem, string usuario)
        {
            var registro = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{usuario};{mensagem}";
            File.AppendAllText(_caminhoArquivo, registro + Environment.NewLine);
        }
    }
}