# Icons Usage [Projektanker.Icons.Avalonia, MaterialDesign]

Purpose: Document how icons are used in this app, how to register providers, and how to bind icons in XAML and code.

## Install packages
Add these NuGet packages to the core app project:
- Projektanker.Icons.Avalonia
- Projektanker.Icons.Avalonia.MaterialDesign

## Provider Registration (App startup)
Register the MaterialDesign icon provider in App.axaml.cs:

```csharp
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;

// ... inside OnFrameworkInitializationCompleted
IconProvider.Current.Register<MaterialDesignIconProvider>();
```

This enables the `mdi:` icon scheme for the Projektanker Icon control.

## XAML Namespace
Add the Projektanker Icons namespace to any XAML where you want to use icons:

```xml
xmlns:ia="clr-namespace:Projektanker.Icons.Avalonia;assembly=Projektanker.Icons.Avalonia"
```

## Basic Usage
Use the Icon control and provide an icon key string via the `Value` property:

```xml
<ia:Icon Value="mdi:information-outline" Width="20" Height="20"/>
```

- Size is controlled by Width/Height.
- Color is controlled by `Foreground`.

## Reliable sizing with Viewbox (recommended)
Some layouts or control templates can constrain vector icon sizing. Wrap the icon in a Viewbox to force uniform scaling:

```xml
<Viewbox Width="40" Height="40" Stretch="Uniform">
  <ia:Icon Value="{Binding IconKindName}" Foreground="{Binding IconBrush}"/>
</Viewbox>
```

Adjust the Viewbox Width/Height to scale the icon consistently.

## Binding to ViewModel
Expose a string property (e.g., `IconKindName`) and a Brush (e.g., `IconBrush`) and bind them:

```xml
<ia:Icon Value="{Binding IconKindName}" Foreground="{Binding IconBrush}" Width="24" Height="24"/>
```

Where the ViewModel maps severity to icon ids and brushes:

```csharp
public string IconKindName => errorType switch
{
    ErrorType.Info => "mdi:information-outline",
    ErrorType.Warning => "mdi:alert-outline",
    ErrorType.Error => "mdi:alert-circle",
    ErrorType.Critical => "mdi:alert-octagon",
    _ => "mdi:alert-circle"
};

public IBrush IconBrush => errorType switch
{
    ErrorType.Info => new SolidColorBrush(Color.Parse("#2563EB")),
    ErrorType.Warning => new SolidColorBrush(Color.Parse("#F59E0B")),
    ErrorType.Error => new SolidColorBrush(Color.Parse("#DC2626")),
    ErrorType.Critical => new SolidColorBrush(Color.Parse("#7F1D1D")),
    _ => new SolidColorBrush(Color.Parse("#DC2626"))
};
```

## ExceptionDialog example
Header section shows the severity icon and title. Uses Viewbox to scale the icon reliably:

```xml
<Grid ColumnDefinitions="Auto,*" VerticalAlignment="Center">
  <Viewbox Grid.Column="0" Width="40" Height="40" Stretch="Uniform">
    <ia:Icon Value="{Binding IconKindName}" Foreground="{Binding IconBrush}"/>
  </Viewbox>
  <TextBlock Grid.Column="1" Margin="12,0,0,0"
             Text="{Binding Title}" FontSize="18" FontWeight="SemiBold"/>
</Grid>
```

## Common Packs and Keys
- Pack: MaterialDesign (registered with `MaterialDesignIconProvider`)
- Prefix: `mdi:`
- Examples:
  - `mdi:information-outline`
  - `mdi:alert-outline`
  - `mdi:alert-circle`
  - `mdi:alert-octagon`

Refer to the Projektanker.Icons.Avalonia.MaterialDesign package for the full set.

## Troubleshooting
- Icon not visible or blank:
  - Ensure provider registration is called at startup: `IconProvider.Current.Register<MaterialDesignIconProvider>()`.
  - Verify the XAML namespace: `xmlns:ia="clr-namespace:Projektanker.Icons.Avalonia;assembly=Projektanker.Icons.Avalonia"`.
  - Make sure the `Value` has the proper `mdi:` prefix.
- Icon size not changing:
  - Wrap the icon in a `Viewbox` with explicit Width/Height and `Stretch="Uniform"`.
- Using other packs:
  - Install the corresponding `Projektanker.Icons.Avalonia.<Pack>` package and register its provider at startup.

## Notes
- MahApps.Metro.IconPacks (WPF) is not compatible with Avalonia.
- Some icon packs require explicit provider registration at startup; MaterialDesign does.
- Prefer Viewbox-based scaling for consistent results across templates.
