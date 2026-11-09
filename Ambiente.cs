
using System.Collections.Generic;

public class Ambiente
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public Queue<Log> Logs { get; set; }

    public Ambiente(int id, string nome)
    {
        Id = id;
        Nome = nome;
        Logs = new Queue<Log>();
    }

    public void RegistrarLog(Log log)
    {
        // Limite máximo de 100 registros
        if (Logs.Count >= 100)
        {
            Logs.Dequeue();
        }

        Logs.Enqueue(log);
    }
}
