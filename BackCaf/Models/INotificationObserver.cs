namespace BackCaf.Models
{
    public interface INotificationObserver
    {
        void Notificar(string mensagem, string usuario);
    }
}