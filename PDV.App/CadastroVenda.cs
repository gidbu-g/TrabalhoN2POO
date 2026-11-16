namespace PDV.App;

public class CadastroVenda
{
    private readonly CadastroProduto _cadastroProduto;
    private readonly List<Venda> _vendas = new();
    private int _proximoCodigo = 1;

    public CadastroVenda(CadastroProduto cadastroProduto)
    {
        _cadastroProduto = cadastroProduto;
    }

    public IReadOnlyList<Venda> Listar()
    {
        return _vendas;
    }

    public Venda? BuscarPorCodigo(int codigo)
    {
        return _vendas.FirstOrDefault(v => v.Codigo == codigo);
    }

    public Venda Criar()
    {
        var venda = new Venda(_proximoCodigo);
        _proximoCodigo++;
        _vendas.Add(venda);
        return venda;
    }

    public ResultadoItemVenda AdicionarItem(Venda venda, string codigoOuNome, int quantidade)
    {
        if (!venda.PodeSerAlterada)
            return ResultadoItemVenda.VendaNaoPodeSerAlterada;

        if (quantidade < 0)
            return ResultadoItemVenda.QuantidadeInvalida;

        Produto? produto = LocalizarProduto(codigoOuNome);
        if (produto == null)
            return ResultadoItemVenda.ProdutoNaoEncontrado;

        if (quantidade > produto.Estoque)
            return ResultadoItemVenda.EstoqueInsuficiente;

        venda.DefinirQuantidade(produto, quantidade);

        return quantidade == 0
            ? ResultadoItemVenda.ItemRemovido
            : ResultadoItemVenda.ItemDefinido;
    }

    public bool RemoverItem(Venda venda, int codigoProduto)
    {
        if (!venda.PodeSerAlterada)
            return false;

        return venda.RemoverItem(codigoProduto);
    }

    public bool PossuiEstoqueSuficiente(Venda venda)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            Produto? produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto);
            if (produto == null || produto.Estoque < item.Quantidade)
                return false;
        }
        return true;
    }

    public bool Pagar(Venda venda, IPagamento pagamento)
    {
        if (!venda.PodeReceberPagamento)
            return false;

        if (!PossuiEstoqueSuficiente(venda))
            return false;

        if (!pagamento.RealizarPagamento(venda))
            return false;

        foreach (ItemVenda item in venda.Itens)
        {
            Produto produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto)!;
            produto.Estoque -= item.Quantidade;
        }
        return true;
    }

    public bool Cancelar(Venda venda)
    {
        if (venda.Cancelada)
            return false;

        if (venda.Paga)
            DevolverEstoque(venda);

        venda.Cancelar();
        return true;
    }

    public void SalvarVendas(string caminho)
    {
        var linhas = new List<string>();

        foreach (Venda venda in _vendas)
        {
            linhas.Add(FormatarVenda(venda));

            foreach (ItemVenda item in venda.Itens)
                linhas.Add(FormatarItem(venda, item));
        }

        File.WriteAllLines(caminho, linhas);
    }

    public void CarregarVendas(string caminho)
    {
        if (!File.Exists(caminho))
            return;

        var carregadas = new List<Venda>();

        foreach (string linha in File.ReadLines(caminho))
        {
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            string[] campos = linha.Split(';');

            try
            {
                switch (campos[0])
                {
                    case "V":
                    {
                        Venda venda = LerVenda(campos);

                        if (carregadas.Any(v => v.Codigo == venda.Codigo))
                            throw new InvalidDataException($"Código de venda repetido no arquivo: {venda.Codigo}.");

                        carregadas.Add(venda);
                        break;
                    }
                    case "I":
                        LerItem(campos, carregadas);
                        break;
                    default:
                        throw new InvalidDataException($"Tipo de registro desconhecido: {linha}");
                }
            }
            catch (Exception erro) when (erro is FormatException or OverflowException)
            {
                throw new InvalidDataException($"Linha de venda inválida: {linha}", erro);
            }
        }

        _vendas.Clear();
        _vendas.AddRange(carregadas);
        _proximoCodigo = carregadas.Count == 0 ? 1 : carregadas.Max(v => v.Codigo) + 1;
    }

    private Produto? LocalizarProduto(string codigoOuNome)
    {
        Produto? produto = null;

        if (int.TryParse(codigoOuNome, out int codigo))
            produto = _cadastroProduto.BuscarPorCodigo(codigo);

        return produto ?? _cadastroProduto.BuscarPorNome(codigoOuNome);
    }

    private void DevolverEstoque(Venda venda)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            Produto? produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto);
            if (produto != null)
                produto.Estoque += item.Quantidade;
        }
    }

    private static string FormatarVenda(Venda venda)
    {
        DadosPagamento dados = venda.Pagamento ?? new DadosPagamento();

        return string.Join(";",
            "V",
            venda.Codigo,
            venda.Cancelada,
            dados.Forma,
            FormatoArquivo.Formatar(dados.ValorRecebido),
            FormatoArquivo.Formatar(dados.Troco),
            dados.QuantidadeParcelas,
            FormatoArquivo.Formatar(dados.ValorParcela));
    }

    private static string FormatarItem(Venda venda, ItemVenda item)
    {
        return string.Join(";",
            "I",
            venda.Codigo,
            item.CodigoProduto,
            item.NomeProduto,
            FormatoArquivo.Formatar(item.PrecoUnitario),
            item.Quantidade);
    }

    private static Venda LerVenda(string[] campos)
    {
        if (campos.Length != 8)
            throw new InvalidDataException("Registro de venda com número de campos incorreto.");

        var venda = new Venda(FormatoArquivo.LerInteiro(campos[1]));

        if (campos[3] != string.Empty)
        {
            venda.Pagamento = new DadosPagamento
            {
                Forma = campos[3],
                ValorRecebido = FormatoArquivo.LerDecimal(campos[4]),
                Troco = FormatoArquivo.LerDecimal(campos[5]),
                QuantidadeParcelas = FormatoArquivo.LerInteiro(campos[6]),
                ValorParcela = FormatoArquivo.LerDecimal(campos[7])
            };
        }

        if (bool.Parse(campos[2]))
            venda.Cancelar();

        return venda;
    }

    private static void LerItem(string[] campos, List<Venda> vendas)
    {
        if (campos.Length != 6)
            throw new InvalidDataException("Registro de item com número de campos incorreto.");

        int codigoVenda = FormatoArquivo.LerInteiro(campos[1]);
        Venda venda = vendas.FirstOrDefault(v => v.Codigo == codigoVenda)
            ?? throw new InvalidDataException($"Item pertence a uma venda inexistente: {codigoVenda}.");

        venda.CarregarItem(new ItemVenda(
            FormatoArquivo.LerInteiro(campos[2]),
            campos[3],
            FormatoArquivo.LerDecimal(campos[4]),
            FormatoArquivo.LerInteiro(campos[5])));
    }
}