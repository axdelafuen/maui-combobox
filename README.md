# Maui.ComboBox

There is no * FREE * ComboBox available in .NET MAUI. (_fuck syncfusion, devexpress or telerik_).

Here is a simple, project for ComboBox implementation. Part of this project is inspired by: [Maui.DropDown by trevleyb](https://github.com/trevleyb/Maui.DropDown). It was initially a fork, but today it has nothing to do with the original project, which is why the repo has become standalone. 

There are currently 3 different ComboBox implementations. Each has its own pros and cons, here's a quick recap of what I've been working on:

| Implementation | Short description | Pros | Cons |
| - | - | - | - |
| Popup | With MAUI.Toolkit open a popup with CollectionView of the ItemsSource  | Works :), customizable, reponsive | Bad performances |
| Canva | Draw a canva with every item from ItemsSource | Lightweight, cutomizable | Bad responsivness (can move UI on open) |
| Grid | Use Grid and visibility behavior to show/hide the CollectionView of ItemsSource | _working on it_ | _working on it_ |

For now, it is recommended to use the `PopupComboBox`. Details on the use of other controls will be written once development are finalized.

## Samples

A very bad name was chosen by myself for the application allowing to make examples of use. However you can find examples of uses of ComboBoxes in: `./Maui.ComboBox.DebugApp/`.

## Getting started

Key points for `PopupComboBox`:

If you ar using an Object as the ItemsSource collection then it should implement ToString() to display the correct item.  

To use this control, copy the code (sorry no NuGet right now) and add it to your Xaml:

To use custom Open and Close icons, it is recommended to use SVGs for a better fit.

```xaml
xmlns:controls="clr-namespace:Namespace.Of.The.Control"
```

```xaml
<controls:PopupComboBox   
    ItemsSource="{Binding ItemsSource}"
    SelectedItem="{Binding SelectedItem}"
    Placeholder="Click for item"
/>
```

Other properties include:

    ItemsSource               : The source collection which can be a List<object>
    SelectedItem              : The item selected 
    Placeholder               : Text to display if SelectedItem is null
    TextColor                 : Color of Selected Item Text
    TextSize                  : Size of Selected Item Text
    DropDownWidth             : Width of the dropdown. If blank, will be the parent control size
    DropDownHeight            : Height of the dropdown
    DropdownCornerRadius      : Corner radius of the drop down. If blank will be square
    DropdownTextColor         : Color of the text in the dropdown
    DropdownBackgroundColor   : Background color of the dropdown
    DropdownBorderColor       : Border color of the dropdown
    DropdownBorderWidth       : Border width of the dropdown
    DropdownClosedImageSource : Image when dropdown is closed >
    DropdownOpenImageSource   : Image when dropdown is open V 
    DropdownImageTint         : Image Tint - if you overwrite background, this changes the image color
    DropdownShadow            : Draw a shadow on the dropdown

## Author

- [axdelafuen](https://github.com/axdelafuen)

## Acknowledgment

Thanks to [trevleyb](https://github.com/trevleyb) for his work that helped me a lot for starting this project.