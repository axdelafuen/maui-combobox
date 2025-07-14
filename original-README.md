# Maui.DropDown

Building a .net Maui application and I needed something that was not a picker 
and that would allow me to select data. 

There is no DropDown or ComboBox available in .net Maui 
as you would normally use the 'Picker' controls as this is normal for iOS, 
but the Picker does not seem to work well on MacCatalyst and there are 
issues with the current Picker implementation. 

So I built a simple DropDown control. Feel free to use as you see fit.

There is a base class called DropDownBoxBase which defines the behaviour and 
two derived classes, DropDownListBox and PopUpListBox which just set the style
of the list box. It is either **Popup** — which pops up an overlay or **DropDown**
which drops down a list. 

There is a current issue with the dropdown. It is constrained by it parent. 
So if the control is in a grid, then the dropdown absolute cannot expand outside
the bounds of the grid (which is why I support a PopUp). If someone can fix 
this, great. It would be good if it overlayed other controls on the page. 

Key points:

1. If you ar using an Object as the ItemsSource collection then it should implement ToString() to display the correct item.  
2. You need to copy or override the .svg images for the drop down images. 

To use this control, copy the code (sorry no NuGet) and add it to your Xaml:

```
<dropDown:PopUpListBox   
    ItemsSource="{Binding StringItems}"
    SelectedItem="{Binding SelectedStringItem}"
    Placeholder="Click for item"
/>
```

or:

```
<dropDown:DropDownListBox
    ItemsSource="{Binding StringItems}"
    SelectedItem="{Binding SelectedStringItem}"
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
    DropdownSeparator         : Show separators between items in the dropdown

