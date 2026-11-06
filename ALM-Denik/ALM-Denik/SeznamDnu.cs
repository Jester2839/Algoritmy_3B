namespace AlmDenik
{
    /// <summary>
    /// Vlastní obousměrný spojový seznam dnů. Si pamatuje začátek, konec
    /// a "aktuální" den, na kterém se zrovna nachází uživatel.
    /// </summary>
    public class SeznamDnu
    {
        public Den Zacatek { get; private set; }
        public Den Konec { get; private set; }
        public Den Aktualni { get; private set; }
        public int Pocet { get; private set; }

        public bool JePrazdny => Zacatek == null;

        // ---------- Pohyb ----------

        /// <returns>true, pokud se přesunul; false, když už je na posledním dni.</returns>
        public bool Dalsi()
        {
            if (Aktualni == null || Aktualni.Dalsi == null) return false;
            Aktualni = Aktualni.Dalsi;
            return true;
        }

        /// <returns>true, pokud se přesunul; false, když už je na prvním dni.</returns>
        public bool Predchozi()
        {
            if (Aktualni == null || Aktualni.Predchozi == null) return false;
            Aktualni = Aktualni.Predchozi;
            return true;
        }

        public void NaZacatek()
        {
            Aktualni = Zacatek;
        }

        public void NaKonec()
        {
            Aktualni = Konec;
        }

        // ---------- Vkládání ----------

        /// <summary>Vloží nový den hned za aktuální a přesune se na něj.
        /// Je-li seznam prázdný, stane se z něj první a jediný den.</summary>
        public void VlozZaAktualni(Den novy)
        {
            if (Aktualni == null)
            {
                Zacatek = novy;
                Konec = novy;
                Aktualni = novy;
                Pocet = 1;
                return;
            }

            Den nasledujici = Aktualni.Dalsi;

            novy.Predchozi = Aktualni;
            novy.Dalsi = nasledujici;
            Aktualni.Dalsi = novy;

            if (nasledujici != null)
            {
                nasledujici.Predchozi = novy;
            }
            else
            {
                Konec = novy; // vkládali jsme za poslední den
            }

            Aktualni = novy;
            Pocet++;
        }

        /// <summary>Připojí den na konec (používá se při načítání ze souboru). Aktuální den nemění.</summary>
        public void PridejNaKonec(Den novy)
        {
            if (Konec == null)
            {
                Zacatek = novy;
                Konec = novy;
                Aktualni = novy;
                Pocet = 1;
                return;
            }

            Konec.Dalsi = novy;
            novy.Predchozi = Konec;
            Konec = novy;
            Pocet++;
        }

        // ---------- Mazání ----------

        /// <summary>Odebere aktuální den. Aktuální se stane následující,
        /// a když žádný není, tak předchozí. V prázdném seznamu je Aktualni null.</summary>
        public void OdeberAktualni()
        {
            if (Aktualni == null) return;

            Den odebirany = Aktualni;
            Den nasledujici = odebirany.Dalsi;
            Den predchozi = odebirany.Predchozi;

            if (predchozi != null) predchozi.Dalsi = nasledujici;
            else Zacatek = nasledujici;   // mazali jsme první den

            if (nasledujici != null) nasledujici.Predchozi = predchozi;
            else Konec = predchozi;       // mazali jsme poslední den

            Aktualni = nasledujici ?? predchozi;
            Pocet--;
        }
    }
}
