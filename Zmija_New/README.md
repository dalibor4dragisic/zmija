# Zmija

Klasična igra „zmija” napravljena u Windows Forms-u (.NET 8). Zmijom može da upravlja igrač ili jedan od četiri autopilota, od kojih neki mogu da popune celu tablu bez sudara.

## Pokretanje

Potreban je [.NET 8 SDK](https://dotnet.microsoft.com/download) na Windows-u.

Iz foldera u kom je `Zmija_New.sln`:

```powershell
dotnet run --project Zmija_New\Zmija_New.csproj
```

U Visual Studiju ili u VS Code-u sa ekstenzijom **C# Dev Kit**: otvori `Zmija_New.sln`, za startni projekat izaberi `Zmija_New` i pritisni **F5**.

## Igra

- Tabla ima 18 × 18 polja. Ivice se spajaju: ko izađe levo, pojavi se desno; ko izađe gore, pojavi se dole.
- Zmija počinje sa 4 segmenta u gornjem levom uglu, a prva hrana je u centru table.
- Hrana se premešta na novo slobodno polje svakih 6 sekundi.
- Svaka pojedena hrana donosi 1 poen i jedan segment više. Poeni se vide u naslovu prozora.
- Svakih 5 poena zmija se ubrzava za 50 ms po potezu, od početnih 300 ms. Posle 50 ms brzina se vraća na 300 ms.
- Na 30 poena igra čestita na pobedi i nudi nastavak.
- Ako zmija popuni celu tablu, igra prijavljuje pobedu i nudi novu igru.
- Igra se završava kada zmija udari u samu sebe. Mesto sudara se oboji žuto-zeleno, a poruka prikazuje rezultat i nudi novu igru.

### Kontrole

| Taster | Akcija |
|---|---|
| Strelice | Upravljanje zmijom (samo u ručnom režimu) |
| SPACE | Pauza / nastavak |

### Meni

- **Opcije → Pokreni novu igru**: počinje igru ispočetka. Izabrani režim ostaje isti.
- **Opcije → Uputstvo**: kratko uputstvo.
- **Režim**: izbor načina kretanja (vidi ispod). Režim može da se promeni i usred igre.

## Režimi

| Režim | Opis |
|---|---|
| **Ručno igranje** | Igrač vodi zmiju strelicama. |
| **Automatsko - A\*** | Originalni autopilot: A* pretraga do hrane, bez spajanja ivica, celo telo je prepreka. Često se zaglavi i udari u sebe. |
| **Automatsko** (podrazumevano) | BFS pretraga sa spajanjem ivica i proverom bezbednosti: ide ka hrani samo ako posle toga može da stigne do svog repa. |
| **Automatsko - Hamiltonov ciklus** | Prati unapred zadatu putanju kroz sva polja, uz prečice ka hrani dok je zmija kratka. Ne može da se sudari. |
| **Automatsko - hibrid** | „Automatsko” dok zmija zauzima manje od 25% table, a zatim Hamiltonov ciklus. |

### Kako rade autopiloti

**Automatsko (BFS)** pre svakog poteza bira prvu od tri mogućnosti koja uspe:

1. **Hrana**: nađe najkraći put do hrane i simulira ga na kopiji zmije. Kreće tim putem samo ako iz buduće pozicije i dalje postoji put do repa.
2. **Rep**: kreće se ka sopstvenom repu, najdužim putem koji postoji, i tako dobija vreme dok se ne oslobodi prostor.
3. **Prostor**: bira susedno polje iz kog je dostupno najviše slobodnih polja.

Pretraga zna da se ivice spajaju i da se polja tela oslobađaju kako se zmija kreće: polje repa je slobodno posle 1 poteza, polje segmenta ispred njega posle 2 poteza, i tako dalje.

**Hamiltonov ciklus** je zatvorena putanja koja prolazi kroz svako polje table tačno jednom: prvi red sleva nadesno, zatim cik-cak kroz ostale redove i nazad gore kroz prvu kolonu. Dok zmija leži na ciklusu po redu, glava stalno juri rep po istoj putanji i ne može da ga stigne. Dok je bar pola table slobodno, glava sme da preskoči deo ciklusa ka hrani, ali samo ako i posle skoka ostaje dovoljno daleko iza repa.

Ako se na ciklus prelazi usred igre (hibrid, ili promena režima), zmija ulazi u ciklus tek kada su sva polja ispred nje sigurno slobodna. Do tada se kreće kao BFS autopilot.

### Rezultati simulacije

Simulacija van UI-ja, po pravilima igre, sa premeštanjem hrane svakih 20 poteza:

| Režim | Do 100 hrane (200 igara) | Do pune table (5 igara) |
|---|---|---|
| Automatsko - A\* | 1 uspešna, 199 sudara | — |
| Automatsko | 199 uspešnih, 1 sudar | — |
| Automatsko - Hamiltonov ciklus | 200 uspešnih | 5 uspešnih |
| Automatsko - hibrid | 200 uspešnih | 5 uspešnih, ~17% brže od Hamiltona |

## Struktura projekta

```
Zmija_New.sln
├── Zmija_New/                  aplikacija (WinForms, net8.0-windows)
│   ├── Form1.cs                tabla, tajmeri, meni, klase Snake / SnakeSegment / CustomPanel
│   ├── Form1.Designer.cs       meni i raspored kontrola
│   ├── IAutopilot.cs           interfejs autopilota i enum GameMode
│   ├── Autopilot.cs            BFS autopilot (hrana → rep → prostor)
│   ├── AStarAutopilot.cs       originalni A* autopilot
│   ├── HamiltonAutopilot.cs    Hamiltonov ciklus sa prečicama
│   └── HybridAutopilot.cs      BFS, pa Hamiltonov ciklus
└── AStar/                      biblioteka za A* pretragu (koristi je samo AStarAutopilot)
```

Autopiloti ne zavise od UI-ja. Zmiju dobijaju kao listu tačaka (`List<Point>`, glava prva, rep poslednji) i vraćaju samo sledeći smer kretanja. `Snake.MoveSnakeAuto` zatim pomera segmente i boji polja.

Novi autopilot se dodaje tako što se napravi klasa koja implementira `IAutopilot`, doda vrednost u `GameMode`, stavka u meni `Režim` (u `Form1.Designer.cs`, sa `Click` handlerom `rezimToolStripMenuItem_Click`) i slučaj u `Form1.CreateAutopilot`.

### Podešavanja

| Konstanta | Gde | Vrednost |
|---|---|---|
| `columns`, `rows` | `Form1` | 18 × 18 (Hamiltonov ciklus traži paran broj redova) |
| `startSegments` | `Form1` | 4 |
| `foodLimit` | `Form1` | 30 |
| `SwitchToHamiltonAt` | `HybridAutopilot` | 0.25 (deo table posle kog hibrid prelazi na ciklus) |
| `safetyMargin` | `HamiltonAutopilot` | 3 (slobodna polja između glave i repa pri prečici) |

## Poznati problemi

- Pauza (SPACE) vraća brzinu na početnih 300 ms.
- Ako se posle 30 poena na pitanje o nastavku odgovori sa „No”, igra ostaje zaustavljena. Nastavlja se tek preko **Pokreni novu igru**.
- A* režim namerno zadržava originalno ponašanje, uključujući nepreciznu heuristiku oko hrane. Ispravljena su samo rušenja.
