# BlazorStudio.ClassLib/Mouse

I attempted to use [MouseFacts.cs](/BlazorStudio.ClassLib/Mouse/MouseFacts.cs) to maintain a constant that represented
the left mouse button. There is currently a bug with
the [DragDisplay.razor](/BlazorStudio.RazorLib/Drag/DragDisplay.razor) component. The bug is that sometimes the
invisible div that covers the viewport to monitor active drag events will not have the "onmouseup" event fire if the
user does certain specific actions. To remedy this I wanted to say, if "onmousemove" and left mouse is not being held
down then remove the invisible div listening for drag events. But it is not currently working.