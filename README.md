# Maui.ComboBox

There is no * FREE * ComboBox available in .NET MAUI. 

Here is a simple, responsive and customizable ComboBox. This project is mostly inspired by the project : [Maui.DropDown](https://github.com/trevleyb/Maui.DropDown). (I felt that the inital project is most like a 'Picker' for the ComboBox and an 'Expander' for the 'DropDown', this is why I developped my own ComboBox).

## Getting started

Key points:

If you ar using an Object as the ItemsSource collection then it should implement ToString() to display the correct item.  

To use this control, copy the code (sorry no NuGet right now) and add it to your Xaml:

To use custom Open and Close icons, it is recommended to use SVGs for a better fit.

```xaml
xmlns:controls="clr-namespace:Namespace.Of.The.Control"
```

```xaml
<controls:ComboBox   
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