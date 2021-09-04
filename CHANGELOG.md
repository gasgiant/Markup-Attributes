# 0.4.0

_(2021-September-4)_

#### Features

* Option for the `TitleGroup` to not be underlined

#### Fixes

* Self referential inlined editors are now catched and disallowed.
* Sliders inside boxes in material editors now have (almost) proper width.
* Removed spaces before titles, when they are placed directly below the title of some other group. 


# 0.3.0

_(2021-August-31)_

#### Features

* UPM installation
* `ReadOnly` and `ReadOnlyGroup` attributes
* `HideIf`, `ShowIf`, `DisableIf`, `EnableIf` attributes (single target versions of the respective group attributes)
* Support for value comparison conditions in C# (useful for enum states)
* `MarkupGUI` API for creating Markup Attributes styled groups in custom inspectors and windows

#### Fixes

* Conditionals now correctly work inside structs.
* Inherited members are now visible for the system.
* Removed weird scaling on header textures. 
* Fields inside boxes are now aligned with the fields outside. 
* `ToggleGroup` now shows the tooltip of the source property.
* `DrawSystemProperties` now draws enable instancing checkbox.
* Fixed a bug, where label width was not reset correctly after the inlined material editor.

# 0.2.1

_(2021-August-9)_

#### Fixes

* Fixed a bug where header textures were missing on unscaled screens ([Issue](https://github.com/gasgiant/Markup-Attributes/issues/1)).

# 0.2.0 

_(2021-August-1)_

Initial release. 

