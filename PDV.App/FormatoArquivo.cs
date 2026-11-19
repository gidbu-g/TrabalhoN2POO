using System.Globalization;

namespace PDV.App;

public static class FormatoArquivo
{
    public static string Formatar(decimal valor)
    {
        return valor.ToString(CultureInfo.InvariantCulture);
    }

    public static decimal LerDecimal(string texto)
    {
        return decimal.Parse(texto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
    }

    public static int LerInteiro(string texto)
    {
        return int.Parse(texto, CultureInfo.InvariantCulture);
    }
}