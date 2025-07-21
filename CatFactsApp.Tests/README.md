# Cat Facts App - Unit Tests 🧪

Kompleksowe testy jednostkowe dla aplikacji Cat Facts App, zapewniające wysoką jakość kodu i niezawodność.

## 📊 Pokrycie testami

### Testy serwisów
- **FileServiceTests** - 9 testów
  - Zapisywanie faktów do pliku
  - Obsługa błędów (null, puste wartości)
  - Walidacja argumentów konstruktora
  - Testowanie timestampów
  - Dopisywanie wielu faktów

- **CatFactServiceTests** - 16 testów
  - Pobieranie faktów z API
  - Obsługa błędów HTTP
  - Obsługa timeoutów
  - Obsługa błędów JSON
  - Walidacja odpowiedzi API
  - Testowanie różnych statusów HTTP

### Testy modeli
- **CatFactTests** - 10 testów
  - Inicjalizacja właściwości
  - Walidacja różnych długości faktów
  - Obsługa znaków specjalnych i Unicode
  - Testowanie wartości granicznych

## 🏃‍♂️ Uruchamianie testów

```bash
# Uruchomienie wszystkich testów
dotnet test

# Uruchomienie z szczegółowym outputem
dotnet test --verbosity normal

# Uruchomienie z pokryciem kodu
dotnet test --collect:"XPlat Code Coverage"
```

## 📈 Statystyki testów

- **Łączna liczba testów**: 35
- **Testy przeszły**: 35 ✅
- **Testy nie przeszły**: 0 ❌
- **Pominięte**: 0 ⏭️
- **Czas wykonania**: ~0.5s

## 🛠️ Technologie testowe

- **xUnit** - Framework do testów jednostkowych
- **Moq** - Framework do mockowania
- **Microsoft.Extensions.Http** - Testowanie HTTP klientów

## 🎯 Wzorce testowe

### AAA Pattern (Arrange-Act-Assert)
Wszystkie testy używają wzorca AAA dla czytelności:
```csharp
[Fact]
public async Task Method_Condition_ExpectedResult()
{
    // Arrange
    var input = new TestData();
    
    // Act
    var result = await service.Method(input);
    
    // Assert
    Assert.Equal(expected, result);
}
```

### Mockowanie zależności
```csharp
var mockLogger = new Mock<ILogger<Service>>();
var mockHttpHandler = new Mock<HttpMessageHandler>();
```

### Testowanie wyjątków
```csharp
Assert.Throws<ArgumentNullException>(() => new Service(null!));
```

## 📋 Kategorie testów

### Testy pozytywne ✅
- Sprawdzają poprawne działanie w normalnych warunkach
- Walidują oczekiwane wyniki

### Testy negatywne ❌
- Sprawdzają obsługę błędnych danych
- Testują edge cases i wyjątki

### Testy graniczne 🎯
- Testują wartości minimalne i maksymalne
- Sprawdzają zachowanie przy pustych danych

## 🔍 Przykłady testów

### Test serwisu z mockowaniem HTTP
```csharp
[Fact]
public async Task GetRandomCatFactAsync_WithSuccessfulResponse_ShouldReturnCatFact()
{
    // Arrange
    var expectedCatFact = new CatFact { Fact = "Test", Length = 4 };
    var jsonResponse = JsonSerializer.Serialize(expectedCatFact);
    var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
    };

    _mockHttpMessageHandler
        .Protected()
        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(httpResponse);

    // Act
    var result = await _catFactService.GetRandomCatFactAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedCatFact.Fact, result.Fact);
}
```

### Test z walidacją logowania
```csharp
private void VerifyLoggerWasCalled(LogLevel logLevel, string message)
{
    _mockLogger.Verify(
        x => x.Log(
            logLevel,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.AtLeastOnce);
}
```

## 🚀 Continuous Integration

Testy są gotowe do integracji z CI/CD pipeline:
- Automatyczne uruchamianie przy każdym commit
- Raportowanie pokrycia kodu
- Blokowanie merge przy nieudanych testach

---

**Uwaga**: Testy używają plików tymczasowych i są automatycznie czyszczone po wykonaniu.
