using System;

namespace AlmDenik
{
    /// <summary>
    /// Vlastní seznam (List) postavený nad polem pevné velikosti.
    /// Když se pole zaplní, vytvoří se pole dvakrát větší a prvky se do něj přepíšou.
    /// Počet záznamů tedy není omezený.
    /// </summary>
    public class SeznamZaznamu
    {
        private const int PocatecniKapacita = 4;

        private Zaznam[] pole = new Zaznam[PocatecniKapacita];

        /// <summary>Kolik prvků je v seznamu (ne kapacita pole!).</summary>
        public int Pocet { get; private set; }

        /// <summary>Přístup podle indexu (číslováno od 0).</summary>
        public Zaznam this[int index]
        {
            get
            {
                KontrolaIndexu(index);
                return pole[index];
            }
        }

        /// <summary>Přidá záznam na konec.</summary>
        public void Pridej(Zaznam zaznam)
        {
            if (Pocet == pole.Length)
            {
                Zvetsi();
            }
            pole[Pocet] = zaznam;
            Pocet++;
        }

        /// <summary>Odebere záznam na indexu a posune všechny další o jedno doleva.</summary>
        public Zaznam OdeberNaIndexu(int index)
        {
            KontrolaIndexu(index);

            Zaznam odebrany = pole[index];

            for (int i = index; i < Pocet - 1; i++)
            {
                pole[i] = pole[i + 1];
            }

            Pocet--;
            pole[Pocet] = null; // uvolnit referenci na poslední (už nepoužitou) pozici

            return odebrany;
        }

        private void Zvetsi()
        {
            Zaznam[] nove = new Zaznam[pole.Length * 2];
            for (int i = 0; i < Pocet; i++)
            {
                nove[i] = pole[i];
            }
            pole = nove;
        }

        private void KontrolaIndexu(int index)
        {
            if (index < 0 || index >= Pocet)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
