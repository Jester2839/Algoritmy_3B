using System;

namespace AlmDenik
{
    /// <summary>
    /// Jeden den = datum + seznam záznamů. Zároveň je to uzel obousměrného spojového seznamu.
    /// </summary>
    public class Den
    {
        public DateTime Datum { get; }
        public SeznamZaznamu Zaznamy { get; } = new SeznamZaznamu();

        // Odkazy na sousední dny (null = žádný soused).
        public Den Dalsi { get; set; }
        public Den Predchozi { get; set; }

        public Den(DateTime datum)
        {
            Datum = datum.Date;
        }
    }
}
