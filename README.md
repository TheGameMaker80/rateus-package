
# 📚 Rate App Popup Handler

A lightweight Unity package that controls **when** and **how often** the "Rate Us" popup should appear.

---

## 💠 Installation

### 1. Add the package to your project

#### Option A: Add to `manifest.json`

1. Go to your project’s root directory (where the `Assets` folder is located).
2. Open the `Packages` folder.
3. Open `manifest.json`.
4. Add the following line inside the `"dependencies"` block:

```json
"com.alexnikolaou.rateus": "https://github.com/TheGameMaker80/rateus-package.git"
```

---

## 🧩 Usage

### 2. Initialize `RateUsManager`

In your Unity C# script, initialize the manager before using it:

```csharp
using alexnikolaou.RateUs;

...

RateUsManager.Instance.Initialize((success, message) =>
{
    if (success)
    {
        // Manager successfully initialized
    }
    else
    {
        // Failed to initialize the manager
    }
});
```

---

### 3. (Optional) Set up custom configuration

You can inject your own config using a custom handler.

#### Step A: Create a class that inherits from `RateUsConfigHandler`

```csharp
public class CustomRateUsConfig : RateUsConfigHandler
{
    public CustomRateUsConfig(string json)
    {
        configJson = json;
    }
}
```

> 🔧 Replace `CustomRateUsConfig` with your actual class name.

#### Step B: Inject config **before** initialization

```csharp
RateUsManager.Instance.configHandler = new CustomRateUsConfig(theJson);
```

> `theJson` must be a stringified JSON that includes the following minimum config:

```json
{
  "rateFirstSessionWins": 5,
  "rateMaxTimesShownInSession": 2,
  "rateMaxTimesShownInVersion": 3,
  "rateNotFirstSessionWinsMod": 4
}
```

---

### 4. Trigger the popup check

Call this method after the user wins or completes a key action:

```csharp
RateUsManager.Instance.CheckForShowingRateUsOnWin((succeed) =>
{
    if (succeed)
    {
        // Show the "Rate Us" popup
    }
    else
    {
        // Conditions not met — do not show popup
    }
});
```

---

## 🙌 Contributions

Pull requests and feedback are welcome! Feel free to fork the repo and contribute.

---

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
