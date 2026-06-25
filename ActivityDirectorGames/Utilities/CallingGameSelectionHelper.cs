using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using ActivityDirectorGames.Views;

namespace ActivityDirectorGames.Utilities;

internal static class CallingGameSelectionHelper
{
    internal static void ClearYellowSelection(Control viewRoot)
    {
        ClearYellowSelection(viewRoot, _ => Brushes.Transparent);
    }

    internal static void ClearYellowSelection(
        Control viewRoot,
        Func<Label, IBrush> getRestoredBackground)
    {
        foreach (Label label in viewRoot.GetVisualDescendants().OfType<Label>())
        {
            if (label.FindAncestorOfType<Border>() is not Border border)
            {
                continue;
            }

            if (border.Background != Brushes.Yellow)
            {
                continue;
            }

            IBrush restoredBackground = getRestoredBackground(label);
            border.Background = restoredBackground;
            ApplyThemeVariant(label, restoredBackground != Brushes.Transparent);
            return;
        }
    }

    internal static IBrush GetBingoRestoredBackground(
        Label label,
        GameMode mode,
        IBrush? selectedBrush)
    {
        if (!int.TryParse(label.Content as string, out int number))
        {
            return Brushes.Transparent;
        }

        bool isOdd = number % 2 != 0;
        bool shouldBeMarked = (mode == GameMode.Odds && isOdd) || (mode == GameMode.Evens && !isOdd);
        return shouldBeMarked ? selectedBrush ?? Brushes.Transparent : Brushes.Transparent;
    }

    private static void ApplyThemeVariant(Label label, bool useLightTheme)
    {
        if (label.FindAncestorOfType<ThemeVariantScope>() is not ThemeVariantScope themeScope)
        {
            return;
        }

        themeScope.RequestedThemeVariant = useLightTheme
            ? ThemeVariant.Light
            : ThemeVariant.Default;
    }
}
