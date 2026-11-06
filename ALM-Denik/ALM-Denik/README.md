# Projekt ALM – Deník

Konzolový deník: dny ve **vlastním obousměrném spojovém seznamu** (`SeznamDnu`),
záznamy dne ve **vlastním seznamu nad polem pevné velikosti** (`SeznamZaznamu`).
Žádné hotové kolekce jazyka se nepoužívají.

## Jazyk a verze
- C# 12, .NET 10

## Spuštění
```
dotnet run
```
(ve složce s `AlmDenik.csproj`. Tedy `cd ALM-Denik` pak `dotnet run`)

## Operace
`predchozi`, `dalsi`, `zacatek`, `konec`, `novy`, `smaz`, `uloz`, `nacti`, `zavri`

## Struktura
| Soubor | Co obsahuje |
|---|---|
| `Zaznam.cs` | jeden záznam (víceřádkový text) |
| `SeznamZaznamu.cs` | vlastní List nad polem, zvětšování pole, mazání s posunem |
| `Den.cs` | datum + záznamy, uzel spojového seznamu |
| `SeznamDnu.cs` | obousměrný spojový seznam, pohyb, vkládání, mazání |
| `Denik.cs` | pravidla pro nový záznam, mazání, ukládání/načítání |
| `Program.cs` | konzolové rozhraní |

## Poznámky k chování
- V prázdném deníku se první záznam s libovolným datem založí jako první den.
- Datum nového záznamu: stejné jako aktuální den = přidá se na konec; mezi aktuálním
  a následujícím dnem = vznikne nový den a stane se aktuálním; jinak se zápis odmítne.
- Text záznamu se zadává po řádcích, ukončí se prázdným řádkem.
- Formát souboru je textový (UTF-8), popsaný v komentáři v `Denik.cs`.
