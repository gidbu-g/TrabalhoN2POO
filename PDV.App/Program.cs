using System.Globalization;
using PDV.App;

CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

var cadastroProduto = new CadastroProduto();
var cadastroVenda = new CadastroVenda(cadastroProduto);
var armazenamento = new Armazenamento(cadastroProduto, cadastroVenda, AppContext.BaseDirectory);

try
{
    armazenamento.CarregarTudo();
}
catch (Exception erro) when (erro is InvalidDataException or IOException or UnauthorizedAccessException)
{
    Console.WriteLine($"Não foi possível carregar os dados salvos: {erro.Message}");
    Console.WriteLine("O sistema iniciará sem esses dados.");
}

var menuProdutos = new MenuProdutos(cadastroProduto, armazenamento);
var menuVendas = new MenuVendas(cadastroVenda, armazenamento);

int opcao;

do
{
    Console.WriteLine();
    Console.WriteLine("=== PONTO DE VENDA ===");
    Console.WriteLine("1 - Cadastro de produtos");
    Console.WriteLine("2 - Vendas");
    Console.WriteLine("0 - Sair");
    opcao = Entrada.LerInteiro("Opção: ");

    switch (opcao)
    {
        case 1:
            menuProdutos.Exibir();
            break;
        case 2:
            menuVendas.Exibir();
            break;
        case 0:
            Console.WriteLine("Encerrando o sistema.");
            break;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
} while (opcao != 0);