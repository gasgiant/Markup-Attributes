# Markup Attributes

A Unity Editor extension for customizing inspector layout with attributes.

#### Key Features

* Create tabs, boxes, foldouts and other groups, hide/disable properties on condition, inline editors.
* Works in C# and in ShaderLab. 
* The inspector allows to inject custom code into it before/after/instead of any property.
* MIT licensed, small, opt-in. 

## Why?

Anyone who wrote custom editors knows, that they are prone to boilerplate and often rely on hardcoded names of the properties, which adds unnecessary friction to development.  One way to deal with it is to use C# Attributes, and the most prominent project to do so is [Odin Inspector](https://odininspector.com/). It is great, but it can't be used in open source and on the Asset Store because it's paid and huge. Also, as far as know, it doesn't do anything for shader editors, which drag with them even more boilerplate and bookkeeping that the regular ones. So, here is my take on the problem. 

Markup Attributes is MIT licensed and relatively small, focusing exclusively on editor layout. It works both in C# and in ShaderLab. Custom inspector provides hooks at any of the properties, which makes it possible to extend the inspector without loosing the layout functionality. 

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
   * [MonoBehaviour and ScriptableObject](#monobehaviour-and-scriptableobject)
   * [Serializable classes and structs](#serializable-classes-and-structs)
   * [Shaders](#shaders)
   * [Nesting Groups](#nesting-groups)
   * [ShaderLab Specifics](#shaderlab-specifics)
3. [Layout Attributes](#layout-attributes)
   * [EndGroup](#endgroup)
   * [Box](#box)
   * [TitleGroup](#titlegroup)
   * [Foldout](#foldout)
   * [TabScope and Tab](#tabscope-and-tab)
   * [HorizontalGroup and VerticalGroup](#horizontalgroup-and-verticalgroup)
   * [HideIf, ShowIf, DisableIf, EnableIf](#conditionals)
   * [ToggleGroup](#togglegroup)
4. [Special Attributes](#special-attributes)
   * [MarkedUpType](#markeduptype)
   * [MarkedUpField](#markedupfield)
   * [InlineEditor](#inlineeditor)
   * [DrawSystemProperties](#drawsystemproperties)
5. [Custom Marked Up Inspectors](#custom-marked-up-inspectors)
6. [MarkupGUI](#markupgui)

## Installation

You can install MarkupAttributes with [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html). Git URL: 

`https://github.com/gasgiant/Markup-Attributes.git#upm`

## Usage

### MonoBehaviour and ScriptableObject

For `MonoBehaviour`s and `ScriptableObject`s you need to create a custom editor that inherits form `MarkedUpEditor`:

```c#
using UnityEditor;
using MarkupAttributes;

[CustomEditor(typeof(MyComponent)), CanEditMultipleObjects]
internal class MyComponentEditor : MarkedUpEditor
{        
}
```

Alternatively, you can apply `MarkedUpEditor` to all classes that inherit from `MonoBehaviour` or `ScriptableObject` and don't have their own custom editor:

```c#
[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
internal class MarkedUpMonoBehaviourEditor : MarkedUpEditor
{
}

[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
internal class MarkedUpScriptableObjectEditor : MarkedUpEditor
{
}
```

### Serializable classes and structs

To make the attributes work inside serialized classes or structs you can add `MarkedUpType` to their definition or add `MarkedUpField` to the fields representing them:

```c#
[MarkedUpType]
[System.Serializable]
struct MyStruct
{
    ...
}

[System.Serializable]
class MyClass
{
    ...
}

class MyComponent : MonoBehaviour
{
    public MyStruct myStruct;

    [MarkedUpField]
    public MyClass myClass;
}
```

Note, that the attributes will work in marked up types only inside `MarkedUpEditor`. Currently marked up types are not supported inside arrays and lists. You can nest marked up types inside other marked up types.

### Shaders

To apply attributes to the materials with a certain `Shader` you should tell Unity to use `MarkedUpShaderGUI`:

```javascript
Shader "Unlit/MyShader"
{
    Properties
    {
        ...
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"
    
    ...
}
```

### Nesting Groups

Any group attribute requires a path in group hierarchy. The last entry in the path is the name of the group.

```c#
[Box("Group")]
public int one;
[TitleGroup("Group/Nested Group")]
public int two;
public int three;
```

![](./ReadmeImages/NestingSample_1.png)

Starting a group closes all groups untill a path match.

```c#
[Box("Group")]
public int one;
[TitleGroup("Group/Nested Group 1")]
public int two;
public int three;
[TitleGroup("Group/Nested Group 2")]
public int four;
public int five;
```

![](./ReadmeImages/NestingSample_2.png)

`./` shortcut opens a group on top of the current one, `../` closes the topmost group and then opens a new one on top. 

```c#
[Box("Group")]
public int one;
[TitleGroup("./Nested Group 1")]
public int two;
public int three;
[TitleGroup("../Nested Group 2")]
public int four;
public int five;
```

`EndGroup` closes the topmost group, or, when provided with a name, closes the named group and all of its children. 

```c#
[Box("Group")]
public int one;
[Box("./Nested Group")]
public int two;
public int three;
[EndGroup("Group")]

public int four;
public int five;
```

![](./ReadmeImages/NestingSample_3.png)

### ShaderLab Specifics

Unfortunately, ShaderLab does not allow any special symbols in property attributes. Because of that, we can't use `/` to write paths and have to replace them with spaces. Underscores then mark were you want actual spaces to be. Also, unlike in C#, you should not use quotes around the strings. For example, instead of

`[Box("Parent Group/My Box")]` 

you would write 

`[Box(Parent_Group My_Box)]`.

The same goes for shortcuts, so 

`[Box("./My Box")]` and `[Box("../My Box")]`

becomes

 `[Box(. MyBox)]` and `[Box(.. My_Box)]`.

## Layout Attributes

### EndGroup

Closes the topmost group. If provided with a name, closes the named group and all its children.

### Box

Starts a vertical group in a box. 


![](./ReadmeImages/BoxSample.png)

| Parameter        | Description                                                |
| :--------------- | ---------------------------------------------------------- |
| __string__ path  | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __bool__ labeled | Adds a label to the box. Default: _true_.                  |
| __float__ space  | Adds space before the group. Default: _0_.                 |

```c#
// C#
[Box("Labeled Box")]
public int one;
public int two;
public int three;

[Box("Unlabeled Box", labeled: false)]
public int four;
public int five;
public int six;
```

```javascript
// ShaderLab
[Box(Labeled_Box)]
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[Box(Unlabeled_Box, false)]
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0
```

### TitleGroup

Starts a vertical group with a title. 

![](./ReadmeImages/TitleGroupSample.png)

| Parameter           | Description                                                |
| ------------------- | ---------------------------------------------------------- |
| __string__ path     | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __bool__ contentBox | Adds a box around the group content. Default: _false_.     |
| __bool__ underline  | Underlines the title. Default: _true_.                     |
| __float__ space     | Adds space before the group. Default: _3_.                 |

```c#
// C#
[TitleGroup("Title Group")]
public int one;
public int two;
public int three;

[TitleGroup("Title Group With A Content Box", contentBox: true)]
public int four;
public int five;
public int six;
```

```javascript
// ShaderLab
[TitleGroup(Title_Group)]
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[TitleGroup(Title_Group_With_A_Box, true)]
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0
```

### Foldout

Starts a collapsible vertical group. 

![](./ReadmeImages/FoldoutSample.png)

| Parameter       | Description                                                |
| --------------- | ---------------------------------------------------------- |
| __string__ path | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __bool__ box    | Puts the foldout inside a box. Default: true.              |
| __float__ space | Adds space before the group. Default: _0_.                 |

```c#
// C#
[Foldout("Foldout In A Box")]
public int one;
public int two;
public int three;

[Foldout("Foldout", box: false)]
public int four;
public int five;
public int six;
```

```javascript
// ShaderLab
[Foldout(Foldout_In_A_Box)]
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[Foldout(Foldout, false)]
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0
```

### TabScope and Tab

`TabScope` creates a control for switching tabs.  `Tab` starts a group placed on a specified page. Names of the pages must match the names defined in `TabScope`. 

![](./ReadmeImages/TabsSample.png)

__TabScope__

| Parameter       | Description                                                  |
| --------------- | ------------------------------------------------------------ |
| __string__ path | Path to the group (see [Nesting Groups](#nesting-groups)).   |
| __string__ tabs | Names of the tabs separated by <code>&#124;</code> in C# and by `space` in ShaderLab. |
| __bool__ box    | Puts the tabs inside a box. Default: _false_.                |
| __float__ space | Adds space before the group. Default: _0_.                   |

__Tab__

| Parameter       | Description                                                  |
| --------------- | ------------------------------------------------------------ |
| __string__ path | Path to the group. Name of the group must match one of the names specified in `TabScope`. See [Nesting Groups](#nesting-groups). |

```c#
// C#
[TabScope("Tab Scope", "Left|Middle|Right", box: true)]
[Tab("./Left")]
public int one;
public int two;
public int three;

[Tab("../Middle")]
public int four;
public int five;
public int six;

[Tab("../Right")]
public int seven;
public int eight;
public int nine;
```

```javascript
// ShaderLab
[TabScope(Tab_Scope, Left Middle Right, true)]
[Tab(. Left)]
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[Tab(.. Middle)]
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0

[Tab(.. Right)]
_Seven("Seven", Float) = 0
_Eight("Eight", Float) = 0
_Nine("Nine", Float) = 0
```

### HorizontalGroup and VerticalGroup

`HorizontalGroup` and `VerticalGroup` start  horizontal and vertical groups, respectively. 

![](./ReadmeImages/HorizontalAndVerticalSample.png)

__VerticalGroup__

| Parameter       | Description                                                |
| --------------- | ---------------------------------------------------------- |
| __string__ path | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __float__ space | Adds space before the group. Default: _0_.                 |

__HorizontalGroup__

| Parameter            | Description                                                |
| -------------------- | ---------------------------------------------------------- |
| __string__ path      | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __float__ labelWidth | Label width inside the horizontal group.                   |
| __float__ space      | Adds space before the group. Default: _0_.                 |

```c#
// C#
[HorizontalGroup("Split", labelWidth: 50)]
[VerticalGroup("./Left")]
public int one;
public int two;
public int three;

[VerticalGroup("../Right")]
public int four;
public int five;
public int six;
```

```javascript
// ShaderLab
[HorizontalGroup(Split, 50)]
[VerticalGroup(. Left)]
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[VerticalGroup(.. Right)]
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0
```

### ReadOnly

`ReadOnly` and `ReadOnlyGroup` disable a property or a group of properties in the inspector. 

![](./ReadmeImages/ReadOnlySample.png)

| Parameter       | Description                                                |
| --------------- | ---------------------------------------------------------- |
| __string__ path | Path to the group (see [Nesting Groups](#nesting-groups)). |

```c#
// C#
[ReadOnly]
public int one;

[ReadOnlyGroup("Read Only Group")]
public int two;
public int three;
public int four;
```

```javascript
// ShaderLab
[ReadOnly]
_One("One", Float) = 0

[ReadOnlyGroup(Read_Only_Group)]
_Two("Two", Float) = 0
_Three("Three", Float) = 0
_Four("Four", Float) = 0
```

### Conditionals

`HideIf`, `ShowIf`, `HideIfGroup` and `ShowIfGroup` hide/show properties or groups of properties depending on a condition.

`DisableIf`, `EnableIf`,  `DisableIfGroup` and `EnableIfGroup` disable/enable properties or groups of properties depending on a condition.

Non-Group attributes work on the property they are applied to. Group attributes work like all other groups and require a path (see [Nesting Groups](#nesting-groups)).

![](./ReadmeImages/ConditionalsSample.gif)

#### C#

`HideIf`, `ShowIf`, `DisableIf`, `EnableIf`

| Parameter                     | Description                                                  |
| ----------------------------- | ------------------------------------------------------------ |
| __string__ memberName         | Member to check the value of. Can be instance or static. Can be a field, a property or a method, that takes no arguments. If no `value` is provided, must be of type `bool`. |
| __object__ value _(optional)_ | Condition will check if member equals `value` using `Equals` method. |

`HideIfGroup`, `ShowIfGroup`, `DisableIfGroup`, `EnableIfGroup`

| Parameter                     | Description                                                  |
| ----------------------------- | ------------------------------------------------------------ |
| __string__ path               | Path to the group (see [Nesting Groups](#nesting-groups)).   |
| __string__ memberName         | Member to check the value of. Can be instance or static. Can be a field, a property or a method, that takes no arguments. If no `value` is provided, must be of type `bool`. |
| __object__ value _(optional)_ | Condition will check if member equals `value` using `Equals` method. |

```c#
private bool boolField;
private bool BoolProperty => one % 2 == 0;
private static bool BoolMethod() => true;
public enum SomeEnum { Foo, Bar }
public SomeEnum state;

// Hide If Field
[HideIf(nameof(boolField))]
public int one;
[HideIf(nameof(boolField))]
public int two;

// Enable If Property
[EnableIfGroup("Enable If Property", nameof(BoolProperty))]
public int three;
public int four;
[EndGroup]

// Disable If Method
[DisableIfGroup("Disable If Method", nameof(BoolMethod))]
public int five;
public int six;
[EndGroup]

// Show If Enum Value
[ShowIf(nameof(state), SomeEnum.Foo)]
public int seven;
[ShowIf(nameof(state), SomeEnum.Bar)]
public int eight;
```

#### ShaderLab

`HideIf`, `ShowIf`, `DisableIf`, `EnableIf`

| Parameter            | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| __string__ condition | Name of the condition. Can be a float property (true if greater than zero, false otherwise), material keyword, or a global keyword (to indicate that keyword is global add G before it). |

`HideIfGroup`, `ShowIfGroup`, `DisableIfGroup`, `EnableIfGroup`

| Parameter            | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| __string__ path      | Path to the group (see [Nesting Groups](#nesting-groups)).   |
| __string__ condition | Name of the condition. Can be a float property (true if greater than zero, false otherwise), material keyword, or a global keyword (to indicate that keyword is global add G before it). |

```javascript
// Float Property Condition
_Toggle("Toggle", Float) = 0
[ShowIf(_Toggle)]
_One("One", Float) = 0
[ShowIf(_Toggle)]
_Two("Two", Float) = 0

// Material Keyword Condition
[EnableIfGroup(Enable_If_Keyword, MY_KEYWORD)]
_Three("Three", Float) = 0
_Four("Four", Float) = 0

// Global Keyword Condition
[DisableIfGroup(Hide_If_Global_Keyword, G MY_KEYWORD)]
_Five("Five", Float) = 0
_Six("Six", Float) = 0

```

### ToggleGroup

Starts a vertical group with a toggle, that can be hidden or disabled depending on the toggle value. 

![](./ReadmeImages/ToggleGroupSample.png)

#### C#

In C# `ToggleGroup` should be used on a serialized `bool` field. 

| Parameter         | Description                                                |
| ----------------- | ---------------------------------------------------------- |
| __string__ path   | Path to the group (see [Nesting Groups](#nesting-groups)). |
| __bool__ foldable | Makes the group collapsible. Default: _false_.             |
| __bool__ box      | Puts the group inside a box. Default: _true_.              |
| __float__ space   | Adds space before the group. Default: _0_.                 |

```c#
[ToggleGroup("Toggle Group")]
public bool boolean;
public int one;
public int two;
public int three;

[ToggleGroup("Foldable Toggle Group", foldable: true)]
public bool anotherBoolean;
public int four;
public int five;
public int six;
```

#### ShaderLab

In ShaderLab `ToggleGroup` should be used on a `float` property. 

| Parameter                       | Description                                                  |
| ------------------------------- | ------------------------------------------------------------ |
| __string__ path                 | Path to the group (see [Nesting Groups](#nesting-groups)).   |
| __bool__ foldable               | Makes the group collapsible. Default: _false_.               |
| __bool__ box                    | Puts the group inside a box. Default: _true_.                |
| __string__ keyword _(optional)_ | Keyword to turn on and off (like the built-in Toggle drawer). |

```javascript
[ToggleGroup(Toggle_Group)]
_Toggle("Toggle", Float) = 0
_One("One", Float) = 0
_Two("Two", Float) = 0
_Three("Three", Float) = 0

[ToggleGroup(Toggle_Group_With_Keyword, true, MY_KEYWORD)]
_AnotherToggle("Another Toggle", Float) = 0
_Four("Four", Float) = 0
_Five("Five", Float) = 0
_Six("Six", Float) = 0
```

## Special Attributes
### MarkedUpType

_C# only_

Makes attributes work inside serializable classes and structs. See [Usage: Serializable classes and structs](#serializable-classes-and-structs). Can optionally hide the target's control (foldout) and remove indent from target's children. 

```c#
[MarkedUpType]
class SomeClass
{
    ...
}

[MarkedUpType(indentChildren: false)]
struct SomeStruct
{
    ...
}
```

### MarkedUpField

_C# only_

Makes attributes work inside fields of serializable classes and structs. See [Usage: Serializable classes and structs](#serializable-classes-and-structs). Can optionally hide the target's control (foldout) and remove indent from target's children. 

```c#
[MarkedUpField]
public SomeClass one;

[MarkedUpField(indentChildren: false)]
public SomeClass two;

[MarkedUpField(indentChildren: false, showControl: false)]
public SomeClass three;
```

### InlineEditor

_C# only_

Shows the inspector of some `Unity.Object` (`MonoBehaviour`, `ScripatableObject` and `Material` are `Unity.Object`s, for instance) "inline" — embeds it in the current inspector. Can be used on an object reference field. Works in `MarkedUpEditor`s and `MarkedUpField`s inside them. 

![](./ReadmeImages/InlineEditorSample.png)

| Parameter                 |            | Description                                                  |
| ------------------------- | ---------- | ------------------------------------------------------------ |
| __InlineEditorMode__ mode | Box        | Shows object field and inspector body inside a foldable box. |
|                           | ContentBox | Shows object field with foldable inspector body.             |
|                           | Stripped   | Shows only inspector body.                                   |

```c#
[InlineEditor]
public SomeData someData;

[InlineEditor(InlineEditorMode.ContentBox)]
public SomeComponent someComponent;

[TitleGroup("Stripped")]
[InlineEditor(InlineEditorMode.Stripped)]
public SomeData stripped1;
```

### DrawSystemProperties

_ShaderLab only_

Tells `MarkedUpShaderGUI` to draw Render Queue, Enable Instancing (if applicable) and Double Sided Global Illumination properties below this property.

```javascript
[DrawSystemProperties]
_SomeProperty("Some Property", Float) = 0
```

## Custom Marked Up Inspectors

### Regular Inspectors

`MarkedUpEditor` allows to inject custom code into itself before, after and instead of any property. Here are the methods used for extension:

* ```c#
  protected bool DrawMarkedUpInspector()
  ```

  Works like `Editor`'s  `DrawDefaultInspector`, but for the marked up editor. Call it if you have overridden the `OnInspectorGUI` and want to draw `MarkedUpEditor` as is. 

* ```c#
  protected virtual void OnInitialize()
  ```

  Is called after `MarkedUpEditor` have initialized itself in `OnEnable`. 

* ```c#
  protected void AddCallback(SerializedProperty property, CallbackEvent type, Action<SerializedProperty> callback)
  ```


  Adds a callback to a specified `SerializedProperty ` at a specified `CallbackEvent`. Only one callback can be added for a given property at a given event. Should be used in `OnInitialize`.

* ```c#
  protected virtual void OnCleanup()
  ```

   Is called before `MarkedUpEditor` started cleanup in `OnDisable`. 

```c#
using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

[CustomEditor(typeof(MyComponent))]
public class MyComponentEditor : MarkedUpEditor
{
    protected override void OnInitialize()
    {
        AddCallback(serializedObject.FindProperty("one"), 
            CallbackEvent.AfterProperty, ButtonAfterOne);

        AddCallback(serializedObject.FindProperty("six"),
            CallbackEvent.BeforeProperty, ButtonBeforeSix);

        AddCallback(serializedObject.FindProperty("three"),
            CallbackEvent.ReplaceProperty, ButtonReplaceThree);
    }

    private void ButtonAfterOne(SerializedProperty property)
    {
        GUILayout.Button("After One");
    }

    private void ButtonBeforeSix(SerializedProperty property)
    {
        GUILayout.Button("Before Six");
    }

    private void ButtonReplaceThree(SerializedProperty property)
    {
        GUILayout.Button("Replace Three");
    }
}
```

![](./ReadmeImages/CustomizedInspectorSample.png)

### Material Inspectors

`MarkedUpShaderGUI` can be extended in a similar manner. It provides the following methods:

* ```c#
  protected virtual void OnInitialize(MaterialEditor materialEditor, MaterialProperty[] properties)
  ```

  Is called after `MarkedUpShaderGUI` have initialized itself. `AddCallback` should be used here.

* ```c#
  protected void AddCallback(MaterialProperty property, CallbackEvent type, Action<MaterialEditor, MaterialProperty[], MaterialProperty> callback)
  ```

  Adds a callback to a specified `MaterialProperty` at a specified `CallbackEvent`. Only one callback can be added for a given property at a given event. Should be used in `OnInitialize`.

```c#
using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

public class MyShaderEditor : MarkedUpShaderGUI
{
    protected override void OnInitialize(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        AddCallback(FindProperty("_Color", properties), CallbackEvent.AfterProperty, ButtonAfterColor);
    }

    private void ButtonAfterColor(MaterialEditor materialEditor, MaterialProperty[] properties,
        MaterialProperty property)
    {
        GUILayout.Button("Button After Color");
    }
}
```

Don't forget to tell Unity to use the modified editor for your shader.

```javascript
Shader "Unlit/MyShader"
{
    Properties
    {
        ...
    }

    CustomEditor "MyShaderEditor"
    
    ...
}
```

## MarkupGUI

Class `MarkupGUI` exposes methods for creating Markup Attributes styled groups. They can be useful for making `EditorWindows` or custom inspectors, that don't use attributes themselves.

![](./ReadmeImages/EditorWindowSample.png)

```c#
using UnityEditor;
using MarkupAttributes.Editor;

public class CustomWindow : EditorWindow
{
    MarkupGUI.GroupsStack groupsStack = new MarkupGUI.GroupsStack();
    int activeTab = 0;
    bool foldout = true;

    public static void Open()
    {
        CustomWindow window = (CustomWindow)GetWindow(typeof(CustomWindow), 
            false, "Custom Window Sample");
        window.Show();
    }

    void OnGUI()
    {
        // Always clear the groups stack.
        groupsStack.Clear();

        groupsStack += MarkupGUI.BeginBoxGroup("Box");
        // group contents ...
        groupsStack.EndGroup();

        groupsStack += MarkupGUI.BeginTitleGroup("TitleGroup", true);
        // group contents ...
        groupsStack.EndGroup();

        groupsStack += MarkupGUI.BeginFoldoutGroup(ref foldout, "Foldout");
        // group contents ...
        groupsStack.EndGroup();

        groupsStack += MarkupGUI.BeginTabsGroup(ref activeTab, 
            new string[] { "Left", "Middle", "Right" }, true);
        // group contents ...

        // Always end all groups at the end.
        groupsStack.EndAll();
    }
}

```





