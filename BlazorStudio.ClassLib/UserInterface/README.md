# BlazorStudio.ClassLib/UserInterface
This directory contains all non UI C# classes necessary for the [TransformableDisplay.razor](/BlazorStudio.RazorLib/Transformable/TransformableDisplay.razor) component and its kin.

The idea of [TransformableDisplay.razor](/BlazorStudio.RazorLib/Transformable/TransformableDisplay.razor) is to define in terms of C# all the css regarding dimensions, and positioning of HTML elements. This allows one to dynamically resize this and reposition them.

Why is:
```csharp
public List<DimensionUnit> WidthCalc { get; set; } = new();
```
a List? This is because one must maintain a base width, perhaps this is "60vw" meaning 60% of the viewport width. However, the user can click and drag to resize the width.

A mouse event is in terms of pixels. One can likely convert from pixels to the "vw" unit of measurement but this is unnecessary complexity and I feel a bad idea.

Instead of doing a conversion between the units I use a List to define a css "calc()" function. I use string interpolation and a StringBuilder instance to create the "calc()" function when rendering and inline it within "style='{...}'". See:

```html
<div class="bstudio_dialog-display"
     style="@DialogRecord.Dimensions.DimensionsCssString">
    
    <!-- ... removed to shorten snippet ... -->
</div>
```