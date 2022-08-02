# BlazorStudio.ClassLib/Store/TreeViewCase
This directory contains all non UI C# classes necessary for the [MainLayout.razor](/BlazorStudio.RazorLib/Shared/MainLayout.razor) theme. See 'class="bstudio_theme-wrapper @ThemeStateWrap.Value.ThemeKey.KeyName"' in the snippet below.

```html
<!-- ... removed to shorten snippet ... -->

<div class="bstudio_theme-wrapper @ThemeStateWrap.Value.ThemeKey.KeyName">
    <div class="bstudio_main-layout">

        <!-- ... removed to shorten snippet ... -->

        <BodyDisplay />
        
        <!-- ... removed to shorten snippet ... -->
    </div>
</div>
```

As well, one can change their theme with the [ThemeSelectTreeView.razor](/BlazorStudio.RazorLib/Theme/ThemeSelectTreeView.razor) component.

How are themes done in this app? Css variables.

See [blazorStudio-color-variable-definitions.css](/BlazorStudio.RazorLib/wwwroot/blazorStudio-color-variable-definitions.css) for the :root definitions.

See the folder [Themes/](/BlazorStudio.RazorLib/wwwroot/Themes/) for the individual themes themselves. For example, [blazorStudio-default-dark-theme.css](/BlazorStudio.RazorLib/wwwroot/Themes/blazorStudio-default-dark-theme.css)