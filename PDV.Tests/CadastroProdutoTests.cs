using PDV.App;
using Xunit;

namespace PDV.Tests;

public class CadastroProdutoTests
{
    [Fact]
    public void Adicionar_ProdutoNovo_DeveAparecerNaLista()
    {
        
        var cadastro = new CadastroProduto();
        var mouse = new Produto(1, "Mouse", 35m, 234);

        
        bool adicionou = cadastro.Adicionar(mouse);

        
        Assert.True(adicionou);
        Assert.Single(cadastro.Listar());
        Assert.Equal("Mouse", cadastro.BuscarPorCodigo(1)!.Nome);
    }

    [Fact]
    public void Alterar_ProdutoExistente_DeveAtualizarOsDados()
    {
        
        var cadastro = new CadastroProduto();
        cadastro.Adicionar(new Produto(1, "Mouse", 35m, 234));

        
        bool alterou = cadastro.Alterar(1, "Mouse Gamer", 80m, 100);

        
        Assert.True(alterou);
        Produto produto = cadastro.BuscarPorCodigo(1)!;
        Assert.Equal("Mouse Gamer", produto.Nome);
        Assert.Equal(80m, produto.Preco);
        Assert.Equal(100, produto.Estoque);
    }

    [Fact]
    public void Remover_ProdutoExistente_NaoDeveMaisEstarNaLista()
    {
        
        var cadastro = new CadastroProduto();
        cadastro.Adicionar(new Produto(1, "Mouse", 35m, 234));

       
        bool removeu = cadastro.Remover(1);

        
        Assert.True(removeu);
        Assert.Empty(cadastro.Listar());
        Assert.Null(cadastro.BuscarPorCodigo(1));
    }

    [Fact]
    public void Adicionar_CodigoRepetido_DeveSerRecusado()
    {
        
        var cadastro = new CadastroProduto();
        cadastro.Adicionar(new Produto(1, "Mouse", 35m, 234));

        
        bool adicionou = cadastro.Adicionar(new Produto(1, "Teclado", 50m, 10));

        
        Assert.False(adicionou);
        Assert.Single(cadastro.Listar());
    }

    [Fact]
    public void Estoque_ValorNegativo_DeveLancarExcecao()
    {
        var produto = new Produto(1, "Mouse", 35m, 234);

        Assert.Throws<ArgumentException>(() => produto.Estoque = -1);
    }
}