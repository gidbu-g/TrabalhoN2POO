using System.Globalization;
using PDV.App;

CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

var cadastroProduto = new CadastroProduto();
var cadastroVenda = new CadastroVenda(cadastroProduto);
var menuProdutos = new MenuProdutos(cadastroProduto);
var menuVendas = new MenuVendas(cadastroVenda);

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