namespace PDV.App;

public class MenuProdutos
{
    private readonly CadastroProduto _cadastro;
    private readonly Armazenamento _armazenamento;

    public MenuProdutos(CadastroProduto cadastro, Armazenamento armazenamento)
    {
        _cadastro = cadastro;
        _armazenamento = armazenamento;
    }

    public void Exibir()
    {
        int opcao;

        do
        {
            Console.WriteLine();
            Console.WriteLine("=== CADASTRO DE PRODUTOS ===");
            Console.WriteLine("1 - Listar produtos");
            Console.WriteLine("2 - Adicionar produto");
            Console.WriteLine("3 - Alterar produto");
            Console.WriteLine("4 - Remover produto");
            Console.WriteLine("0 - Voltar");
            opcao = Entrada.LerInteiro("Opção: ");

            switch (opcao)
            {
                case 1:
                    ListarProdutos();
                    break;
                case 2:
                    AdicionarProduto();
                    break;
                case 3:
                    AlterarProduto();
                    break;
                case 4:
                    RemoverProduto();
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        } while (opcao != 0);
    }

    private void ListarProdutos()
    {
        Console.WriteLine();

        if (_cadastro.Listar().Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (Produto produto in _cadastro.Listar())
            Console.WriteLine(produto);
    }

    private void AdicionarProduto()
    {
        int codigo = Entrada.LerInteiro("Código: ");

        if (_cadastro.BuscarPorCodigo(codigo) != null)
        {
            Console.WriteLine("Já existe um produto com esse código.");
            return;
        }

        string nome = Entrada.LerTexto("Nome: ");
        decimal preco = Entrada.LerDecimal("Preço: ");
        int estoque = Entrada.LerInteiro("Estoque: ");

        try
        {
            _cadastro.Adicionar(new Produto(codigo, nome, preco, estoque));
            _armazenamento.SalvarProdutos();
            Console.WriteLine("Produto adicionado com sucesso.");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine($"Não foi possível adicionar: {erro.Message}");
        }
    }

    private void AlterarProduto()
    {
        int codigo = Entrada.LerInteiro("Código do produto a alterar: ");
        Produto? produto = _cadastro.BuscarPorCodigo(codigo);

        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.WriteLine($"Alterando: {produto}");
        string nome = Entrada.LerTexto("Novo nome: ");
        decimal preco = Entrada.LerDecimal("Novo preço: ");
        int estoque = Entrada.LerInteiro("Novo estoque: ");

        try
        {
            _cadastro.Alterar(codigo, nome, preco, estoque);
            _armazenamento.SalvarProdutos();
            Console.WriteLine("Produto alterado com sucesso.");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine($"Não foi possível alterar: {erro.Message}");
        }
    }

    private void RemoverProduto()
    {
        int codigo = Entrada.LerInteiro("Código do produto a remover: ");

        if (_cadastro.Remover(codigo))
        {
            _armazenamento.SalvarProdutos();
            Console.WriteLine("Produto removido com sucesso.");
        }
        else
        {
            Console.WriteLine("Produto não encontrado.");
        }
    }
}