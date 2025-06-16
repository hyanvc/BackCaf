using BackCaf.Models;

namespace BackCaf.Factories
{
    public static class BebidaFactory
    {
        public static Bebida Criar(string tipo)
        {
            return tipo.ToLower() switch
            {
                "cafe" => new Cafe(),
                "cha" => new Cha(),
                _ => throw new System.Exception("Tipo de bebida não suportado")
            };
        }
    }
}