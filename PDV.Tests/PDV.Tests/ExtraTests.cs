using PDV.App;
using Xunit;

namespace PDV.Tests;

public class PersistenciaTests
{
    [Fact]
    public void Produto_NomeComPontoEVirgula_DeveSerRecusado()
    {
        Assert.Throws<ArgumentException>(() => new Produto(1, "Mouse;Gamer", 35m, 10));
    }

    [Fact]
    public void SalvarProdutos_DeveGravarUmRegistroPorLinhaSeparadoPorPontoEVirgula()
    {
        string caminho = Path.GetTempFileName();
        try
        {
            var cadastro = new CadastroProduto();
            cadastro.Adicionar(new Produto(1, "Mouse", 35m, 234));

            cadastro.SalvarProdutos(caminho);

            Assert.Equal(new[] { "1;Mouse;35;234" }, File.ReadAllLines(caminho));
        }
        finally
        {
            File.Delete(caminho);
        }
    }

    [Fact]
    public void SalvarECarregarProdutos_DeveManterOsDados()
    {
        string caminho = Path.GetTempFileName();
        try
        {
            var original = new CadastroProduto();
            original.Adicionar(new Produto(1, "Mouse", 35.5m, 10));
            original.Adicionar(new Produto(2, "Teclado", 100m, 5));
            original.SalvarProdutos(caminho);

            var carregado = new CadastroProduto();
            carregado.CarregarProdutos(caminho);

            Assert.Equal(2, carregado.Listar().Count);
            Produto mouse = carregado.BuscarPorCodigo(1)!;
            Assert.Equal("Mouse", mouse.Nome);
            Assert.Equal(35.5m, mouse.Preco);
            Assert.Equal(10, mouse.Estoque);
        }
        finally
        {
            File.Delete(caminho);
        }
    }

    [Fact]
    public void CarregarProdutos_ArquivoInexistente_NaoDeveAlterarOCadastro()
    {
        var cadastro = new CadastroProduto();
        cadastro.Adicionar(new Produto(1, "Mouse", 35m, 10));
        string caminhoInexistente = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");

        cadastro.CarregarProdutos(caminhoInexistente);

        Assert.Single(cadastro.Listar());
    }

    [Fact]
    public void CarregarProdutos_LinhaInvalida_DeveLancarExcecaoSemAlterarOCadastro()
    {
        string caminho = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(caminho, new[] { "1;Mouse;35;10", "2;Teclado;abc;5" });
            var cadastro = new CadastroProduto();
            cadastro.Adicionar(new Produto(9, "Original", 1m, 1));

            Assert.Throws<InvalidDataException>(() => cadastro.CarregarProdutos(caminho));

            Assert.Single(cadastro.Listar());
            Assert.Equal("Original", cadastro.Listar()[0].Nome);
        }
        finally
        {
            File.Delete(caminho);
        }
    }

    [Fact]
    public void SalvarECarregarVendas_DeveManterItensPagamentoEStatus()
    {
        string caminho = Path.GetTempFileName();
        try
        {
            var produtos = new CadastroProduto();
            produtos.Adicionar(new Produto(1, "Mouse", 35.5m, 10));
            var vendas = new CadastroVenda(produtos);

            Venda paga = vendas.Criar();
            vendas.AdicionarItem(paga, "1", 2);
            vendas.Pagar(paga, new PagamentoDinheiro(100m));

            Venda cancelada = vendas.Criar();
            vendas.Cancelar(cancelada);

            vendas.SalvarVendas(caminho);

            var carregadas = new CadastroVenda(produtos);
            carregadas.CarregarVendas(caminho);

            Assert.Equal(2, carregadas.Listar().Count);

            Venda primeira = carregadas.BuscarPorCodigo(1)!;
            Assert.True(primeira.Paga);
            Assert.False(primeira.Cancelada);
            Assert.Equal(71m, primeira.Total);
            Assert.Equal("Dinheiro", primeira.Pagamento!.Forma);
            Assert.Equal(29m, primeira.Pagamento.Troco);
            Assert.Equal("Mouse", primeira.Itens[0].NomeProduto);

            Assert.True(carregadas.BuscarPorCodigo(2)!.Cancelada);
            Assert.Equal(3, carregadas.Criar().Codigo);
            Assert.Equal(8, produtos.BuscarPorCodigo(1)!.Estoque);
        }
        finally
        {
            File.Delete(caminho);
        }
    }

    [Fact]
    public void CarregarVendas_ItemDeVendaInexistente_DeveLancarExcecao()
    {
        string caminho = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(caminho, new[] { "I;5;1;Mouse;35;1" });
            var vendas = new CadastroVenda(new CadastroProduto());

            Assert.Throws<InvalidDataException>(() => vendas.CarregarVendas(caminho));
            Assert.Empty(vendas.Listar());
        }
        finally
        {
            File.Delete(caminho);
        }
    }
}