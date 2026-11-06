namespace AlmDenik
{
    /// <summary>
    /// Jeden záznam v deníku. Text může být víceřádkový (řádky jsou oddělené '\n').
    /// </summary>
    public class Zaznam
    {
        public string Text { get; }

        public Zaznam(string text)
        {
            Text = text;
        }
    }
}
