# Daily Bible Verse App

**Module:** 6002CEM Mobile App Development  
**Student:** Jerry Michael   
**Framework:** .NET MAUI 9 (C#)  
**Platforms:** Android, iOS, Windows

---

## Background & Motivation

Daily Bible Verse App is a personal scripture companion that helps users build a consistent devotional habit. Users can look up any verse from the full Bible using a live public API, save favourites, write personal notes, and receive daily push notification reminders.

The app was built with clean architectural principles — MVVM, Dependency Injection, and a local SQLite database — to demonstrate professional software design alongside a practical, faith-centred feature set.

---

## Features

| Feature | Description |
|---|---|
| **Verse Lookup** | Search any verse by book, chapter, verse and translation via bible-api.com |
| **Daily Verse** | Rotating daily verse — deterministic by day-of-year, same verse all day |
| **Favourites** | Save verses to SQLite; view, share, or delete from a dedicated tab |
| **Notes** | Full CRUD note-taking with optional verse linking and search |
| **Login / Register** | SQLite-backed auth with SHA-256 hashed passwords and session persistence |
| **Push Notifications** | Scheduled daily reminder at user-chosen time (Plugin.LocalNotification) |
| **Streak Tracker** | Tracks consecutive days and total verses read |
| **Settings** | Notification toggle, time picker, logout |
| **Share** | Native OS share sheet for any verse |
| **Dark Mode** | Full light/dark theme support via AppThemeBinding |

---

## Architecture

### MVVM Pattern
All business logic lives in ViewModels — Views contain **zero business logic** in code-behind. The `BaseViewModel` (inheriting `ObservableObject` from CommunityToolkit.Mvvm) is shared by all 5 ViewModels, following the DRY principle.

```
Views ──(binds)──▶ ViewModels ──(inject)──▶ Services ──(implement)──▶ Interfaces
```

### Dependency Injection / Inversion of Control
Configured in `MauiProgram.cs` — every service is registered against its interface so callers depend on abstractions, not concretions:

```csharp
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<IAuthService,     AuthService>();
builder.Services.AddSingleton<IBibleVerseService, BibleVerseService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
```

ViewModels and Pages are **Transient** (fresh instance per navigation):
```csharp
builder.Services.AddTransient<BibleVerseViewModel>();
builder.Services.AddTransient<NotesViewModel>();
// etc.
```

### Software Design Patterns
1. **Repository Pattern** — `DatabaseService` abstracts all SQLite access behind `IDatabaseService`
2. **Strategy Pattern** — Password hashing algorithm isolated in `AuthService.HashPassword()`
3. **Factory Pattern** — `BibleVerseService` creates `BibleVerse` objects from raw API JSON
4. **Singleton Pattern** — Services registered as singletons in the DI container
5. **Converter Pattern** — `InverseBoolConverter`, `TruncateTextConverter`, `BoolToColorConverter` etc.

### Folder Structure (Separation of Concerns)
```
DailyBibleVerseApp/
├── Models/         — Data models (BibleVerse, FavoriteVerse, Note, User, BibleApiResponse)
├── ViewModels/     — MVVM ViewModels (BaseViewModel + 5 concrete VMs)
├── Views/          — XAML pages (zero business logic in code-behind)
├── Services/       — Concrete implementations (DatabaseService, AuthService, etc.)
├── Interfaces/     — Service contracts (IDatabaseService, IAuthService, etc.)
├── Converters/     — IValueConverter implementations
├── Controls/       — Custom GoldButton control
├── Helpers/        — Pure utility functions (DateHelper)
└── Platforms/      — Platform-specific entry points
```

---

## Database (SQLite — Local + Cloud API)

### Local SQLite (3 tables, all CRUD)
| Table | Create | Read | Update | Delete |
|---|---|---|---|---|
| Users | Register | Login / AutoLogin | Streak update | — |
| FavoriteVerses | Save verse | List all | — | Delete / Clear all |
| Notes | Create note | List / Search | Edit note | Delete note |

### Cloud Integration (bible-api.com — Public REST API)
- **Endpoint 1:** `GET /{book}+{chapter}:{verse}?translation={version}` — specific verse lookup
- **Endpoint 2:** `GET /{curated-ref}` — daily verse (deterministic by day-of-year)
- **Endpoint 3:** `GET /{random-ref}` — random verse for "Load Another"
- **Endpoint 4:** Same endpoint, different translation param — translation switching

---

## UI Controls (Comprehensive Complexity — 8+ distinct controls)

1. `CollectionView` — Favourites list, Notes list
2. `TabBar` — 5 tabs: Bible, Daily, Favourites, Notes, Settings
3. `Picker` — Book selector (66 books), Translation selector
4. `Entry` — Book/chapter/verse inputs, login fields, note title, search
5. `Editor` — Note content (auto-resizing)
6. `Switch` — Notification toggle
7. `TimePicker` — Daily reminder time
8. `Border` — Cards throughout the app
9. `ActivityIndicator` — Loading states
10. `Grid` — Multi-column layouts
11. `ScrollView` — Scrollable content pages
12. **Custom Control** — `GoldButton` (composite ContentView with bindable properties)

---

## Setup

**Requirements:** Visual Studio 2022 v17.8+, .NET 9 SDK, MAUI workload

```bash
dotnet workload install maui
```

1. Open `DailyBibleVerseApp.sln` in Visual Studio
2. Select target: **Android Emulator** or **Windows Machine**
3. Press **F5**

---

*Built with .NET MAUI 9 · CommunityToolkit.Mvvm 8.4 · sqlite-net-pcl · Plugin.LocalNotification · bible-api.com*
