
using System.Collections.Generic;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public List<Ambiente> Ambientes { get; set; }

    public Usuario(int id, string nome)
    {
        Id = id;
        Nome = nome;
        Ambientes = new List<Ambiente>();
    }

    public bool ConcederPermissao(Ambiente ambiente)
    {
        if (Ambientes.Contains(ambiente))
        {
            return false;
        }

        Ambientes.Add(ambiente);
        return true;
    }

    public bool RevogarPermissao(Ambiente ambiente)
    {
        return Ambientes.Remove(ambiente);
    }
}
