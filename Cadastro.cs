
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Cadastro
{
    private List<Usuario> usuarios;
    private List<Ambiente> ambientes;

    private const string ARQUIVO = "dados.json";

    public Cadastro()
    {
        usuarios = new List<Usuario>();
        ambientes = new List<Ambiente>();
    }

    // ADICIONAR USUARIO
    public void AdicionarUsuario(Usuario usuario)
    {
        if (PesquisarUsuario(usuario.Id) != null)
        {
            Console.WriteLine("ID de usuário já cadastrado!");
            return;
        }

        usuarios.Add(usuario);
    }

    // REMOVER USUARIO
    public bool RemoverUsuario(Usuario usuario)
    {
        Usuario encontrado = PesquisarUsuario(usuario.Id);

        if (encontrado == null)
            return false;

        if (encontrado.Ambientes.Count > 0)
            return false;

        return usuarios.Remove(encontrado);
    }

    // PESQUISAR USUARIO
    public Usuario PesquisarUsuario(int id)
    {
        return usuarios.FirstOrDefault(u => u.Id == id);
    }

    // ADICIONAR AMBIENTE
    public void AdicionarAmbiente(Ambiente ambiente)
    {
        if (PesquisarAmbiente(ambiente.Id) != null)
        {
            Console.WriteLine("ID de ambiente já cadastrado!");
            return;
        }

        ambientes.Add(ambiente);
    }

    // REMOVER AMBIENTE
    public bool RemoverAmbiente(Ambiente ambiente)
    {
        Ambiente encontrado = PesquisarAmbiente(ambiente.Id);

        if (encontrado == null)
            return false;

        // Retira a permissão de todos os usuários
        foreach (Usuario usuario in usuarios)
        {
            usuario.RevogarPermissao(encontrado);
        }

        return ambientes.Remove(encontrado);
    }

    // PESQUISAR AMBIENTE
    public Ambiente PesquisarAmbiente(int id)
    {
        return ambientes.FirstOrDefault(a => a.Id == id);
    }

    // CONCEDER PERMISSAO
    public bool ConcederPermissao(int idUsuario, int idAmbiente)
    {
        Usuario usuario = PesquisarUsuario(idUsuario);
        Ambiente ambiente = PesquisarAmbiente(idAmbiente);

        if (usuario == null || ambiente == null)
            return false;

        return usuario.ConcederPermissao(ambiente);
    }

    // REVOGAR PERMISSAO
    public bool RevogarPermissao(int idUsuario, int idAmbiente)
    {
        Usuario usuario = PesquisarUsuario(idUsuario);
        Ambiente ambiente = PesquisarAmbiente(idAmbiente);

        if (usuario == null || ambiente == null)
            return false;

        return usuario.RevogarPermissao(ambiente);
    }

    // REGISTRAR ACESSO
    public bool RegistrarAcesso(int idUsuario, int idAmbiente)
    {
        Usuario usuario = PesquisarUsuario(idUsuario);
        Ambiente ambiente = PesquisarAmbiente(idAmbiente);

        if (usuario == null || ambiente == null)
            return false;

        bool autorizado = usuario.Ambientes.Contains(ambiente);

        Log log = new Log(usuario, autorizado);

        ambiente.RegistrarLog(log);

        return autorizado;
    }

    // LISTAR USUARIOS
    public List<Usuario> ListarUsuarios()
    {
        return usuarios;
    }

    // LISTAR AMBIENTES
    public List<Ambiente> ListarAmbientes()
    {
        return ambientes;
    }

    // UPLOAD - SALVAR DADOS
    public void Upload()
    {
        var dados = new
        {
            Usuarios = usuarios.Select(u => new
            {
                u.Id,
                u.Nome,
                Ambientes = u.Ambientes.Select(a => a.Id).ToList()
            }).ToList(),

            Ambientes = ambientes.Select(a => new
            {
                a.Id,
                a.Nome,
                Logs = a.Logs.Select(l => new
                {
                    l.DtAcesso,
                    UsuarioId = l.Usuario.Id,
                    l.TipoAcesso
                }).ToList()
            }).ToList()
        };

        string json = JsonSerializer.Serialize(
            dados,
            new JsonSerializerOptions { WriteIndented = true }
        );

        File.WriteAllText(ARQUIVO, json);
    }

    // DOWNLOAD - CARREGAR DADOS
    public void Download()
    {
        if (!File.Exists(ARQUIVO))
            return;

        string json = File.ReadAllText(ARQUIVO);

        using JsonDocument documento = JsonDocument.Parse(json);

        JsonElement raiz = documento.RootElement;

        usuarios.Clear();
        ambientes.Clear();

        // Primeiro carrega os ambientes
        foreach (JsonElement item in raiz.GetProperty("Ambientes").EnumerateArray())
        {
            int id = item.GetProperty("Id").GetInt32();
            string nome = item.GetProperty("Nome").GetString();

            ambientes.Add(new Ambiente(id, nome));
        }

        // Depois carrega os usuários
        foreach (JsonElement item in raiz.GetProperty("Usuarios").EnumerateArray())
        {
            int id = item.GetProperty("Id").GetInt32();
            string nome = item.GetProperty("Nome").GetString();

            Usuario usuario = new Usuario(id, nome);

            foreach (JsonElement idAmbiente in item.GetProperty("Ambientes").EnumerateArray())
            {
                Ambiente ambiente = PesquisarAmbiente(idAmbiente.GetInt32());

                if (ambiente != null)
                    usuario.ConcederPermissao(ambiente);
            }

            usuarios.Add(usuario);
        }

        // Finalmente recupera os logs
        foreach (JsonElement item in raiz.GetProperty("Ambientes").EnumerateArray())
        {
            int id = item.GetProperty("Id").GetInt32();

            Ambiente ambiente = PesquisarAmbiente(id);

            foreach (JsonElement logItem in item.GetProperty("Logs").EnumerateArray())
            {
                int usuarioId = logItem.GetProperty("UsuarioId").GetInt32();

                Usuario usuario = PesquisarUsuario(usuarioId);

                if (usuario != null)
                {
                    DateTime data = logItem.GetProperty("DtAcesso").GetDateTime();

                    bool tipo = logItem.GetProperty("TipoAcesso").GetBoolean();

                    Log log = new Log(usuario, tipo);
                    log.DtAcesso = data;

                    ambiente.RegistrarLog(log);
                }
            }
        }
    }
}
