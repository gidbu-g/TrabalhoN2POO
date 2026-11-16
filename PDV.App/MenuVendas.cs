namespace PDV.App;

public class MenuVendas
{
    private readonly CadastroVenda _cadastro;

    public MenuVendas(CadastroVenda cadastro)
    {
        _cadastro = cadastro;
    }

    public void Exibir()
    {
        int opcao;

        do
        {
            Console.WriteLine();
            Console.WriteLine("=== VENDAS ===");
            Console.WriteLine("1 - Listar vendas");
            Console.WriteLine("2 - Nova venda");
            Console.WriteLine("3 - Continuar venda em aberto");
            Console.WriteLine("4 - Cancelar venda");
            Console.WriteLine("0 - Voltar");
            opcao = Entrada.LerInteiro("Opção: ");

            switch (opcao)
            {
                case 1:
                    ListarVendas();
                    break;
                case 2:
                    NovaVenda();
                    break;
                case 3:
                    ContinuarVenda();
                    break;
                case 4:
                    CancelarVenda();
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        } while (opcao != 0);
    }

    private void ListarVendas()
    {
        Console.WriteLine();

        if (_cadastro.Listar().Count == 0)
        {
            Console.WriteLine("Nenhuma venda registrada.");
            return;
        }

        foreach (Venda venda in _cadastro.Listar())
        {
            string produtos = venda.Itens.Count == 0
                ? "(sem itens)"
                : string.Join(", ", venda.Itens.Select(item => item.NomeProduto));
            string pagamento = venda.Pagamento?.ToString() ?? "não realizado";

            Console.WriteLine($"Venda {venda.Codigo} [{DescreverStatus(venda)}]");
            Console.WriteLine($"  Produtos: {produtos}");
            Console.WriteLine($"  Total: {venda.Total:C}");
            Console.WriteLine($"  Pagamento: {pagamento}");
        }
    }

    private void NovaVenda()
    {
        Venda venda = _cadastro.Criar();
        Console.WriteLine($"Venda {venda.Codigo} iniciada.");
        AtenderVenda(venda);
    }

    private void ContinuarVenda()
    {
        List<Venda> abertas = _cadastro.Listar()
            .Where(venda => venda.PodeSerAlterada)
            .ToList();

        if (abertas.Count == 0)
        {
            Console.WriteLine("Não há vendas em aberto.");
            return;
        }

        foreach (Venda aberta in abertas)
            Console.WriteLine($"Venda {aberta.Codigo} | {aberta.Itens.Count} item(ns) | Total: {aberta.Total:C}");

        int codigo = Entrada.LerInteiro("Código da venda: ");
        Venda? venda = abertas.FirstOrDefault(v => v.Codigo == codigo);

        if (venda == null)
        {
            Console.WriteLine("Venda em aberto não encontrada.");
            return;
        }

        AtenderVenda(venda);
    }

    private void CancelarVenda()
    {
        int codigo = Entrada.LerInteiro("Código da venda a cancelar: ");
        Venda? venda = _cadastro.BuscarPorCodigo(codigo);

        if (venda == null)
        {
            Console.WriteLine("Venda não encontrada.");
            return;
        }

        if (venda.Cancelada)
        {
            Console.WriteLine("Esta venda já está cancelada.");
            return;
        }

        if (!Entrada.LerSimNao($"Confirma o cancelamento da venda {codigo}?"))
            return;

        bool estavaPaga = venda.Paga;
        _cadastro.Cancelar(venda);

        Console.WriteLine(estavaPaga
            ? "Venda cancelada. As quantidades voltaram ao estoque."
            : "Venda cancelada.");
    }

    private void AtenderVenda(Venda venda)
    {
        int opcao;

        do
        {
            Console.WriteLine();
            Console.WriteLine($"=== VENDA {venda.Codigo} ===");
            Console.WriteLine("1 - Adicionar ou alterar item");
            Console.WriteLine("2 - Remover item");
            Console.WriteLine("3 - Ver itens e total");
            Console.WriteLine("4 - Pagar");
            Console.WriteLine("0 - Sair (a venda continua em aberto)");
            opcao = Entrada.LerInteiro("Opção: ");

            switch (opcao)
            {
                case 1:
                    AdicionarItem(venda);
                    break;
                case 2:
                    RemoverItem(venda);
                    break;
                case 3:
                    ExibirItens(venda);
                    break;
                case 4:
                    PagarVenda(venda);
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        } while (opcao != 0 && venda.PodeSerAlterada);
    }

    private void AdicionarItem(Venda venda)
    {
        string produto = Entrada.LerTexto("Código ou nome do produto: ");
        int quantidade = Entrada.LerInteiroComPadrao("Quantidade (0 remove o item)", 1);

        ResultadoItemVenda resultado = _cadastro.AdicionarItem(venda, produto, quantidade);
        Console.WriteLine(DescreverResultado(resultado));
    }

    private void RemoverItem(Venda venda)
    {
        int codigo = Entrada.LerInteiro("Código do produto a remover: ");

        if (_cadastro.RemoverItem(venda, codigo))
            Console.WriteLine("Item removido da venda.");
        else
            Console.WriteLine("Esse produto não está na venda.");
    }

    private static void ExibirItens(Venda venda)
    {
        Console.WriteLine();

        if (venda.Itens.Count == 0)
        {
            Console.WriteLine("A venda ainda não tem itens.");
            return;
        }

        foreach (ItemVenda item in venda.Itens)
            Console.WriteLine(item);

        Console.WriteLine($"Total da venda: {venda.Total:C}");
    }

    private void PagarVenda(Venda venda)
    {
        if (!venda.PodeReceberPagamento)
        {
            Console.WriteLine("Adicione ao menos um item antes de pagar.");
            return;
        }

        if (!_cadastro.PossuiEstoqueSuficiente(venda))
        {
            Console.WriteLine("O estoque não é mais suficiente");
            return;
        }

        Console.WriteLine($"Total da venda: {venda.Total:C}");
        Console.WriteLine("1 - Dinheiro");
        Console.WriteLine("2 - Cartão de Crédito");
        Console.WriteLine("3 - Cartão de Débito");
        int forma = Entrada.LerInteiro("Forma de pagamento: ");

        if (forma < 1 || forma > 3)
        {
            Console.WriteLine("Forma de pagamento inválida.");
            return;
        }

        IPagamento? pagamento = forma switch
        {
            1 => MontarPagamentoDinheiro(),
            2 => MontarPagamentoCredito(venda),
            _ => MontarPagamentoDebito()
        };

        if (pagamento == null)
        {
            Console.WriteLine("Pagamento não realizado.");
            return;
        }

        if (_cadastro.Pagar(venda, pagamento))
        {
            Console.WriteLine("Pagamento realizado com sucesso!");
            Console.WriteLine($"Valor da venda: {venda.Total:C}");
            Console.WriteLine($"Pagamento: {venda.Pagamento}");
        }
        else
        {
            Console.WriteLine("Pagamento não realizado. Verifique o valor recebido ou a aprovação da operação.");
        }
    }

    private static IPagamento? MontarPagamentoDinheiro()
    {
        decimal valorRecebido = Entrada.LerDecimal("Valor recebido do cliente: ");
        return new PagamentoDinheiro(valorRecebido);
    }

    private static IPagamento? MontarPagamentoCredito(Venda venda)
    {
        int parcelas = Entrada.LerInteiro("Quantidade de parcelas: ");

        if (parcelas < 1)
        {
            Console.WriteLine("Quantidade de parcelas inválida.");
            return null;
        }

        var pagamento = new PagamentoCartaoCredito(parcelas);
        Console.WriteLine($"{parcelas}x de {pagamento.CalcularValorParcela(venda):C}");

        return Entrada.LerSimNao("Confirma o pagamento?") ? pagamento : null;
    }

    private static IPagamento? MontarPagamentoDebito()
    {
        bool aprovada = Entrada.LerSimNao("Operação aprovada?");
        return new PagamentoCartaoDebito(aprovada);
    }

    private static string DescreverStatus(Venda venda)
    {
        if (venda.Cancelada)
            return "Cancelada";
        if (venda.Paga)
            return "Paga";
        return "Em aberto";
    }

    private static string DescreverResultado(ResultadoItemVenda resultado)
    {
        return resultado switch
        {
            ResultadoItemVenda.ItemDefinido => "Item registrado.",
            ResultadoItemVenda.ItemRemovido => "Item removido.",
            ResultadoItemVenda.ProdutoNaoEncontrado => "Produto não encontrado.",
            ResultadoItemVenda.EstoqueInsuficiente => "Estoque insuficiente.",
            ResultadoItemVenda.QuantidadeInvalida => "Quantidade inválida.",
            ResultadoItemVenda.VendaNaoPodeSerAlterada => "Esta venda não pode mais ser alterada.",
            _ => "Resultado desconhecido."
        };
    }
}