
using System;

class Program
{
    static Cadastro cadastro = new Cadastro();

    static void Main(string[] args)
    {
        // Recupera os dados salvos anteriormente
        cadastro.Download();

        int opcao = -1;

        do
        {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine("     CONTROLE DE ACESSOS");
            Console.WriteLine("==================================");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("1 - Cadastrar ambiente");
            Console.WriteLine("2 - Consultar ambiente");
            Console.WriteLine("3 - Excluir ambiente");
            Console.WriteLine("4 - Cadastrar usuário");
            Console.WriteLine("5 - Consultar usuário");
            Console.WriteLine("6 - Excluir usuário");
            Console.WriteLine("7 - Conceder permissão");
            Console.WriteLine("8 - Revogar permissão");
            Console.WriteLine("9 - Registrar acesso");
            Console.WriteLine("10 - Consultar logs");
            Console.WriteLine("==================================");

            Console.Write("Escolha uma opção: ");

            int.TryParse(Console.ReadLine(), out opcao);

            switch (opcao)
            {
                case 0:
                    cadastro.Upload();
                    Console.WriteLine("Dados salvos com sucesso!");
                    Console.WriteLine("Encerrando o programa...");
                    break;

                case 1:
                    CadastrarAmbiente();
                    break;

                case 2:
                    ConsultarAmbiente();
                    break;

                case 3:
                    ExcluirAmbiente();
                    break;

                case 4:
                    CadastrarUsuario();
                    break;

                case 5:
                    ConsultarUsuario();
                    break;

                case 6:
                    ExcluirUsuario();
                    break;

                case 7:
                    ConcederPermissao();
                    break;

                case 8:
                    RevogarPermissao();
                    break;

                case 9:
                    RegistrarAcesso();
                    break;

                case 10:
                    ConsultarLogs();
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            if (opcao != 0)
            {
                Console.WriteLine();
                Console.WriteLine("Pressione ENTER para continuar...");
                Console.ReadLine();
            }

        } while (opcao != 0);
    }

    // 1 - CADASTRAR AMBIENTE
    static void CadastrarAmbiente()
    {
        Console.Write("ID do ambiente: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Nome do ambiente: ");
        string nome = Console.ReadLine();

        Ambiente ambiente = new Ambiente(id, nome);

        cadastro.AdicionarAmbiente(ambiente);

        Console.WriteLine("Operação de cadastro concluída.");
    }

    // 2 - CONSULTAR AMBIENTE
    static void ConsultarAmbiente()
    {
        Console.Write("Informe o ID do ambiente: ");
        int id = int.Parse(Console.ReadLine());

        Ambiente ambiente = cadastro.PesquisarAmbiente(id);

        if (ambiente != null)
        {
            Console.WriteLine("ID: " + ambiente.Id);
            Console.WriteLine("Nome: " + ambiente.Nome);
            Console.WriteLine("Quantidade de logs: " + ambiente.Logs.Count);
        }
        else
        {
            Console.WriteLine("Ambiente não encontrado!");
        }
    }

    // 3 - EXCLUIR AMBIENTE
    static void ExcluirAmbiente()
    {
        Console.Write("ID do ambiente: ");
        int id = int.Parse(Console.ReadLine());

        Ambiente ambiente = cadastro.PesquisarAmbiente(id);

        if (ambiente != null && cadastro.RemoverAmbiente(ambiente))
            Console.WriteLine("Ambiente removido com sucesso!");
        else
            Console.WriteLine("Não foi possível remover o ambiente!");
    }

    // 4 - CADASTRAR USUARIO
    static void CadastrarUsuario()
    {
        Console.Write("ID do usuário: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Nome do usuário: ");
        string nome = Console.ReadLine();

        Usuario usuario = new Usuario(id, nome);

        cadastro.AdicionarUsuario(usuario);

        Console.WriteLine("Operação de cadastro concluída.");
    }

    // 5 - CONSULTAR USUARIO
    static void ConsultarUsuario()
    {
        Console.Write("ID do usuário: ");
        int id = int.Parse(Console.ReadLine());

        Usuario usuario = cadastro.PesquisarUsuario(id);

        if (usuario != null)
        {
            Console.WriteLine("ID: " + usuario.Id);
            Console.WriteLine("Nome: " + usuario.Nome);

            Console.WriteLine("Ambientes autorizados:");

            foreach (Ambiente ambiente in usuario.Ambientes)
            {
                Console.WriteLine(ambiente.Id + " - " + ambiente.Nome);
            }

            if (usuario.Ambientes.Count == 0)
                Console.WriteLine("Nenhuma permissão cadastrada.");
        }
        else
        {
            Console.WriteLine("Usuário não encontrado!");
        }
    }

    // 6 - EXCLUIR USUARIO
    static void ExcluirUsuario()
    {
        Console.Write("ID do usuário: ");
        int id = int.Parse(Console.ReadLine());

        Usuario usuario = cadastro.PesquisarUsuario(id);

        if (usuario != null && cadastro.RemoverUsuario(usuario))
            Console.WriteLine("Usuário removido com sucesso!");
        else
            Console.WriteLine("Não foi possível remover o usuário!");
    }

    // 7 - CONCEDER PERMISSAO
    static void ConcederPermissao()
    {
        Console.Write("ID do ambiente: ");
        int idAmbiente = int.Parse(Console.ReadLine());

        Console.Write("ID do usuário: ");
        int idUsuario = int.Parse(Console.ReadLine());

        if (cadastro.ConcederPermissao(idUsuario, idAmbiente))
            Console.WriteLine("Permissão concedida!");
        else
            Console.WriteLine("Não foi possível conceder a permissão!");
    }

    // 8 - REVOGAR PERMISSAO
    static void RevogarPermissao()
    {
        Console.Write("ID do ambiente: ");
        int idAmbiente = int.Parse(Console.ReadLine());

        Console.Write("ID do usuário: ");
        int idUsuario = int.Parse(Console.ReadLine());

        if (cadastro.RevogarPermissao(idUsuario, idAmbiente))
            Console.WriteLine("Permissão revogada!");
        else
            Console.WriteLine("Não foi possível revogar a permissão!");
    }

    // 9 - REGISTRAR ACESSO
    static void RegistrarAcesso()
    {
        Console.Write("ID do ambiente: ");
        int idAmbiente = int.Parse(Console.ReadLine());

        Console.Write("ID do usuário: ");
        int idUsuario = int.Parse(Console.ReadLine());

        Usuario usuario = cadastro.PesquisarUsuario(idUsuario);
        Ambiente ambiente = cadastro.PesquisarAmbiente(idAmbiente);

        if (usuario == null || ambiente == null)
        {
            Console.WriteLine("Usuário ou ambiente não encontrado!");
            return;
        }

        bool autorizado = cadastro.RegistrarAcesso(idUsuario, idAmbiente);

        if (autorizado)
            Console.WriteLine("ACESSO AUTORIZADO!");
        else
            Console.WriteLine("ACESSO NEGADO!");
    }

    // 10 - CONSULTAR LOGS
    static void ConsultarLogs()
    {
        Console.Write("ID do ambiente: ");
        int id = int.Parse(Console.ReadLine());

        Ambiente ambiente = cadastro.PesquisarAmbiente(id);

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("1 - Somente autorizados");
        Console.WriteLine("2 - Somente negados");
        Console.WriteLine("3 - Todos os registros");
        Console.Write("Escolha o filtro: ");

        int filtro = int.Parse(Console.ReadLine());

        Console.WriteLine();
        Console.WriteLine("LOGS DO AMBIENTE: " + ambiente.Nome);
        Console.WriteLine("----------------------------------");

        foreach (Log log in ambiente.Logs)
        {
            if (filtro == 1 && !log.TipoAcesso)
                continue;

            if (filtro == 2 && log.TipoAcesso)
                continue;

            if (filtro < 1 || filtro > 3)
                continue;

            string situacao = log.TipoAcesso
                ? "AUTORIZADO"
                : "NEGADO";

            Console.WriteLine("Data: " + log.DtAcesso);
            Console.WriteLine("Usuário: " + log.Usuario.Nome);
            Console.WriteLine("Situação: " + situacao);
            Console.WriteLine("----------------------------------");
        }
    }
}
