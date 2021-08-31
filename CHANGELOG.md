# 0.3.0

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

#### Fixes

* Fixed a bug where header textures were missing on unscaled screens ([Issue](https://github.com/gasgiant/Markup-Attributes/issues/1)).

# 0.2.0 

Initial release. 