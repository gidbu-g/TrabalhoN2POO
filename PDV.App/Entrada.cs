namespace PDV.App;

public static class Entrada
{
    public static string LerTexto(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? texto = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(texto))
                return texto.Trim();

            Console.WriteLine("Valor obrigatório. Tente novamente.");
        }
    }

    public static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);

            if (int.TryParse(Console.ReadLine(), out int valor))
                return valor;

            Console.WriteLine("Número inteiro inválido. Tente novamente.");
        }
    }

    public static int LerInteiroComPadrao(string mensagem, int padrao)
    {
        while (true)
        {
            Console.Write($"{mensagem} [{padrao}]: ");
            string? texto = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(texto))
                return padrao;

            if (int.TryParse(texto, out int valor))
                return valor;

            Console.WriteLine("Número inteiro inválido. Tente novamente.");
        }
    }

    public static decimal LerDecimal(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);

            if (decimal.TryParse(Console.ReadLine(), out decimal valor))
                return valor;

            Console.WriteLine("Valor inválido. Tente novamente.");
        }
    }

    public static bool LerSimNao(string mensagem)
    {
        while (true)
        {
            Console.Write($"{mensagem} (s/n): ");
            string? resposta = Console.ReadLine()?.Trim().ToLower();

            if (resposta == "s")
                return true;
            if (resposta == "n")
                return false;

            Console.WriteLine("Responda com 's' ou 'n'.");
        }
    }
}