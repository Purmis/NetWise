# NetWise - Cat Facts Application Suite 🐱

**Kompletny pakiet aplikacji do zarządzania faktami o kotach** - od prostej aplikacji konsolowej po zaawansowaną aplikację webową z pełną architekturą enterprise.

## 📦 Zawartość projektu

### 1. **CatFactsApp** - Aplikacja konsolowa
Profesjonalna aplikacja konsolowa z zaawansowanym logowaniem i obsługą błędów.

### 2. **CatFactsWebApp** - Aplikacja webowa (główna)
Zaawansowana, responsywna aplikacja webowa ASP.NET Core z czystą architekturą zgodną z zasadami SOLID.

### 3. **CatFactsApp.Tests** - Testy jednostkowe
Kompleksowy pakiet 35 testów jednostkowych pokrywających wszystkie komponenty.

---

# 🌐 Cat Facts Web App (Główna aplikacja)

Zaawansowana, responsywna aplikacja webowa ASP.NET Core do pobierania, wyświetlania i zarządzania faktami o kotach z czystą architekturą zgodną z zasadami SOLID.

## 🌟 Funkcjonalności

### Podstawowe
- **Pobieranie faktów**: Automatyczne pobieranie faktów o kotach z API catfact.ninja
- **Responsywny interfejs**: Piękny, nowoczesny UI dostosowany do różnych rozmiarów ekranów
- **Zarządzanie faktami**: Przeglądanie, ocenianie, dodawanie do ulubionych
- **Filtrowanie**: Zaawansowane filtry według kategorii, oceny, daty, tekstu
- **Eksport**: Eksport faktów do pliku tekstowego z możliwością filtrowania

### Zaawansowane
- **Statystyki**: Wyświetlanie statystyk faktów (liczba, średnia ocena)
- **Ulubione**: System ulubionych faktów z szybkim dostępem
- **Ocenianie**: System ocen 1-5 gwiazdek dla każdego faktu
- **Historia**: Przeglądanie wszystkich pobranych faktów z datami
- **AJAX**: Interaktywne operacje bez przeładowywania strony

## 🏗️ Architektura

Aplikacja została zaprojektowana zgodnie z zasadami **SOLID** i wykorzystuje wzorzec **Clean Architecture**:

### Warstwy aplikacji:
1. **Domain Layer** (`Models/Domain/`) - Modele domenowe
2. **Data Layer** (`Data/`, `Repositories/`) - Dostęp do danych z Entity Framework Core
3. **Business Layer** (`Services/`) - Logika biznesowa i usługi
4. **Presentation Layer** (`Controllers/`, `Views/`) - Kontrolery MVC i widoki Razor

### Wzorce projektowe:
- **Repository Pattern** - Abstrakcja dostępu do danych
- **Dependency Injection** - Luźne powiązania między komponentami
- **Service Layer** - Enkapsulacja logiki biznesowej
- **ViewModel Pattern** - Separacja modeli widoku od modeli domenowych

## 🛠️ Technologie

- **Framework**: ASP.NET Core 9.0 MVC
- **Język**: C# 12
- **Baza danych**: Entity Framework Core InMemory
- **Frontend**: Bootstrap 5.3, Font Awesome 6, jQuery
- **API**: RESTful endpoints + AJAX
- **Architektura**: Clean Architecture, SOLID principles

## 📦 Wymagania

- .NET 9.0 SDK lub nowszy
- Połączenie internetowe (do pobierania faktów z API)

## 🚀 Instalacja i uruchomienie

1. **Klonowanie repozytorium**:
```bash
git clone [URL_REPOZYTORIUM]
cd CatFactsWebApp
```

2. **Przywrócenie pakietów**:
```bash
dotnet restore
```

3. **Budowanie aplikacji**:
```bash
dotnet build
```

4. **Uruchomienie aplikacji**:
```bash
dotnet run
```

5. **Otwórz przeglądarkę** i przejdź do: `http://localhost:5005`

## 📱 Responsywność

Aplikacja została zaprojektowana z podejściem **mobile-first** i jest w pełni responsywna:

- **Desktop** (1200px+): Pełny interfejs z wszystkimi funkcjami
- **Tablet** (768px-1199px): Dostosowany layout z zachowaniem funkcjonalności
- **Mobile** (320px-767px): Zoptymalizowany interfejs dotykowy

## 🎨 Interfejs użytkownika

### Strona główna (`/`)
- Hero section z wprowadzeniem
- Statystyki aplikacji
- Najnowsze fakty
- Ulubione fakty
- Interaktywne przyciski (Pobierz nowy fakt, Losuj 5 faktów)

### Przeglądanie (`/Home/Browse`)
- Zaawansowane filtry
- Lista wszystkich faktów
- Operacje na faktach (ocena, ulubione, usuwanie)
- Eksport do pliku

### O aplikacji (`/Home/About`)
- Opis funkcjonalności
- Informacje o technologiach
- Zasady SOLID w praktyce

## 🔧 Konfiguracja

### Usługi (Program.cs)
```csharp
// Entity Framework InMemory
builder.Services.AddDbContext<CatFactsDbContext>(options =>
    options.UseInMemoryDatabase("CatFactsDb"));

// HTTP Client dla API
builder.Services.AddHttpClient<ICatFactService, CatFactService>();

// Dependency Injection
builder.Services.AddScoped<ICatFactRepository, CatFactRepository>();
builder.Services.AddScoped<ICatFactBusinessService, CatFactBusinessService>();
```

### Baza danych
Aplikacja używa **Entity Framework Core InMemory** z przykładowymi danymi:
- Automatyczne tworzenie bazy przy starcie
- Dane przykładowe (2 fakty) dla demonstracji
- Pełna funkcjonalność CRUD

## 📊 API Endpoints

### Publiczne endpointy:
- `GET /` - Strona główna
- `GET /Home/Browse` - Przeglądanie faktów
- `GET /Home/About` - O aplikacji

### AJAX API:
- `POST /Home/FetchFact` - Pobierz nowy fakt
- `POST /Home/FetchMultipleFacts` - Pobierz wiele faktów
- `POST /Home/ToggleFavorite` - Przełącz ulubiony
- `POST /Home/RateFact` - Oceń fakt
- `POST /Home/DeleteFact` - Usuń fakt
- `GET /Home/ExportFacts` - Eksportuj fakty

## 🧪 Testowanie

### Testowanie responsywności:
1. Otwórz narzędzia deweloperskie (F12)
2. Przełącz na widok urządzeń mobilnych
3. Przetestuj różne rozdzielczości:
   - iPhone (375px)
   - iPad (768px)
   - Desktop (1200px+)

### Testowanie funkcjonalności:
1. **Pobieranie faktów**: Kliknij "Pobierz nowy fakt"
2. **Ocenianie**: Kliknij gwiazdki przy faktach
3. **Ulubione**: Kliknij ikonę serca
4. **Filtrowanie**: Użyj filtrów w sekcji Browse
5. **Eksport**: Pobierz fakty do pliku txt

## 📁 Struktura projektu

```
CatFactsWebApp/
├── Controllers/           # Kontrolery MVC
├── Data/                 # DbContext i konfiguracja EF
├── Models/
│   ├── Domain/          # Modele domenowe
│   └── ViewModels/      # Modele widoków
├── Repositories/        # Wzorzec Repository
├── Services/           # Logika biznesowa i API
├── Views/              # Widoki Razor
│   ├── Home/          # Widoki strony głównej
│   └── Shared/        # Layout i komponenty
├── wwwroot/           # Pliki statyczne
└── Program.cs         # Konfiguracja aplikacji
```

## 🔒 Bezpieczeństwo

- **Walidacja danych**: Wszystkie dane wejściowe są walidowane
- **Error handling**: Obsługa błędów na wszystkich poziomach
- **Logging**: Szczegółowe logowanie operacji
- **HTTPS**: Wsparcie dla bezpiecznych połączeń

## 🚀 Wdrożenie

Aplikacja jest gotowa do wdrożenia na:
- **Azure App Service**
- **IIS**
- **Docker**
- **Heroku** (z odpowiednią konfiguracją)

## 👥 Autor

Projekt stworzony jako demonstracja zaawansowanej aplikacji webowej ASP.NET Core z czystą architekturą i zasadami SOLID.

## 📄 Licencja

Ten projekt jest udostępniony na licencji MIT.

---

**Uwaga**: Aplikacja używa publicznego API catfact.ninja. W przypadku problemów z dostępnością API, aplikacja wyświetli odpowiednie komunikaty błędów i będzie działać z danymi przykładowymi.
