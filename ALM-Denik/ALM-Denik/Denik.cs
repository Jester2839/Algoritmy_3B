using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AlmDenik
{
    public enum VysledekVlozeni
    {
        PridanoDoAktualnihoDne,
        VytvorenNovyDen,
        Odmitnuto
    }

    /// <summary>
    /// Logika deníku (pravidla pro nový záznam, mazání, ukládání). Nezná konzoli.
    /// </summary>
    public class Denik
    {
        public const string FormatData = "dd.MM.yyyy";

        public SeznamDnu Dny { get; private set; } = new SeznamDnu();

        // ---------- Nový záznam ----------

        public VysledekVlozeni NovyZaznam(DateTime datum, string text)
        {
            datum = datum.Date;
            Zaznam zaznam = new Zaznam(text);

            // Prázdný deník: první záznam založí první den.
            if (Dny.JePrazdny)
            {
                Den prvni = new Den(datum);
                prvni.Zaznamy.Pridej(zaznam);
                Dny.VlozZaAktualni(prvni);
                return VysledekVlozeni.VytvorenNovyDen;
            }

            Den aktualni = Dny.Aktualni;

            // 1) Stejné datum jako aktuální den -> přidat na konec jeho záznamů.
            if (datum == aktualni.Datum)
            {
                aktualni.Zaznamy.Pridej(zaznam);
                return VysledekVlozeni.PridanoDoAktualnihoDne;
            }

            // 2) Datum leží mezi aktuálním a následujícím dnem -> nový den za aktuálním.
            bool poAktualnim = datum > aktualni.Datum;
            bool predNasledujicim = aktualni.Dalsi == null || datum < aktualni.Dalsi.Datum;

            if (poAktualnim && predNasledujicim)
            {
                Den novy = new Den(datum);
                novy.Zaznamy.Pridej(zaznam);
                Dny.VlozZaAktualni(novy);
                return VysledekVlozeni.VytvorenNovyDen;
            }

            // 3) Jinak odmítnout.
            return VysledekVlozeni.Odmitnuto;
        }

        // ---------- Mazání ----------

        /// <summary>Smaže záznam (index od 0) v aktuálním dni. Když den zůstane prázdný, zmizí.</summary>
        public void SmazZaznam(int index)
        {
            Den aktualni = Dny.Aktualni;
            aktualni.Zaznamy.OdeberNaIndexu(index);

            if (aktualni.Zaznamy.Pocet == 0)
            {
                Dny.OdeberAktualni();
            }
        }

        // ---------- Ukládání ----------
        //
        // Formát souboru (textový, UTF-8):
        //   <počet dnů>
        //   DEN <dd.MM.yyyy> <počet záznamů>
        //   ZAZNAM <počet řádků>
        //   <řádek 1>
        //   <řádek 2>
        //   ...

        public void Uloz(string cesta)
        {
            using (StreamWriter w = new StreamWriter(cesta, false, new UTF8Encoding(false)))
            {
                w.WriteLine(Dny.Pocet);

                Den den = Dny.Zacatek;
                while (den != null)
                {
                    string datum = den.Datum.ToString(FormatData, CultureInfo.InvariantCulture);
                    w.WriteLine("DEN " + datum + " " + den.Zaznamy.Pocet);

                    for (int i = 0; i < den.Zaznamy.Pocet; i++)
                    {
                        string[] radky = den.Zaznamy[i].Text.Split('\n');
                        w.WriteLine("ZAZNAM " + radky.Length);
                        for (int r = 0; r < radky.Length; r++)
                        {
                            w.WriteLine(radky[r]);
                        }
                    }

                    den = den.Dalsi;
                }
            }
        }

        /// <summary>Načte deník ze souboru. Při chybě vyhodí výjimku a původní deník zůstane beze změny.</summary>
        public void Nacti(string cesta)
        {
            SeznamDnu nove = new SeznamDnu();

            using (StreamReader r = new StreamReader(cesta, Encoding.UTF8))
            {
                int pocetDnu = int.Parse(CtiRadek(r));

                for (int d = 0; d < pocetDnu; d++)
                {
                    string[] hlavicka = CtiRadek(r).Split(' ');
                    if (hlavicka.Length != 3 || hlavicka[0] != "DEN")
                        throw new FormatException("Chybná hlavička dne.");

                    DateTime datum = DateTime.ParseExact(hlavicka[1], FormatData, CultureInfo.InvariantCulture);
                    int pocetZaznamu = int.Parse(hlavicka[2]);

                    if (pocetZaznamu <= 0)
                        throw new FormatException("Den bez záznamů.");
                    if (nove.Konec != null && datum <= nove.Konec.Datum)
                        throw new FormatException("Dny nejsou seřazené podle data.");

                    Den den = new Den(datum);

                    for (int z = 0; z < pocetZaznamu; z++)
                    {
                        string[] zh = CtiRadek(r).Split(' ');
                        if (zh.Length != 2 || zh[0] != "ZAZNAM")
                            throw new FormatException("Chybná hlavička záznamu.");

                        int pocetRadku = int.Parse(zh[1]);
                        StringBuilder text = new StringBuilder();
                        for (int i = 0; i < pocetRadku; i++)
                        {
                            if (i > 0) text.Append('\n');
                            text.Append(CtiRadek(r));
                        }

                        den.Zaznamy.Pridej(new Zaznam(text.ToString()));
                    }

                    nove.PridejNaKonec(den);
                }
            }

            nove.NaZacatek();
            Dny = nove;
        }

        private static string CtiRadek(StreamReader r)
        {
            string radek = r.ReadLine();
            if (radek == null)
                throw new FormatException("Neočekávaný konec souboru.");
            return radek;
        }
    }
}
