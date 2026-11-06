using System;
using System.Globalization;
using System.Text;

namespace AlmDenik
{
    internal class Program
    {
        private const string VychoziSoubor = "denik.txt";

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Denik denik = new Denik();
            string zprava = "";
            bool bezi = true;

            while (bezi)
            {
                Vypis(denik);

                if (zprava != "")
                {
                    Console.WriteLine(zprava);
                    zprava = "";
                }

                Console.Write("> ");
                string prikaz = (Console.ReadLine() ?? "zavri").Trim().ToLowerInvariant();

                switch (prikaz)
                {
                    case "predchozi":
                        if (!denik.Dny.Predchozi()) zprava = "Už jste na prvním dni.";
                        break;

                    case "dalsi":
                        if (!denik.Dny.Dalsi()) zprava = "Už jste na posledním dni.";
                        break;

                    case "zacatek":
                        denik.Dny.NaZacatek();
                        break;

                    case "konec":
                        denik.Dny.NaKonec();
                        break;

                    case "novy":
                        zprava = NovyZaznam(denik);
                        break;

                    case "smaz":
                        zprava = SmazZaznam(denik);
                        break;

                    case "uloz":
                        zprava = Uloz(denik);
                        break;

                    case "nacti":
                        zprava = Nacti(denik);
                        break;

                    case "zavri":
                        bezi = false;
                        break;

                    default:
                        zprava = "Neznámá operace: \"" + prikaz + "\".";
                        break;
                }
            }
        }

        // ---------- Výpis ----------

        private static void Vypis(Denik denik)
        {
            Console.WriteLine();
            Console.WriteLine("==================== DENÍK ====================");

            Den den = denik.Dny.Aktualni;
            int pocetZaznamu = den == null ? 0 : den.Zaznamy.Pocet;

            Console.WriteLine("Počet dnů: " + denik.Dny.Pocet + "   |   Záznamů v aktuálním dni: " + pocetZaznamu);
            Console.WriteLine("------------------------------------------------");

            if (den == null)
            {
                Console.WriteLine("Deník je prázdný. Přidejte první záznam operací \"novy\".");
            }
            else
            {
                Console.WriteLine("Den: " + den.Datum.ToString(Denik.FormatData, CultureInfo.InvariantCulture));
                Console.WriteLine();
                VypisZaznamy(den);
            }

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Operace: predchozi | dalsi | zacatek | konec | novy | smaz | uloz | nacti | zavri");
        }

        private static void VypisZaznamy(Den den)
        {
            for (int i = 0; i < den.Zaznamy.Pocet; i++)
            {
                string[] radky = den.Zaznamy[i].Text.Split('\n');
                string cislo = (i + 1) + ") ";
                string odsazeni = new string(' ', cislo.Length);

                for (int r = 0; r < radky.Length; r++)
                {
                    Console.WriteLine((r == 0 ? cislo : odsazeni) + radky[r]);
                }
            }
        }

        // ---------- Operace ----------

        private static string NovyZaznam(Denik denik)
        {
            DateTime datum;
            while (true)
            {
                Console.Write("Datum (dd.MM.yyyy, prázdný řádek = zrušit): ");
                string vstup = (Console.ReadLine() ?? "").Trim();

                if (vstup == "") return "Zadávání zrušeno.";

                if (DateTime.TryParseExact(vstup, Denik.FormatData, CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out datum))
                {
                    break;
                }
                Console.WriteLine("Chybný formát data, zkuste to znovu.");
            }

            Console.WriteLine("Text záznamu (konec zadávání = prázdný řádek):");
            StringBuilder text = new StringBuilder();
            while (true)
            {
                string radek = Console.ReadLine() ?? "";
                if (radek == "") break;

                if (text.Length > 0) text.Append('\n');
                text.Append(radek);
            }

            if (text.Length == 0) return "Prázdný záznam nebyl uložen.";

            switch (denik.NovyZaznam(datum, text.ToString()))
            {
                case VysledekVlozeni.PridanoDoAktualnihoDne:
                    return "Záznam byl přidán do aktuálního dne.";
                case VysledekVlozeni.VytvorenNovyDen:
                    return "Byl vytvořen nový den a záznam byl přidán.";
                default:
                    return "Zápis odmítnut: datum musí být buď aktuální den, nebo ležet mezi aktuálním a následujícím dnem.";
            }
        }

        private static string SmazZaznam(Denik denik)
        {
            Den den = denik.Dny.Aktualni;
            if (den == null) return "Není co mazat, deník je prázdný.";

            Console.Write("Číslo záznamu ke smazání (1-" + den.Zaznamy.Pocet + "): ");
            string vstup = (Console.ReadLine() ?? "").Trim();

            if (!int.TryParse(vstup, out int cislo) || cislo < 1 || cislo > den.Zaznamy.Pocet)
                return "Neplatné číslo záznamu.";

            Console.WriteLine();
            Console.WriteLine("Vybraný záznam:");
            string[] radky = den.Zaznamy[cislo - 1].Text.Split('\n');
            for (int i = 0; i < radky.Length; i++)
            {
                Console.WriteLine("  " + radky[i]);
            }

            Console.Write("Opravdu smazat? (a/n): ");
            string odpoved = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (odpoved != "a") return "Mazání zrušeno.";

            denik.SmazZaznam(cislo - 1);
            return "Záznam byl smazán.";
        }

        private static string Uloz(Denik denik)
        {
            string cesta = ZeptejSeNaSoubor();
            try
            {
                denik.Uloz(cesta);
                return "Deník uložen do souboru " + cesta + ".";
            }
            catch (Exception e)
            {
                return "Uložení se nezdařilo: " + e.Message;
            }
        }

        private static string Nacti(Denik denik)
        {
            string cesta = ZeptejSeNaSoubor();
            try
            {
                denik.Nacti(cesta);
                return "Deník načten ze souboru " + cesta + ".";
            }
            catch (Exception e)
            {
                return "Načtení se nezdařilo: " + e.Message;
            }
        }

        private static string ZeptejSeNaSoubor()
        {
            Console.Write("Soubor (Enter = " + VychoziSoubor + "): ");
            string cesta = (Console.ReadLine() ?? "").Trim();
            return cesta == "" ? VychoziSoubor : cesta;
        }
    }
}
