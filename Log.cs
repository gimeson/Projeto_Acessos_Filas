
using System;

public class Log
{
    public DateTime DtAcesso { get; set; }
    public Usuario Usuario { get; set; }
    public bool TipoAcesso { get; set; }

    public Log(Usuario usuario, bool tipoAcesso)
    {
        DtAcesso = DateTime.Now;
        Usuario = usuario;
        TipoAcesso = tipoAcesso;
    }
}
