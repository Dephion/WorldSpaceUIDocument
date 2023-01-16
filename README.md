# Dephion WorldSpaceUI Document
A small library to allow Unity's UIToolkit UIDocument to be rendered in WorldSpace.

*_This is a pre-production version and will see many improvements as time progresses._

## Setup
To use the WorldSpace UIDocument you must follow these steps:

1. Add the [WorldSpaceUIEventSystem](Runtime/Core/WorldSpaceUI/WorldSpaceUIEventSystem.cs) to your main camera.
2. Create an empty GameObject somewhere in 3D space and add [WorldSpaceUIDocument](Runtime/Core/WorldSpaceUI/WorldSpaceUIDocument.cs).
3. Configure the WorldSpaceUIDocument width, height and set the VisualTreeAsset (some root element, you can `Add()` whatever you need later).
4. Add the PanelSettings you want to use. (You can reuse PanelSettings between panels, so configure it once!)
5. Assign a RenderTexture prefab. (There is one in this package which you can assign, and reuse)

## Sample
We added a sample scene with 2 WorldSpace UIDocuments.
You can find it [here](Samples~/WorldSpaceUISample).

In this sample we override the WorldSpaceUIDocument to assign the sample view in the Start method!