using PDV.App;
using Xunit;

namespace PDV.Tests;

public class CadastroVendaTests
{
    private readonly CadastroProduto _produtos = new();
    private readonly CadastroVenda _vendas;

    public CadastroVendaTests()
    {
        _produtos.Adicionar(new Produto(1, "Mouse", 35m, 10));
        _produtos.Adicionar(new Produto(2, "Teclado", 100m, 5));
        _vendas = new CadastroVenda(_produtos);
    }

    private int EstoqueDe(int codigoProduto)
    {
        return _produtos.BuscarPorCodigo(codigoProduto)!.Estoque;
    }

    [Fact]
    public void AdicionarItem_ProdutoComEstoque_DeveIncluirItemNaVenda()
    {
        Venda venda = _vendas.Criar();

        ResultadoItemVenda resultado = _vendas.AdicionarItem(venda, "1", 2);

        Assert.Equal(ResultadoItemVenda.ItemDefinido, resultado);
        Assert.Single(venda.Itens);
        Assert.Equal("Mouse", venda.Itens[0].NomeProduto);
        Assert.Equal(2, venda.Itens[0].Quantidade);
        Assert.Equal(70m, venda.Total);
    }

    [Fact]
    public void RemoverItem_ItemExistente_DeveRetirarDaVenda()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        bool removeu = _vendas.RemoverItem(venda, 1);

        Assert.True(removeu);
        Assert.Empty(venda.Itens);
        Assert.Equal(0m, venda.Total);
    }

    [Fact]
    public void Pagar_ComDinheiro_DeveRegistrarTrocoEAbaterEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        bool pagou = _vendas.Pagar(venda, new PagamentoDinheiro(100m));

        Assert.True(pagou);
        Assert.True(venda.Paga);
        Assert.Equal("Dinheiro", venda.Pagamento!.Forma);
        Assert.Equal(100m, venda.Pagamento.ValorRecebido);
        Assert.Equal(30m, venda.Pagamento.Troco);
        Assert.Equal(8, EstoqueDe(1));
    }

    [Fact]
    public void Pagar_ComCartaoCredito_DeveRegistrarParcelasEAbaterEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "2", 1);

        bool pagou = _vendas.Pagar(venda, new PagamentoCartaoCredito(4));

        Assert.True(pagou);
        Assert.Equal("Cartão de Crédito", venda.Pagamento!.Forma);
        Assert.Equal(4, venda.Pagamento.QuantidadeParcelas);
        Assert.Equal(25m, venda.Pagamento.ValorParcela);
        Assert.Equal(4, EstoqueDe(2));
    }

    [Fact]
    public void Pagar_ComCartaoDebito_DeveRegistrarPagamentoEAbaterEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 3);

        bool pagou = _vendas.Pagar(venda, new PagamentoCartaoDebito(true));

        Assert.True(pagou);
        Assert.Equal("Cartão de Débito", venda.Pagamento!.Forma);
        Assert.Equal(7, EstoqueDe(1));
    }

    [Fact]
    public void AdicionarItem_PorNome_DeveIncluirItem()
    {
        Venda venda = _vendas.Criar();

        ResultadoItemVenda resultado = _vendas.AdicionarItem(venda, "teclado", 1);

        Assert.Equal(ResultadoItemVenda.ItemDefinido, resultado);
        Assert.Equal(2, venda.Itens[0].CodigoProduto);
    }

    [Fact]
    public void AdicionarItem_ProdutoJaNaVenda_DeveAlterarQuantidade()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        _vendas.AdicionarItem(venda, "1", 5);

        Assert.Single(venda.Itens);
        Assert.Equal(5, venda.Itens[0].Quantidade);
        Assert.Equal(175m, venda.Total);
    }

    [Fact]
    public void AdicionarItem_QuantidadeZero_DeveRemoverItem()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        ResultadoItemVenda resultado = _vendas.AdicionarItem(venda, "1", 0);

        Assert.Equal(ResultadoItemVenda.ItemRemovido, resultado);
        Assert.Empty(venda.Itens);
    }

    [Fact]
    public void AdicionarItem_EstoqueInsuficiente_DeveRecusar()
    {
        Venda venda = _vendas.Criar();

        ResultadoItemVenda resultado = _vendas.AdicionarItem(venda, "1", 11);

        Assert.Equal(ResultadoItemVenda.EstoqueInsuficiente, resultado);
        Assert.Empty(venda.Itens);
    }

    [Fact]
    public void AdicionarItem_NaoDeveAbaterEstoqueAntesDoPagamento()
    {
        Venda venda = _vendas.Criar();

        _vendas.AdicionarItem(venda, "1", 4);

        Assert.Equal(10, EstoqueDe(1));
    }

    [Fact]
    public void Pagar_DinheiroInsuficiente_DeveFalharSemAbaterEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        bool pagou = _vendas.Pagar(venda, new PagamentoDinheiro(50m));

        Assert.False(pagou);
        Assert.False(venda.Paga);
        Assert.Equal(10, EstoqueDe(1));
    }

    [Fact]
    public void Pagar_DebitoNaoAprovado_DeveFalharSemAbaterEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        bool pagou = _vendas.Pagar(venda, new PagamentoCartaoDebito(false));

        Assert.False(pagou);
        Assert.False(venda.Paga);
        Assert.Equal(10, EstoqueDe(1));
    }

    [Fact]
    public void Pagar_DuasVendasDisputandoEstoque_SegundaDeveFalhar()
    {
        Venda primeira = _vendas.Criar();
        Venda segunda = _vendas.Criar();
        _vendas.AdicionarItem(primeira, "1", 8);
        _vendas.AdicionarItem(segunda, "1", 8);

        bool pagouPrimeira = _vendas.Pagar(primeira, new PagamentoCartaoDebito(true));
        bool pagouSegunda = _vendas.Pagar(segunda, new PagamentoCartaoDebito(true));

        Assert.True(pagouPrimeira);
        Assert.False(pagouSegunda);
        Assert.Equal(2, EstoqueDe(1));
    }

    [Fact]
    public void Cancelar_VendaPaga_DeveMarcarCanceladaEDevolverEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);
        _vendas.Pagar(venda, new PagamentoCartaoDebito(true));

        bool cancelou = _vendas.Cancelar(venda);

        Assert.True(cancelou);
        Assert.True(venda.Cancelada);
        Assert.Equal(10, EstoqueDe(1));
    }

    [Fact]
    public void Cancelar_VendaNaoPaga_NaoDeveAlterarEstoque()
    {
        Venda venda = _vendas.Criar();
        _vendas.AdicionarItem(venda, "1", 2);

        bool cancelou = _vendas.Cancelar(venda);

        Assert.True(cancelou);
        Assert.True(venda.Cancelada);
        Assert.Equal(10, EstoqueDe(1));
    }
}